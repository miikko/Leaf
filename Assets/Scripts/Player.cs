using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //we use these variables for player tracking
    public GameObject waypoint;
    private float timer = 0.5f;

    // store our jumping variables and movement
    // jumping has different heights
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public bool hasWallClimb = false;

    // wall jumping and wall sliding variables
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    bool wallSliding;
    int wallDirX;

    // our gravity and velocity variables
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    PlayerInput getInput;

    public Animator anim;

    Controller2D controller;

    Vector2 directionalInput;
    AttackCube atk;


    // assign controller to Controller2D and define gravity and jumping velocity
    // we use adjustable gravity and calculate our min and max jump velocity based on given gravity 
    // max velocity is gravity's absolute value times timeToJumpApex
    // min velocity is square root of 2 times absolute gravity times minJumpHeight
    // default values are:
    // gravity -50
    // maxJumpVelocity 20
    // minJumpVelocity 10
    void Start()
    {
        getInput = GetComponent<PlayerInput>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        atk = GameObject.Find("PlayerCharacter/AttackBox").GetComponent<AttackCube>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2); // negative value
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    // calculate our character velocity in both axis, check if wall sliding is true
    // use Move() with this velocity and input
    // check for collisions info if we collided above or below
    // on collision below and above, y velocity is set to zero
    // and collision below with slidingDownMaxSlope = true, characters y velocity will go down (affected by gravity) and slope angle
    void Update()
    {
        CalculateVelocity();
        // Wall Sliding disabled by default
        if (hasWallClimb)
        { 
            HandleWallSliding();
        }

        controller.Move(velocity * Time.deltaTime, directionalInput);

        // handle animations

        // set idle animation character when grounded and no input
        if (directionalInput.x == 0 && controller.collisions.below && !atk.IsAttacking())
        {
            anim.SetTrigger("Idle");
        }
        //set run animation when input is given, is grounded and has velocity.x
        if (controller.collisions.below && getInput.directionalInput.x != 0 && velocity.x != 0 && !atk.IsAttacking())
        {
            anim.SetTrigger("Run");
        }

        // trigger jump animation while airborne
        if (!controller.collisions.below && !atk.IsAttacking())
        {
            anim.ResetTrigger("Run");
            anim.SetTrigger("Jump");
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        //Updating player position on certain intervals determined by the "timer"
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {

            timer = 0.5f;
        }
    }

    // set player input in diretionalInput
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public Vector2 getDirectionalInput()
    {
        return directionalInput;
    }

    // implement jump logic here, also while sliding
    // jumping from the wall with x axis input will increase or decrease the velocity
    // jumping from the wall without input has its own variable also
    // forbid on jumping from max angle slope
    // start jumping on normal conditions while key is pressed down
    // jump velocity is maxJumpVelocity while key is down
    // on release we slow the velocity

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }
    // on key release we will not slow down jump velocity until minimum jump value
    // jump velocity is slowed down when value is reached

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    // wall sliding method
    // get the direction of the wall which we slide in
    // character will grab onto the wall with no collision below and downward velocity
    // set our y velocity to wall sliding speed
    // handle wall stickiness - set x axis velocity to zero with no smoothing
    // we stop sticking to wall if directional input is opposite to wall direction, else we stick to the wall for wallStickTime
    // 
    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }

    }
    // method to calculate velocity with global gravity variable
    // get movespeed first in x axis
    // goal is to get smooth acceleration to the desired velocity
    // use Mathf.SmoothDamp() with current and target velocity, take a ref smoothing variable
    // last parameter is the smoothing time and will be determined by our air status (airborne or grounded)
    // now update y velocity with gravity and deltaTime

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }
}