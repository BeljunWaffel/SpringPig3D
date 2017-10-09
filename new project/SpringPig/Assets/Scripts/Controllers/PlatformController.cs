using System;
using System.Collections.Generic;
using UnityEngine;

class PlatformController : MonoBehaviour
{
    public List<Vector3> _positions;
    public List<int> _secondsToReachTarget;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private int endPositionIndex;
    private Rigidbody platform;
    private float time;

    void Start()
    {
        if (_positions.Count != _secondsToReachTarget.Count)
        {
            throw new ArgumentException("#items in lists should be equal");
        }

        platform = GetComponent<Rigidbody>();
        startPosition = _positions[endPositionIndex];
        platform.position = startPosition;
        endPositionIndex = 1;
        endPosition = _positions[endPositionIndex];
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime / _secondsToReachTarget[endPositionIndex];
        transform.position = Vector3.Lerp(startPosition, endPosition, time);

        if (platform.position == endPosition)
        {
            startPosition = endPosition;
            endPositionIndex++;
            if (endPositionIndex > _positions.Count - 1)
            {
                endPositionIndex = 0;
            }
            endPosition = _positions[endPositionIndex];
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