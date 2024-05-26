using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    Animator doAnimations;

    int wallJumpTimer;

    [SerializeField]
    float horizontalInput;
    public float groundAcceleration;
    float airAcceleration;
    public float defaultAirAcceleration;
    float airDeceleration = 0.97f;
    [SerializeField]
    bool grounded;
    [SerializeField]
    float Currentspeed;
    public float jumpStrength;

    
    public Transform groundCollisionPosition;
    public Vector2 groundCheckSize;
    public LayerMask ground;

    public Transform wallCheckPosition;
    public Vector2 wallCheckSize;
      
    void Start()
    {
        airAcceleration = defaultAirAcceleration;
        rb = GetComponent<Rigidbody2D>();
        doAnimations = GetComponent<Animator>();
        
    }

    private bool touchingWall()
    {
        if (Physics2D.OverlapBox(wallCheckPosition.position, wallCheckSize, 0, ground))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 4.5f;
        }
        else
        {
            rb.gravityScale = 3f;
        }
        doAnimations.SetBool("Grounded", grounded);
        if (wallJumpTimer <= 0)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        


        if (horizontalInput != 0)
        {
            doAnimations.SetBool("Moving", true);
            //face left and right when moving left and right
            if (horizontalInput == -1)
            {
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
            else
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }

            //movement left and right
            if (grounded)
            {
                Currentspeed += horizontalInput * groundAcceleration;
            }
            else
            {
                Currentspeed += horizontalInput * airAcceleration;
            }

            //stop the player when touching wall
            if (touchingWall())
            { 
                Currentspeed = 0;
            }
           

            
            
        }
        else
        {
            doAnimations.SetBool("Moving", false);
        }
        
        rb.velocity= (new Vector2(Currentspeed, rb.velocity.y));
        
        //decelarete to when not holding a direction
        
            if (grounded)
            {
            
                Currentspeed *= 0.98f;
            
            
            }
            else
            {
                Currentspeed *= airDeceleration;
            }
            if (Mathf.Abs(Currentspeed) < 0.01f)
            {
                Currentspeed = 0;
            }

        if (touchingWall() && !grounded)
        {
            doAnimations.SetBool("Walljumping", true);
        }
        else
        {
            doAnimations.SetBool("Walljumping", false);
        }
        
        //jump
        if (Input.GetKeyDown(KeyCode.Space)&&grounded)
        {
            rb.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
        }
        
        //walljump
        if (Input.GetKeyDown(KeyCode.Space) && !grounded && touchingWall())
        {
            wallJumpTimer = 100;
            horizontalInput = -horizontalInput;
            


            if (transform.rotation.y == 0)
            {
                rb.velocity = (new Vector2(0, jumpStrength / 1.3f));
                Currentspeed = -10;
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
            else
            {
                rb.velocity = (new Vector2(0, jumpStrength / 1.3f));
                Currentspeed = 10;
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
        wallJumpTimer--;

        grounded = (Physics2D.OverlapBox(groundCollisionPosition.position, groundCheckSize, 0, ground));
        
        

        //cancel jump when not pressing space
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }

        if (wallJumpTimer>0)
        {
            airAcceleration = defaultAirAcceleration/10;
            airDeceleration = 0.99f;
        }
        else
        {
            airAcceleration = defaultAirAcceleration;
            airDeceleration = 0.97f;
        }
        

        


    }
    //visual indicator of jumpdetection
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(groundCollisionPosition.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckSize);
    }
}
