using Assets.Scripts.Events;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float moveVelocity = 20f;
    [SerializeField] private bool wallSlidingEnabled = true;
    [SerializeField] private bool doubleJumpEnabled = true;
    [SerializeField] private float maxWallSlideVelocity = 1f;
    [SerializeField] private Animator animator;

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
        SetPlayerColor();
        playerRigidbody = GetComponent<Rigidbody2D>();
        platformChecker = transform.Find("PlatformChecker").GetComponent<BoxCollider2D>();
        wallChecker = transform.Find("WallChecker").GetComponent<BoxCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(0, 0);
        platformMask = LayerMask.GetMask("Platform");
        wallMask = LayerMask.GetMask("Wall");
        GameEvents.Instance.OnGameStarts += StartPlayer;
        GameEvents.Instance.OnItemInShopBought += ChangePlayerColor;
    }

    private void StartPlayer()
    {
        moveVector = new Vector2(moveVelocity, 0);
    }

    void Update()
    {
        if(platformChecker.IsTouchingLayers(platformMask) || wallChecker.IsTouchingLayers(wallMask))
        {
            animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsJumping", true);
        }

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
            if (collision.gameObject.CompareTag("LeftWall"))
            {
                animator.SetBool("IsSlidingLeft", true);
            }
            if (collision.gameObject.CompareTag("RightWall"))
            {
                animator.SetBool("IsSlidingRight", true);
            }

            OnWallTouch();
            
            GameEvents.Instance.HandleWallTriggerEntered(new PlayerWallEntered
            {
                PlayerPosition = transform.position,
                EnteredWallName = collision.gameObject.name
            });
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (collision.gameObject.CompareTag("LeftWall"))
            {
                animator.SetBool("IsSlidingLeft", false);
            }
            if (collision.gameObject.CompareTag("RightWall"))
            {
                animator.SetBool("IsSlidingRight", false);
            }
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bank") && GameManager.collectedCoins != 0)
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
            if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                HandleJumping();
            }
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

            AudioManager.Instance.Play("Jump", "SfxEnabled");
            playerRigidbody.velocity = jumpVector;
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump && doubleJumpEnabled)
            {
                AudioManager.Instance.Play("Jump", "SfxEnabled");
                playerRigidbody.velocity = jumpVector;
                canDoubleJump = false;
            }
        }
    }

    private void ChangePlayerColor(ItemInShopBought item)
    {
        GetComponent<SpriteRenderer>().color = item.ItemColor;
    }

    private void SetPlayerColor()
    {
        string currentColorHash = PlayerPrefs.GetString("CurrentPlayerColor", "#000000");
        ColorUtility.TryParseHtmlString(currentColorHash, out Color color);
        GetComponent<SpriteRenderer>().color = color;
    }
}
