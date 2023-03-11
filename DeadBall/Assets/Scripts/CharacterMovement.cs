using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float movementSpeed = 12f;
    private Vector2 direction;

    [Header("Components")]
    public GameObject playerBall;
    public Rigidbody2D rb;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Vertical Movement")]
    public float jumpForce = 10f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Collision")]
    public bool onGround = true;
    public float groundLength = 0.08f;
    public float groundWidth = 0.01f;
    public Collider2D floorCollider;
    public ContactFilter2D floorFilter;
    public LayerMask ground;

    [Header("Destroy")]
    public float verticalCutOff = -8f;


    //private void Start() => player = GetComponent<Rigidbody2D>();

    void Start()
    {
        for (int i = 0; i < GlobalVariable.playerBalls.Length; i++)
        {
            if (GlobalVariable.playerBalls[i] == null)
            {
                GlobalVariable.playerBalls[i] = playerBall;
                break;
            }
        }
        

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        DestroyObject();
    }

    private void FixedUpdate()
    {
        moveCharacter(direction.x);

        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }

        ModifyPhysics();
    }

    private void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * movementSpeed);

        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    private void ModifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
       
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    private void CheckGround()
    {
        float extraHeight = groundLength;
        float minusWidth = groundWidth;

        RaycastHit2D rayCastHit = Physics2D.BoxCast(floorCollider.bounds.center, floorCollider.bounds.size - new Vector3(minusWidth * 2, 0), 0f, Vector2.down, extraHeight, ground);
        
        Color rayColor;
        if (onGround)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(floorCollider.bounds.center + new Vector3(-minusWidth, 0) + new Vector3(floorCollider.bounds.extents.x, 0) , Vector2.down * (floorCollider.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(floorCollider.bounds.center - new Vector3(-minusWidth, 0) - new Vector3(floorCollider.bounds.extents.x, 0), Vector2.down * (floorCollider.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(floorCollider.bounds.center - new Vector3(floorCollider.bounds.extents.x - minusWidth, floorCollider.bounds.extents.y + extraHeight), Vector2.right * ((floorCollider.bounds.extents.x - minusWidth) * 2), rayColor);

        if (rayCastHit.collider != null)
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

    }

    private void DestroyObject()
    {
        if (playerBall.transform.position.y < verticalCutOff)
        {
            Destroy(playerBall);
            GlobalVariable.ConsolidateArray();
        }
    }


}
