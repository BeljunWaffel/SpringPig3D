using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float _movementMultiplier;
    public int _energy;

    private Rigidbody player;
    private float distToGround;
    private bool wasGrounded = false;
    private bool isMovingHorizontally = false;
    
    // Needed to calculate energy
    private float maxHeight = -1;
    private float startingHeight;
    
    // Box interactions
    private bool isPushingBoxInX = false;
    private bool isPushingBoxInZ = false;

    // DEBUG VARIABLES
    public bool IsDebug = false;
    
    void Start()
    {
        player = GetComponent<Rigidbody>();

        // Get collider to calculate distance to ground for IsGrounded() function.
        var collider = GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;

        wasGrounded = IsGrounded();
    }

    // Applied before physics
    void FixedUpdate()
    {
        PerformHorizontalMovement();
        PerformVerticalMovement();
    }

    /**
     * 
     * GETTERS AND SETTERS
     * 
     */
    public bool IsMovingHorizontally()
    {
        return isMovingHorizontally;
    }

    public bool IsPushingBoxInX
    {
        get
        {
            return isPushingBoxInX;
        }

        set
        {
            isPushingBoxInX = value;
        }
    }

    public bool IsPushingBoxInZ
    {
        get
        {
            return isPushingBoxInZ;
        }

        set
        {
            isPushingBoxInZ = value;
        }
    }

    /**
     * 
     * DISPLAY
     * 
     */
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("Energy: {0}", _energy));
    }

    /**
     * 
     * MOVEMENT
     * 
     */
    private void PerformVerticalMovement()
    {
        var jump = Input.GetAxis("Jump");
        var miniJump = Input.GetAxis("MiniJump");
        var isGrounded = IsGrounded();
        
        // If player was in the air before but isn't anymore, calculate how much energy was gained.
        if (!wasGrounded && isGrounded)
        {
            var energyGain = Mathf.RoundToInt(startingHeight - player.position.y - Constants.ENERGY_DOWNGRADE);
            IncrementEnergy(energyGain);
        }

        wasGrounded = isGrounded;

        if (IsDebug) PrintMaxJumpHeight();

        // Vertical Movement
        if (isGrounded)
        {
            startingHeight = player.position.y;

            if (jump != 0)
            {
                // Apply velocity directly, since we want an immediate change.
                // https://docs.unity3d.com/ScriptReference/Rigidbody-velocity.html
                player.velocity = new Vector3(player.velocity.x, CalculateJumpVelocity(_energy, includeClearance: true), player.velocity.z);
            }
            else if (miniJump != 0)
            {
                player.velocity = new Vector3(player.velocity.x, CalculateJumpVelocity(Constants.MINI_JUMP_HEIGHT, includeClearance: true), player.velocity.z);
            }
        }
    }

    private void PerformHorizontalMovement()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            player.velocity = new Vector3(0f, player.velocity.y, 0f);
            isMovingHorizontally = false;
        }
        else
        {
            // Horizontal movement
            Vector3 movement;
            movement = new Vector3(moveHorizontal * _movementMultiplier,
                                   player.velocity.y,
                                   moveVertical * _movementMultiplier);

            if (isPushingBoxInX && movement.x != 0)
            {
                movement.x = GetPlayerPushingBoxSpeed(movement.x);
            }

            if (isPushingBoxInZ && movement.z != 0)
            {
                movement.z = GetPlayerPushingBoxSpeed(movement.z);
            }

            player.velocity = movement;
            isMovingHorizontally = true;
        }
    }

    private float CalculateJumpVelocity(int jumpHeight, bool includeClearance)
    {
        /*
            Potentially useful link if we want to go with the catapult idea https://forum.unity3d.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/
            useful link? https://www.youtube.com/watch?time_continue=171&v=v1V3T5BPd7E 
            v0 = sqrt(v^2 - 2*g*h). Solve for when v = 0 and h = energy, so:
            v0 = sqrt(-2*g*h)
        */
        
        var gravity = -1 * Physics.gravity.magnitude;
        var height = includeClearance ? jumpHeight + Constants.JUMP_CLEARANCE : jumpHeight;
        float yVelocity = Mathf.Sqrt(-2 * gravity * height);
        return yVelocity;
    }
    
    private bool IsGrounded()
    {
        var isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        return isGrounded;
    }

    private void IncrementEnergy(int energy)
    {
        _energy += energy;
        if (_energy > Constants.MAX_ENERGY)
        {
            _energy = Constants.MAX_ENERGY;
        }
        if (_energy < Constants.MIN_ENERGY)
        {
            _energy = Constants.MIN_ENERGY;
        }
    }

    /**
     * 
     * COLLISIONS
     * 
     */
    void OnCollisionExit(Collision collision)
    {
        if (TagList.ContainsTag(collision.gameObject, Constants.TAG_BOX))
        {
            isPushingBoxInX = false;
            isPushingBoxInZ = false;
        }
    }

    private float GetPlayerPushingBoxSpeed(float playerVelocity)
    {
        // If player velocity is < Box speed, return player velocity.
        // Otherwise return boxspeed in the direction the player was moving.
        return (Mathf.Abs(playerVelocity) < Constants.BOX_SPEED) ?
               playerVelocity :
               MathUtils.IsPositive(playerVelocity) * Constants.BOX_SPEED;
    }

    // Prints out the max height
    private void PrintMaxJumpHeight()
    {
        // Calculate max height
        if (!IsGrounded() && player.position.y > maxHeight)
        {
            maxHeight = player.position.y;
        }

        if (IsGrounded() && maxHeight != -1)
        {
            Debug.Log(maxHeight);
        }
    }
}
