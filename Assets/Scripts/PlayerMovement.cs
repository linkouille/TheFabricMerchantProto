using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    //Rigibody 2D Component handling physics2D
    private Rigidbody2D rb;
    private InputHolder iH;

    //States of the Player
    private bool canMove = true;
    private bool canJump = true;
    private bool requestJump = false;
    private bool requestWallJump = false;
    private bool slide = false;
    private bool isWalljumping = false;

    //Variables for jump
    [SerializeField] private float speed = 8;
    [SerializeField] private float jumpHeight = 12;

    [SerializeField] private float slideSpeed = 5;

    //Size of the circle that detect if the player is near a wall
    [SerializeField] private float groundDist = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    //Coyote time Timer
    [SerializeField] private float hangTime = 0.1f;
    private float hangCounter;

    //Jump Buffer Timer
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    //Jump Apex Detection
    [SerializeField] private float apexVel = 1.5f;
    //Apex charateristics
    [SerializeField] private float apexGravityScale = 0.6f;
    [SerializeField] private float airControlVel=6;
    
    [SerializeField] private float maxFallVelocity = 20;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        iH = GetComponent<InputHolder>();
    }

    private void Update()
    {
        // COYOTE TIME
        if (IsGrounded())
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime;
        }

        // JUMP BUFFER
        
        if (iH.jumpB)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        //Launch Jump
        if (jumpBufferCounter >= 0 && hangCounter > 0f && canJump)
        {
            requestJump = true;
            jumpBufferCounter = -1;
        }

        //Use OnSlideCollide to detect if the PLayer is sliding
        slide = (OnSideCollide(Vector2.right) || OnSideCollide(Vector2.left)) && !IsGrounded();
        //WallJump
        if(slide && iH.jumpB)
        {
            requestWallJump = true;
        }
        //Player Can Move at the moment he is on the ground but IsGrounded() false doesn't mean canMove False (aircontrol)
        if (IsGrounded())
        {
            canMove = true;
        }
    }

    private void FixedUpdate ()
    {
        //Horizontal Movement
        if (canMove)
        {
            if(IsGrounded())
                rb.velocity = Vector2.right * iH.input.x * speed + Vector2.up * rb.velocity.y;
            else
                rb.velocity = Vector2.right * iH.input.x * airControlVel + Vector2.up * rb.velocity.y;

            //Detach from the wall when sliding
            if (slide)
            {
                if((iH.input.x > 0 && OnSideCollide(Vector2.right)) || ((iH.input.x < 0 && OnSideCollide(Vector2.left)))) // We want to push the wall
                {
                    rb.AddForce(Vector2.right * iH.input.x * speed, ForceMode2D.Impulse);
                }
                rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            }

        }
        
        //We requested Jump
        if (requestJump)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            StartCoroutine(IDisableJump(0.1f));
            requestJump = false;
        }

        //We requested a WallJump
        if (requestWallJump)
        {
            //Compute the direction of the wall
            int side = OnSideCollide(Vector2.right) ? -1 : 1;
            rb.velocity = new Vector2(0, 0);

            //We Impulse at the oposite side
            rb.AddForce((Vector2.up * jumpHeight + side * Vector2.right * jumpHeight * 0.5f), ForceMode2D.Impulse);
            
            requestWallJump = false;
            StartCoroutine(IDisableMove(.5f));
        }


        //Detect if APEX of the Jump
        if (!IsGrounded() && rb.velocity.y < apexVel && rb.velocity.y > -apexVel && !isWalljumping)
        {
            rb.gravityScale = apexGravityScale;
        }
        else
        {
            rb.gravityScale = 3;
        }

        //Clamp vertical velocity for bether fall control
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y < -maxFallVelocity ? -maxFallVelocity : rb.velocity.y);

    }

    public bool OnSideCollide(Vector3 dir)
    {
        return Physics2D.CircleCast(transform.position + GetComponent<CircleCollider2D>().radius * dir.normalized, groundDist, Vector2.zero, 0, groundLayer);
    }


    public bool IsGrounded()
    {
        return OnSideCollide(Vector3.down);
    }

    public bool IsAboved()
    {
        return OnSideCollide(Vector3.up);
    }

    public Vector3 getVel()
    {
        return rb.velocity;
    }

    
    IEnumerator IDisableMove(float time)
    {
        bool prevcanMoveval = canMove;
        isWalljumping = true;
        canMove = false;
        yield return new WaitForSeconds(time);
        isWalljumping = false;
        canMove = prevcanMoveval;
    }

    IEnumerator IDisableJump(float time)
    {
        canJump = false;
        yield return new WaitForSeconds(time);
        canJump = true;
    }
}
