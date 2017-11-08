using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementMultiplier;
    [SerializeField] public int Energy;
    [SerializeField] private int _gravityMagnitude;
    [SerializeField] private Transform _cameraRig;

    private Rigidbody _player;
    private float _distToGround;
    private bool _isMovingHorizontally = false;
    
    // Needed to calculate energy
    private bool _wasGrounded = false;
    private bool _wasInAirBecauseOfMiniJump = false;
    private int _maxHeight = -1;
    private float _startingHeight;
    
    // Box interactions
    private bool _isPushingBoxInX = false;
    private bool _isPushingBoxInZ = false;
    
    void Start()
    {
        _player = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, -1f * _gravityMagnitude, 0f);

        // Get collider to calculate distance to ground for IsGrounded() function.
        var collider = GetComponent<Collider>();
        _distToGround = collider.bounds.extents.y;

        _wasGrounded = IsGrounded();
    }

    // Applied before physics
    void FixedUpdate()
    {
        //transform.forward = _cameraRig.forward;

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
        return _isMovingHorizontally;
    }

    public bool IsPushingBoxInX
    {
        get
        {
            return _isPushingBoxInX;
        }

        set
        {
            _isPushingBoxInX = value;
        }
    }

    public bool IsPushingBoxInZ
    {
        get
        {
            return _isPushingBoxInZ;
        }

        set
        {
            _isPushingBoxInZ = value;
        }
    }
    
    /**
     * 
     * DISPLAY
     * 
     */
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("Energy: {0}", Energy));
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
        int currentHeight = Mathf.RoundToInt(_player.position.y - _player.transform.localScale.y);
        if (!IsGrounded() && currentHeight > _maxHeight)
        {
            _maxHeight = currentHeight;
        }

        // If player was in the air before but isn't anymore, calculate how much energy was gained.
        if (!_wasGrounded && isGrounded)
        {
            if (_wasInAirBecauseOfMiniJump && currentHeight < _startingHeight)
            {
                // If you minijump to get to a lower spot, lose 1 extra energy because you were able to jump by 1.
                var energyGain = Mathf.RoundToInt(_maxHeight - currentHeight - Constants.ENERGY_DOWNGRADE - 1);
                IncrementEnergy(energyGain);
            }
            else if(!_wasInAirBecauseOfMiniJump)
            {
                var energyGain = Mathf.RoundToInt(_maxHeight - currentHeight - Constants.ENERGY_DOWNGRADE);
                IncrementEnergy(energyGain);
            }
        }

        _wasGrounded = isGrounded;

        // Vertical Movement
        if (isGrounded)
        {
            _startingHeight = currentHeight;
            _wasInAirBecauseOfMiniJump = false;
            _maxHeight = 0;

            if (jump != 0 && Energy != 0)
            {
                // Apply velocity directly, since we want an immediate change.
                // https://docs.unity3d.com/ScriptReference/Rigidbody-velocity.html
                _player.velocity = new Vector3(_player.velocity.x, CalculateJumpVelocity(Energy, includeClearance: true), _player.velocity.z);
                Energy = 0;
            }
            else if (miniJump != 0)
            {
                _player.velocity = new Vector3(_player.velocity.x, CalculateJumpVelocity(Constants.MINI_JUMP_HEIGHT, includeClearance: true), _player.velocity.z);
                _wasInAirBecauseOfMiniJump = true;
            }
        }
    }

    private void PerformHorizontalMovement()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            _player.velocity = new Vector3(0f, _player.velocity.y, 0f);
            _isMovingHorizontally = false;
        }
        else
        {
            // Horizontal movement
            Vector3 movement;
            var multiplier = _movementMultiplier;           
            
            movement = new Vector3(moveHorizontal * multiplier,
                                   _player.velocity.y,
                                   moveVertical * multiplier);

            // Rotate movement so that it faces the same direction as the camera.
            movement = Quaternion.Euler(0f, _cameraRig.rotation.eulerAngles.y, 0f) * movement;

            // Rotate player to face the direction of movement
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(movement.x, 0f, movement.z), 10 * Time.deltaTime);

            if (_isPushingBoxInX && movement.x != 0)
            {
                movement.x = GetPlayerPushingBoxSpeed(movement.x);
            }

            if (_isPushingBoxInZ && movement.z != 0)
            {
                movement.z = GetPlayerPushingBoxSpeed(movement.z);
            }

            _player.velocity = movement;
            _isMovingHorizontally = true;
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
        var isGrounded = Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.1f);
        return isGrounded;
    }

    private void IncrementEnergy(int energy)
    {
        Energy += energy;
        if (Energy > Constants.MAX_ENERGY)
        {
            Energy = Constants.MAX_ENERGY;
        }
        if (Energy < Constants.MIN_ENERGY)
        {
            Energy = Constants.MIN_ENERGY;
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
            _isPushingBoxInX = false;
            _isPushingBoxInZ = false;
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
