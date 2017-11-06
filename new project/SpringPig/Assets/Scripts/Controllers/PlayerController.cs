using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float _movementMultiplier;
    public int _energy;
    public int _gravityMagnitude;
    public Transform _cameraRig;

    private Rigidbody player;
    private float distToGround;
    private bool isMovingHorizontally = false;
    
    // Needed to calculate energy
    private bool wasGrounded = false;
    private bool wasInAirBecauseOfMiniJump = false;
    private int maxHeight = -1;
    private float startingHeight;
    
    // Box interactions
    private bool isPushingBoxInX = false;
    private bool isPushingBoxInZ = false;

    // DEBUG VARIABLES
    public bool IsDebug = false;
    
    void Start()
    {
        player = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, -1f * _gravityMagnitude, 0f);

        // Get collider to calculate distance to ground for IsGrounded() function.
        var collider = GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;

        wasGrounded = IsGrounded();
    }

    // Applied before physics
    void FixedUpdate()
    {
        transform.forward = _cameraRig.forward;

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

        // If in the air, calculate max height to determine amount of energy to gain.
        int currentHeight = Mathf.RoundToInt(player.position.y - player.transform.localScale.y);
        if (!IsGrounded() && currentHeight > maxHeight)
        {
            maxHeight = currentHeight;
        }

        // If player was in the air before but isn't anymore, calculate how much energy was gained.
        if (!wasGrounded && isGrounded)
        {
            if (wasInAirBecauseOfMiniJump && currentHeight < startingHeight)
            {
                // If you minijump to get to a lower spot, lose 1 extra energy because you were able to jump by 1.
                var energyGain = Mathf.RoundToInt(maxHeight - currentHeight - Constants.ENERGY_DOWNGRADE - 1);
                IncrementEnergy(energyGain);
            }
            else if(!wasInAirBecauseOfMiniJump)
            {
                var energyGain = Mathf.RoundToInt(maxHeight - currentHeight - Constants.ENERGY_DOWNGRADE);
                IncrementEnergy(energyGain);
            }
        }

        wasGrounded = isGrounded;

        // Vertical Movement
        if (isGrounded)
        {
            startingHeight = currentHeight;
            wasInAirBecauseOfMiniJump = false;
            maxHeight = 0;

            if (jump != 0 && _energy != 0)
            {
                // Apply velocity directly, since we want an immediate change.
                // https://docs.unity3d.com/ScriptReference/Rigidbody-velocity.html
                player.velocity = new Vector3(player.velocity.x, CalculateJumpVelocity(_energy, includeClearance: true), player.velocity.z);
                _energy = 0;
            }
            else if (miniJump != 0)
            {
                player.velocity = new Vector3(player.velocity.x, CalculateJumpVelocity(Constants.MINI_JUMP_HEIGHT, includeClearance: true), player.velocity.z);
                wasInAirBecauseOfMiniJump = true;
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
            var multiplier = _movementMultiplier;           

            //var playerdirection = moveVertical * cameraDirection + moveHorizontal * _camera.right;
            var playerdirection = new Vector3(1, 1, 1);

            movement = new Vector3(moveHorizontal * multiplier,
                                   player.velocity.y,
                                   moveVertical * multiplier);

            // Rotate movement so that it faces the same direction as the player.
            movement = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f) * movement;

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
        var isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
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

    /**
     * 
     * Called when the Player is burned by Lava
     * 
     **/ 
    public void Burn()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(0, 0.5F, 0), new Quaternion(0,0,0,0));
    }
}
