using Assets.Scripts.Events;
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
    private bool canDoubleJump;
    private LayerMask platformMask;
    private LayerMask wallMask;
    private float gravityScaleAtStart;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        platformChecker = transform.Find("PlatformChecker").GetComponent<BoxCollider2D>();
        wallChecker = transform.Find("WallChecker").GetComponent<BoxCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(moveVelocity, 0);
        platformMask = LayerMask.GetMask("Platform");
        wallMask = LayerMask.GetMask("Wall");
        gravityScaleAtStart = playerRigidbody.gravityScale;
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
            GameEvents.Instance.HandleSpikeTriggerEntered();
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

            GameEvents.Instance.HandleWallTriggerEntered(new PlayerWallEntered
            {
                PlayerPosition = transform.position
            });
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            GameEvents.Instance.HandleCoinTriggerEntered();
            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bank"))
        {
            GameEvents.Instance.HandleBankTriggerEntered();
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

    public void ResetPosition()
    {
        SceneManager.LoadScene(0);
    }
}
