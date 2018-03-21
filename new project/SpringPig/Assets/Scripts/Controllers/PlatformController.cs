using System;
using System.Collections.Generic;
using UnityEngine;

class PlatformController : MonoBehaviour
{
    /// <summary>
    /// Describes a moving platform. If a platform has 0 or 1 positions, it will stand still. Otherwise it will go from one point to another and loop.
    /// </summary>

    [SerializeField] public List<Vector3> Positions;
    [SerializeField] public List<int> SecondsToReachTarget;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private int _startPositionIndex;
    private int _endPositionIndex;
    private Rigidbody _platform;
    private float _time;

    public bool IsSet { get; set; } = false;

    void Start()
    {
        SetupPlatform();
    }

    public void SetupPlatform()
    {
        if (Positions.Count != SecondsToReachTarget.Count)
        {
            throw new ArgumentException("#items in lists should be equal");
        }

        if (Positions.Count <= 1)
        {
            return;
        }

        _platform = GetComponent<Rigidbody>();

        _startPositionIndex = 0;
        _endPositionIndex = 1;

        _startPosition = Positions[0];
        _platform.position = _startPosition;
        _endPosition = Positions.Count > 1 ? Positions[1] : _startPosition;

        _time = 0;
        IsSet = true;
    }

    private void Update()
    {
        if (!IsSet)
        {
            SetupPlatform();
        }

        if (Positions.Count > 1)
        {
            _time += Time.deltaTime / SecondsToReachTarget[_startPositionIndex];
            transform.position = Vector3.Lerp(_startPosition, _endPosition, _time);

            if (_platform.position == _endPosition)
            {
                _startPositionIndex++;
                _endPositionIndex++;

                if (_endPositionIndex > Positions.Count - 1)
                {
                    _endPositionIndex = 0;
                }
                if (_startPositionIndex > Positions.Count - 1)
                {
                    _startPositionIndex = 0;
                }

                _startPosition = _endPosition;
                _endPosition = Positions[_endPositionIndex];
                _time = 0;
            }
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