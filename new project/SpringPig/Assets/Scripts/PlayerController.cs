using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float _movementMultiplier;
    public float _energy;

    private Rigidbody player;
    private float distToGround;

    // DEBUG VARIABLES
    public bool IsDebug = false;
    private float maxHeight = -1;
    
    void Start()
    {
        player = GetComponent<Rigidbody>();
        var collider = GetComponent<Collider>();
        distToGround = collider.bounds.extents.y;
    }

    // Applied before physics
    void FixedUpdate()
    {
        var moveHorizortal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var jump = Input.GetAxis("Jump");

        PrintJumpHeight();

        // Horizontal movement
        Vector3 movement;
        movement = new Vector3(moveHorizortal * _movementMultiplier,
                               0,
                               moveVertical * _movementMultiplier);
        player.AddForce(movement);

        // Vertical Movement
        if (IsGrounded() && jump != 0)
        {
            // Apply velocity directly, since we want an immediate change.
            // https://docs.unity3d.com/ScriptReference/Rigidbody-velocity.html
            player.velocity = new Vector3(0f, CalculateJumpVelocity(), 0f);
        }
    }

    private float CalculateJumpVelocity()
    {
        /*
            Potentially useful link if we want to go with the catapult idea https://forum.unity3d.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/
            useful link? https://www.youtube.com/watch?time_continue=171&v=v1V3T5BPd7E 
            v0 = sqrt(v^2 - 2*g*h). Solve for when v = 0 and h = energy, so:
            v0 = sqrt(-2*g*h)
        */
        
        var gravity = -1 * Physics.gravity.magnitude;
        float yVelocity = Mathf.Sqrt(-2 * gravity * _energy);
        return yVelocity;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Pick Up"))
    //    {
    //        other.gameObject.SetActive(false);
    //        Destroy(other.gameObject);
    //    }
    //}

    private bool IsGrounded()
    {
        var isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        return isGrounded;
    }

    // Prints out the max height
    private void PrintJumpHeight()
    {
        if (IsDebug)
        {
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
}
