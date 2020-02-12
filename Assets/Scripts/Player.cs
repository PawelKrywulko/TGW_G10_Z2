using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float moveVelocity = 20f;
    [SerializeField] private bool wallSlidingEnabled = true;

    //private variables
    private float directionFactor = 1f;
    private Rigidbody2D playerRigidbody;
    private BoxCollider2D platformChecker;
    private BoxCollider2D wallChecker;
    private Vector2 jumpVector;
    private Vector2 moveVector;
    private PlatformGenerator platformGenerator;
    private bool canDoubleJump;
    private LayerMask platformMask;
    private LayerMask wallMask;
    private float gravityScaleAtStart;
    private int collectedCoins = 0;
    private GameManager gameManager;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        platformChecker = transform.Find("PlatformChecker").GetComponent<BoxCollider2D>();
        wallChecker = transform.Find("WallChecker").GetComponent<BoxCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(moveVelocity, 0);
        platformGenerator = FindObjectOfType<PlatformGenerator>();
        platformMask = LayerMask.GetMask("Platform");
        wallMask = LayerMask.GetMask("Wall");
        gravityScaleAtStart = playerRigidbody.gravityScale;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        Jump();
        Move();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spike"))
        {
            collectedCoins = 0;
            gameManager.UpdateCollectedCoins(collectedCoins);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            directionFactor *= -1;

            if(wallSlidingEnabled)
            {
                moveVector = new Vector2(0,0);
                playerRigidbody.gravityScale = 1;
            }
            else
            {
                moveVector *= -1;
            }

            platformGenerator.SetUpPlatforms(6);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            collectedCoins++;
            gameManager.UpdateCollectedCoins(collectedCoins);
            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bank"))
        {
            gameManager.UpdateBankedCoins(collectedCoins);
            collectedCoins = 0;
            gameManager.UpdateCollectedCoins(collectedCoins);
        }
    }

    private void Move()
    {
        transform.Translate(moveVector * Time.deltaTime);

        if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            var timeBetweenTouches = Mathf.Abs(Input.GetTouch(0).deltaTime - Input.GetTouch(1).deltaTime);
            if(timeBetweenTouches > 0 && timeBetweenTouches < 0.03f)
            {
                directionFactor *= -1;
                moveVector *= directionFactor;
            }
        }
    }

    private void Jump()
    {
        #region PC Input
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (platformChecker.IsTouchingLayers(platformMask) || wallChecker.IsTouchingLayers(wallMask))
            {
                if(wallChecker.IsTouchingLayers(wallMask) && wallSlidingEnabled)
                {
                    playerRigidbody.gravityScale = gravityScaleAtStart;
                     moveVector = new Vector2(moveVelocity, 0) * directionFactor;
                }
                playerRigidbody.velocity = jumpVector;
                canDoubleJump = true;
            }
            else
            {
                if(canDoubleJump)
                {
                    playerRigidbody.velocity = jumpVector;
                    canDoubleJump = false;
                }
            }
        }
#endif
        #endregion

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (platformChecker.IsTouchingLayers(platformMask) || wallChecker.IsTouchingLayers(wallMask))
            {
                if (wallChecker.IsTouchingLayers(wallMask) && wallSlidingEnabled)
                {
                    playerRigidbody.gravityScale = gravityScaleAtStart;
                    moveVector = new Vector2(moveVelocity, 0) * directionFactor;
                }
                playerRigidbody.velocity = jumpVector;
                canDoubleJump = true;
            }
            else
            {
                if (canDoubleJump)
                {
                    playerRigidbody.velocity = jumpVector;
                    canDoubleJump = false;
                }
            }
        }
    }

    void OnDisable()
    {
        //SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public Vector3 GetCurrentPosition()
    {
        return gameObject.transform.position;
    }
}
