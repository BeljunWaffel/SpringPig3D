using UnityEngine;

public class BoxController : MonoBehaviour
{
    private Rigidbody _box;

	void Start ()
    {
        _box = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (IsPlayerAtSameLevelAsBox(player))
            {
                PushBox(player);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (IsPlayerAtSameLevelAsBox(player))
            {
                PushBox(player);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            FreezeBox();
        }
    }

    private bool IsCollidingWithPlayer(Collision collision)
    {
        return TagList.ContainsTag(collision.gameObject, Constants.TAG_PLAYER);
    }

    private bool IsPlayerAtSameLevelAsBox(PlayerController playerObject)
    {
        var player = playerObject.GetComponent<Rigidbody>();
        var positionDiff = player.transform.position - _box.transform.position;
        if (positionDiff.y > 0.5)
        {
            // Return false if the player is on top of the box
            return false;
        }
        else
        {
            return true;
        }
    }

    private void PushBox(PlayerController playerObject)
    {
        var player = playerObject.GetComponent<Rigidbody>();
        if (playerObject.IsMovingHorizontally())
        {
            var positionDiff = player.transform.position - _box.transform.position;
            var collisionDirection = new Vector3(positionDiff.x * -1, 0, positionDiff.z * -1).normalized;

            var isCollisionX = Mathf.Abs(collisionDirection.x) > Mathf.Abs(collisionDirection.z);

            // Verify player input
            var moveHorizontal = Input.GetAxis("Horizontal");
            var moveVertical = Input.GetAxis("Vertical");

            // Set box push speed to 1
            if (isCollisionX && moveHorizontal != 0 && MathUtils.IsSameSign(moveHorizontal, collisionDirection.x))
            {
                _box.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                _box.velocity = new Vector3(player.velocity.x, 0f, 0f);
                playerObject.IsPushingBoxInX = true;
            }
            else if (!isCollisionX && moveVertical != 0 && MathUtils.IsSameSign(moveVertical, collisionDirection.z))
            {
                _box.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                _box.velocity = new Vector3(0f, 0f, player.velocity.z);
                playerObject.IsPushingBoxInZ = true;
            }
            else
            {
                FreezeBox();
            }
        }
        else
        {
            FreezeBox();
        }
    }

    private void FreezeBox()
    {
        _box.constraints = RigidbodyConstraints.FreezeAll;
    }
}
