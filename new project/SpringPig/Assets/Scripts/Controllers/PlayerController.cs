using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementMultiplier;
    [SerializeField] private int _fallingGravityMultiplier;
    [SerializeField] private int _jumpingGravityMultiplier;
    [SerializeField] public int Energy;
    [SerializeField] public Transform CameraRig;
    [SerializeField] public Transform Projectile;

    private Rigidbody _player;
    private float _distToGround;
    private bool _isMovingHorizontally = false;
    
    // Needed to calculate energy
    private bool _wasGrounded = false;
    private bool _wasInAirBecauseOfMiniJump = false;
    private bool _wasInAirBecauseOfJump = false;
    private int _maxHeight = -1;
    private float _startingHeight;

    private DateTime _lastFireTime;
    
    void Start()
    { 
        _player = GetComponent<Rigidbody>();

        // Get collider to calculate distance to ground for IsGrounded() function.
        var collider = GetComponent<Collider>();
        _distToGround = collider.bounds.extents.y;

        _wasGrounded = IsGrounded();
    }

    // Applied before physics
    void FixedUpdate()
    {
        PerformHorizontalMovement();
        PerformVerticalMovement();
        FireProjectile();
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

    public bool IsPushingBoxInX { get; set; } = false;
    public bool IsPushingBoxInZ { get; set; } = false;
    
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
    public void FireProjectile()
    {
        var fire = Input.GetAxis("Fire");
        if (fire != 0 && DateTime.Now - _lastFireTime > TimeSpan.FromMilliseconds(100))
        {
            _lastFireTime = DateTime.Now;
            var projectile = Instantiate(Projectile);

            // Determine where the projective starts relative to the player
            projectile.transform.SetPositionAndRotation(transform.position + transform.forward * .75f, Quaternion.identity);

            var projectileController = projectile.GetComponent<ProjectileController>();

            // If you find an enemy, lob a projectile at it. Else shoot straight forward
            var enemy = FindObjectOfType<EnemyController>();
            if (enemy)
            {
                var secondsToEnemy = 2;
                var enemyExpectedPosition = enemy.GetExpectedPositionInMilliSeconds(secondsToEnemy * 1000);
                var directionToEnemy = enemyExpectedPosition - projectile.transform.position;
                var distanceToEnemy = Mathf.Sqrt(Mathf.Pow((enemy.transform.position.x - projectile.transform.position.x), 2) + Mathf.Pow((enemy.transform.position.z - projectile.transform.position.z), 2));
                
                projectile.GetComponent<Rigidbody>().mass = 5;
                projectile.GetComponent<Rigidbody>().useGravity = true;
                projectile.GetComponent<Rigidbody>().velocity = new Vector3(directionToEnemy.x / secondsToEnemy, -1 * Physics.gravity.y, directionToEnemy.z / secondsToEnemy);
            }
            else
            {
                projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileController.ProjectileSpeed;
            }   

            // Ensure projectile does not collide with player
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), _player.GetComponent<Collider>());
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), enemy.GetComponent<Collider>());
        }
    }

    private void PerformVerticalMovement()
    {
        var jump = Input.GetAxis("Jump");
        var miniJump = Input.GetAxis("MiniJump");
        var isGrounded = IsGrounded();

        // If in the air, calculate max height to determine amount of energy to gain.
        int currentHeight = Mathf.RoundToInt(_player.position.y - _player.transform.localScale.y);
        if (!isGrounded)
        {
            if (currentHeight > _maxHeight)
            {
                _maxHeight = currentHeight;
            }

            // If falling, make gravity stronger and fall 2x faster
            if (_player.velocity.y < 0)
            {
                _player.velocity += Vector3.up * Physics.gravity.y * (_fallingGravityMultiplier - 1) * Time.deltaTime;
            }
            else if (_player.velocity.y > 0) // If player is jumping, make gravity a bit stronger too
            {
                _player.velocity += Vector3.up * Physics.gravity.y * (_jumpingGravityMultiplier - 1) * Time.deltaTime;
            }
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
            else if (!_wasInAirBecauseOfJump && !_wasInAirBecauseOfMiniJump && _maxHeight == currentHeight)
            {
                // Don't lose energy in this case. This occurs if the player is not falling off the ledge of something and the grounds himself again by moving off the ledge.
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
            _wasInAirBecauseOfJump = false;
            _maxHeight = 0;

            if (jump != 0 && Energy != 0)
            {
                _player.velocity = new Vector3(_player.velocity.x, CalculateJumpVelocity(Energy, includeClearance: true), _player.velocity.z);
                _wasInAirBecauseOfJump = true;
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
            movement = Quaternion.Euler(0f, CameraRig.rotation.eulerAngles.y, 0f) * movement;

            // Rotate player to face the direction of movement
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(movement.x, 0f, movement.z), 10 * Time.deltaTime);

            if (IsPushingBoxInX && movement.x != 0)
            {
                movement.x = GetPlayerPushingBoxSpeed(movement.x);
            }

            if (IsPushingBoxInZ && movement.z != 0)
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
        float yVelocity = Mathf.Sqrt(-2 * gravity * _jumpingGravityMultiplier * height);
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
            IsPushingBoxInX = false;
            IsPushingBoxInZ = false;
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
