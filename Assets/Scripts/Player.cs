using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float moveVelocity = 20f;

    //private variables
    private Rigidbody2D playerRigidbody;
    private CapsuleCollider2D playerFeet;
    private Vector2 jumpVector;
    private Vector2 moveVector;
    private PlatformGenerator platformGenerator;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerFeet = GetComponent<CapsuleCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(moveVelocity, 0);
        platformGenerator = FindObjectOfType<PlatformGenerator>();
    }

    void Update()
    {
        Jump();
        Move();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            moveVector *= -1;
            platformGenerator.SetUpPlatforms(6);
        }

        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Spike"))
        {
            gameObject.SetActive(false);
        }
    }

    private void Move()
    {
        transform.Translate(moveVector * Time.deltaTime);
    }

    private void Jump()
    {
        if(!playerFeet.IsTouchingLayers(LayerMask.GetMask("Platform")) && !playerFeet.IsTouchingLayers(LayerMask.GetMask("Wall"))) 
        { 
            return; 
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.velocity = jumpVector;
        }
#endif

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            playerRigidbody.velocity = jumpVector;
        }
    }

    public void ResetPosition()
    {
        SceneManager.LoadScene(0);
    }

    public Vector3 GetCurrentPosition()
    {
        return gameObject.transform.position;
    }
}
