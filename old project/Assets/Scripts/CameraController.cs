using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject _player;

    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - _player.transform.position;
    }

    // LateUpdate is called once per frame, after Update
    void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
    }
}
