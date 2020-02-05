using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    [SerializeField] public float jumpHeight = 7f;
    [SerializeField] public float moveVelocity = 20f;

    //private variables
    private Rigidbody2D playerRigidbody;
    private CapsuleCollider2D playerFeet;
    private Vector2 jumpVector;
    private Vector2 moveVector;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerFeet = GetComponent<CapsuleCollider2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(moveVelocity, 0);
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
        if(!playerFeet.IsTouchingLayers(LayerMask.GetMask("Platform"))) { return; }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.velocity = jumpVector;
        }
#endif

        if (Input.touchCount == 1)
        {
            playerRigidbody.velocity = jumpVector;
        }
    }

    public void ResetPosition()
    {
        gameObject.SetActive(true);
        transform.position = new Vector2(1.17f, 5f);
    }
}
