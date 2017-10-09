using UnityEngine;

class PlatformController : MonoBehaviour
{
    public Vector3 _endPosition;
    public int _secondsToReachTarget;

    private Vector3 startPosition;
    private Rigidbody platform;
    private bool isMovingTowardsEnd;
    private float time;

    void Start()
    {
        platform = GetComponent<Rigidbody>();
        startPosition = platform.position;
        isMovingTowardsEnd = true;
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime / _secondsToReachTarget;
        transform.position = Vector3.Lerp(startPosition, _endPosition, time);

        if (platform.position == _endPosition)
        {
            // Switch start and end position and reset time.
            var endPos = _endPosition;
            _endPosition = startPosition;
            startPosition = endPos;
            isMovingTowardsEnd = !isMovingTowardsEnd;
            time = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.transform.parent = transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsCollidingWithPlayer(collision))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.transform.parent = null;
        }
    }

    private bool IsCollidingWithPlayer(Collision collision)
    {
        return TagList.ContainsTag(collision.gameObject, Constants.TAG_PLAYER);
    }
}