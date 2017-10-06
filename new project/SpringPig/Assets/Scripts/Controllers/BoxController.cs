using UnityEngine;

public class BoxController : MonoBehaviour
{
    private Rigidbody box;

	void Start ()
    {
        box = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            PushBox(player);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            PushBox(player);
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
        var gameObject = collision.gameObject;
        TagList collisionTags = gameObject.GetComponent<TagList>();
        if (collisionTags != null && collisionTags.ContainsTag(Constants.PLAYER))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PushBox(PlayerController playerObject)
    {
        var player = playerObject.GetComponent<Rigidbody>();
        if (playerObject.IsMovingHorizontally())
        {
            var positionDiff = player.transform.position - box.transform.position;
            var collisionDirection = new Vector3(positionDiff.x * -1, 0, positionDiff.z * -1).normalized;

            var isCollisionX = Mathf.Abs(collisionDirection.x) > Mathf.Abs(collisionDirection.z);

            // Verify player input
            var moveHorizontal = Input.GetAxis("Horizontal");
            var moveVertical = Input.GetAxis("Vertical");

            // Set box push speed to 1
            if (isCollisionX && moveHorizontal != 0 && MathUtils.IsSameSign(moveHorizontal, collisionDirection.x))
            {
                box.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                box.velocity = new Vector3(player.velocity.x, 0f, 0f);
                playerObject.IsPushingBoxInX = true;
            }
            else if (!isCollisionX && moveVertical != 0 && MathUtils.IsSameSign(moveVertical, collisionDirection.z))
            {
                box.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
                box.velocity = new Vector3(0f, 0f, player.velocity.z);
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
        box.constraints = RigidbodyConstraints.FreezeAll;
    }

    private float GetBoxSpeed(float playerVelocity)
    {
        // If player velocity is < Box speed, return player velocity.
        // Otherwise return boxspeed in the direction the player was moving.
        return (Mathf.Abs(playerVelocity) < Constants.BOX_SPEED) ?
               playerVelocity :
               MathUtils.IsPositive(playerVelocity) * Constants.BOX_SPEED;
    }
}
