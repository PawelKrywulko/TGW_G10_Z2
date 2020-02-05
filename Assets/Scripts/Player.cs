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
    private Vector2 jumpVector;
    private Vector2 moveVector;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        jumpVector = new Vector2(0, jumpHeight);
        moveVector = new Vector2(moveVelocity, 0);
    }

    void Update()
    {
        Jump();
        Move();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag.Contains("Wall"))
        {
            moveVector *= -1;
        }
    }

    private void Move()
    {
        transform.Translate(moveVector * Time.deltaTime);
    }

    private void Jump()
    {
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
}
