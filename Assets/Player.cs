using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.velocity = jumpVector;
        }
        
        transform.Translate(moveVector * Time.deltaTime);
    }
}
