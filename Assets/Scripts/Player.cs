using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float moveVelocity = 20f;
    [SerializeField] private bool wallSlidingEnabled = true;
    [SerializeField] private bool doubleJumpEnabled = true;
    [SerializeField] private float maxWallSlideVelocity = 1f;

    //private variables
    private float directionFactor = 1f;
    private Rigidbody2D playerRigidbody;
    private BoxCollider2D platformChecker;
    private BoxCollider2D wallChecker;
    private Vector2 jumpVector;
    private Vector2 moveVector;
    private bool canDoubleJump;
    private bool isWallSliding = false;
    private LayerMask platformMask;
    private LayerMask wallMask;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        platformChecker = transform.Find("PlatformChecker").GetComponent<BoxCollider2D>();
        wallChecker = transform.Find("WallChecker").GetComponent<BoxCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(0, 0);
        platformMask = LayerMask.GetMask("Platform");
        wallMask = LayerMask.GetMask("Wall");
           GameEvents.Instance.OnGameStarts += StartPlayer;
    }

    private void StartPlayer()
    {
        moveVector = new Vector2(moveVelocity, 0);
    }

    void Update()
    {
        Jump();
        Move();
        WallSlide();
    }

    void FixedUpdate()
    {
        //Clamp y velocity while wall sliding
        if(isWallSliding)
        {
            if(playerRigidbody.velocity.y < -maxWallSlideVelocity)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -maxWallSlideVelocity);
            }
        }
    }
    private void WallSlide()
    {
        if (wallChecker.IsTouchingLayers(wallMask))
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
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
            OnWallTouch();

            GameEvents.Instance.HandleWallTriggerEntered(new PlayerWallEntered
            {
                PlayerPosition = transform.position
            });
        }
    }

    private void OnWallTouch()
    {
        directionFactor *= -1;

        if (wallSlidingEnabled)
        {
            moveVector = new Vector2(0, 0);

            if(playerRigidbody.velocity.y > 1)
                playerRigidbody.velocity = new Vector2(0, 1);
        }
        else
        {
            moveVector *= -1;
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
    }

    private void Jump()
    {
        #region PC Input

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJumping();
        }

#endif
        #endregion

        #region Mobile Input
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleJumping();
        }
        #endregion
    }

    private void HandleJumping()
    {
        if (platformChecker.IsTouchingLayers(platformMask) || wallChecker.IsTouchingLayers(wallMask))
        {
            if (wallChecker.IsTouchingLayers(wallMask) && wallSlidingEnabled)
            {
                moveVector = new Vector2(moveVelocity, 0) * directionFactor;
            }

            playerRigidbody.velocity = jumpVector;
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump && doubleJumpEnabled)
            {
                playerRigidbody.velocity = jumpVector;
                canDoubleJump = false;
            }
        }
    }

    public void ResetPosition()
    {
        SceneManager.LoadScene(0);
    }
}
