using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{

    public float maxSlopeAngle = 80;

    SpriteRenderer sprite;
    public CollisionInfo collisions; // this struct contains all of our collisions info
    [HideInInspector]
    public Vector2 playerInput;
    Character gameChar;
    GameObject waypoint;
    Player player;


    // inherit parent Start() and override with additional code
    // in this case we just added facing direction
    public override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        gameChar = GetComponent<Character>();
        sprite = GetComponent<SpriteRenderer>();
        waypoint = GameObject.Find("Waypoints");
        collisions.faceDir = 1;

    }

    // use this method if no input is given as parameter

    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    // use this when input is given
    // this is where we handle movement
    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        
        UpdateRaycastOrigins();  // update the position of character and raycast location everytime we move
        
        collisions.Reset(); // reset the collisioninfo when character is about to move
        collisions.moveAmountOld = moveAmount; // store this for later use when moving in slope
        playerInput = input; // store this for later use

        // when our movement is downwards (-y) we need to handle descending slopes 
        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        
        // logic to handle character facing
        if (moveAmount.x != 0) // horizontal movement detected
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x); // faceDir will be 1 or -1 depending on x vector
        }


        // always handle horizontal collisions when moving
        HorizontalCollisions(ref moveAmount);

        // handle vertical collisions when we move in y direction
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        //flip characters sprite according to facing direction
        if (player.getDirectionalInput().x != 0)
        {
            sprite.flipX = (player.getDirectionalInput().x == -1) ? false : true;

        }



        // after all collisions are handled, we get final moveAmount and translate our position accordingly
        transform.Translate(moveAmount);


        // character will not fall through obstacle if bool is true
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    // when we move, call this
    // handle collisions in x axis and logic to ascending slopes
    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir; 
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth; // rays casted by move amount's length

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth; // minimum rayLength
        }

        // if horizontal rays (starting from bottom left or right) encounter an obstacle, hit detection info is generated
        for (int i = 0; i < horizontalRayCount; i++)
        {
            
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // we check our direction and cast rays facing that way with if-conditions
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); // move the raycasts vector up evenly each iteration

            // Shoot raycasts from rayOrigin height, to facing direction, with movement length and layer mask
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            
            // For debugging and clarity, draw rays in scene view
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);


            // raycast hits obstacle - handle it
            if (hit)
            {
                // we are at obstacle, skip to next ray, also this fixes bugs with moving platforms
                if (hit.distance == 0)
                {
                    continue;
                }
                //Deadly tag preset, kill character
                if (hit.collider.tag == "Deadly" && PlayerInput.isAlive)
                {
                    gameChar.ChangeHP(-gameChar.HealthPoints);
                }

                //Deadly tag preset, damage character by 1 hp
                if (hit.collider.tag == "Damage")
                {
                    gameChar.ChangeHP(-1);
                }
                //If we collide with Waypoint tagged collider, assign new respawn location for player, also let player move through it
                if (hit.collider.tag == "Waypoint")
                {
                    GameObject wpp = hit.transform.gameObject; // get object we hit
                    Vector3 wp;
                    wp = wpp.transform.position; // get objects position
                    if (gameChar.spawnPoint != wp)
                    {
                        gameChar.spawnPoint = wp; // update position as players new spawn
                    }
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); // character hits an angle, get angle
                
                //first ray from the bottom detects a valid slopeAngle
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    // when are going down a slope
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld; // use the original moveAmount stored earlier when descending
                    }
                    float distanceToSlopeStart = 0;

                    // check changes in slope angles
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        
                        // slow down movement in slopes
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }

                    //use the method to climb obstacles with valid angle
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                // when we arent climbing OR slopeAngle is too big
                // we move to a direction, with skinWidth excluded
                // update rayLength with hit distance
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX; 
                    rayLength = hit.distance;

                    /* not used at the moment
                     * 
                     * if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }
                    */

                    // report collision facing by equal checking directionX value
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }
    // logic to vertical collisions
    // 
    void VerticalCollisions(ref Vector2 moveAmount)
    {
        // get our y direction
        // assign ray length
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        
        // start creating rays from the top left or bottom left and get their collision info
        for (int i = 0; i < verticalRayCount; i++)
        {
            // we start shooting vertical rays like in horizontal section
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            // draw it to scene
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            // hit detected 
            if (hit)
            {
                //Deadly tag preset, kill character
                if (hit.collider.tag == "Deadly" && PlayerInput.isAlive)
                {
                    gameChar.ChangeHP(-gameChar.HealthPoints);
                }

                //Deadly tag preset, damage character by 1 hp
                if (hit.collider.tag == "Damage")
                {
                    gameChar.ChangeHP(-1);
                }

                // check for tag Through, it will let player to jump from beneath and dropdown from the platform
                if (hit.collider.tag == "Through")
                {
                    // skip to next ray, fixes a bug with moving platform and player position
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }

                    // let player be inside obstacle while true
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    // check for down input and let character to fall through it
                    // then reset and exit
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }
                //If we collide with Waypoint tagged collider, assign new respawn location for player, also let player move through it
                if (hit.collider.tag == "Waypoint")
                {
                    GameObject wpp = hit.transform.gameObject; // get object we hit
                    Vector3 wp;
                    wp = wpp.transform.position; // get objects position
                    if (gameChar.spawnPoint != wp)
                    {
                        gameChar.spawnPoint = wp; // update position as players new spawn
                    }
                    continue;
                }
                // movement vertically and rayLength updated
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                //while we are climbing a slope we need to get update vertical move amount with tangent and the angle
                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                //update location where collision happens
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        // slope detected and we calculate our ascend amount
        // start by casting rays from bottom left or bottom right and getting hit detection info
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            // hit detected, get angle and check for angle changes
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // get new horizontal move amount, also update collisions info
                // calculate on every angle change
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }


    // logic for climbing slope, we need the movevement, angle and slope normal
    // moveAmount turned into absolute value
    // we need to move up vertically by y amount, so use sine with slopeAngle and moveDistance
    // when we dont have y velocity or it is equal to our climb amount, we keep giving moveAmount new y value
    // and assign x velocity with new value with cosine, moveDistance and amount
    // also tell collision detection that we have a surface below and we are climbing
    // update collision info with current slope vectors
    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {

        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    // logic to handle descending slope
    // cast rays and check which direction we will slide down and start the descending if we arent already
    // desceding will run until valid slope angle
    void DescendSlope(ref Vector2 moveAmount)
    {
        // raycasts to check collisions below
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        // use XOR operator here to determine which direction we are descending
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }
        // if we are not already sliding, we start the descend
        if (!collisions.slidingDownMaxSlope)
        {
           
            float directionX = Mathf.Sign(moveAmount.x); // get the direction
            
            // check direction and if-condition to determine which side will start the raycast
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask); // shoot rays according to previous condition, length is infinity

            // succesful hit
            if (hit)
            {
                // get the angle 
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // we dont want to descend a zero angle wall, rather a valid angle slope
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    //slope detection, returns true when character is descending any slope
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        // check if distance to collision to slope angle with use of tangent
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            //calculate x and y amount with sine and cosine
                            float moveDistance = Mathf.Abs(moveAmount.x); // get moveAmounts absolute value
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance; // get y descend value with sine, angle and distance
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x); // assign new x value with cosine, angle, distance and direction
                            moveAmount.y -= descendmoveAmountY; // new descend y value

                            //update collision info
                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }
    // automatic descending
    // get the parameters from hit detection and move amount
    // detection occured
    // we get the collision info from raycasts so we can calculate the amount we descend until we hit a valid angle
    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            //hit detected and we need to run this until a valid angle occurs
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad); // slide this amount

                //update collisions info 
                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }

    }

    // method to stop falling through platforms
    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }


    // collision info
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        //resets everything
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

}