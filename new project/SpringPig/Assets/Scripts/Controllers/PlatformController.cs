using System;
using System.Collections.Generic;
using UnityEngine;

class PlatformController : MonoBehaviour
{
    public List<Vector3> _positions;
    public List<int> _secondsToReachTarget;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private int startPositionIndex;
    private int endPositionIndex;
    private Rigidbody platform;
    private float time;

    private float debugTimeStart;

    void Start()
    {
        if (_positions.Count != _secondsToReachTarget.Count)
        {
            throw new ArgumentException("#items in lists should be equal");
        }

        platform = GetComponent<Rigidbody>();

        startPositionIndex = 0;
        endPositionIndex = 1;

        startPosition = _positions[0];
        platform.position = startPosition;

        endPosition = _positions[1];
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime / _secondsToReachTarget[startPositionIndex];
        transform.position = Vector3.Lerp(startPosition, endPosition, time);

        if (platform.position == endPosition)
        {
            Debug.Log("time:" + Mathf.RoundToInt(Time.time - debugTimeStart) + "\nposition:" + transform.position);
            startPositionIndex++;
            endPositionIndex++;

            if (endPositionIndex > _positions.Count - 1)
            {
                endPositionIndex = 0;
            }
            if (startPositionIndex > _positions.Count - 1)
            {
                startPositionIndex = 0;
            }

            startPosition = endPosition;
            endPosition = _positions[endPositionIndex];
            time = 0;
            debugTimeStart = Time.time;
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