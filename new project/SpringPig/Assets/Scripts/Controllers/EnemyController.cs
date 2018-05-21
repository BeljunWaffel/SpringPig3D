using UnityEngine;

class EnemyController : MonoBehaviour
{
    [SerializeField] public Vector3 StartPosition;
    [SerializeField] public Vector3 EndPosition;
    [SerializeField] public float TimeToReachNextPos;
    
    private Rigidbody _enemy;

    void Start()
    {
        _enemy = GetComponent<Rigidbody>();
        _enemy.position = StartPosition;
        _enemy.velocity = (EndPosition - StartPosition) / TimeToReachNextPos;
    }
    
    private void Update()
    {
        if (_enemy.position == EndPosition)
        {
            var temp = StartPosition;
            StartPosition = EndPosition;
            EndPosition = temp;
            
            _enemy.velocity = (EndPosition - StartPosition) / TimeToReachNextPos;
        }
    }

    public Vector3 GetExpectedPositionInMilliSeconds(int numMs)
    {
        var msToReachEndPos = 0f;
        if (_enemy.velocity.x != 0)
        {
            msToReachEndPos = ((EndPosition.x - _enemy.position.x) / _enemy.velocity.x) * 1000;
        }

        if (_enemy.velocity.z != 0)
        {
            var timeToReachEndPosZ = ((EndPosition.z - _enemy.position.z) / _enemy.velocity.z) * 1000;
            if (timeToReachEndPosZ > msToReachEndPos)
            {
                msToReachEndPos = timeToReachEndPosZ;
            }
        }

        // If the enemy will not reach EndDistance and turn around, then expected position is pos + velocity * time
        Vector3 expectedPos;
        if (msToReachEndPos >= numMs)
        {
            expectedPos = _enemy.position + _enemy.velocity * (numMs / 1000.0f);
        }
        else // Otherwise it turns around. Calculate how far it gets once it turns around.
        {
            var numMsAfterReachingEnd = numMs - msToReachEndPos;

            // Start at the end, and see where you get from there
            var startPos = EndPosition;
            var endPos = StartPosition;

            while (numMsAfterReachingEnd > TimeToReachNextPos * 1000)
            {
                numMsAfterReachingEnd -= (TimeToReachNextPos * 1000);
                var temp = startPos;
                startPos = endPos;
                endPos = temp;
            }

            var newVelocity = (endPos - startPos) / TimeToReachNextPos;
            expectedPos = startPos + newVelocity * (numMsAfterReachingEnd / 1000f);
        }

        return expectedPos;
    }
}