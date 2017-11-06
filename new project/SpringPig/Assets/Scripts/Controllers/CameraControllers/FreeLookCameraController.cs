using UnityEngine;

public class FreeLookCameraController : MonoBehaviour
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:
    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [Range(0f, 10f)] [SerializeField] private float _turnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField] private Transform _targetObject;    // The object to follow    
    [SerializeField] private bool _autoTargetPlayer = true;
    [SerializeField] private int _cameraMoveSpeed = 5;
    [SerializeField] private float _tiltMin = 45f;  // The min/max values of the x-axis rotation of the pivot. (Determines how far up/down the camera can look)
    [SerializeField] private float _tiltMax = 75f;
    [SerializeField] private float _zoomSpeed = .5f;
    [SerializeField] private float _zoomMinY = 1;
    [SerializeField] private float _zoomMaxAdditionalY = 3;

    // Pivot
    private Transform _pivot;
    private Quaternion _pivotTargetRot;

    // Camera
    private Transform _camera;
    private float _cameraStartY;

    private float _lookAngle;
    private float _tiltAngle;
    private Vector3 _pivotEulers;
    private Quaternion _transformTargetRot;

    void Start()
    {
        if (_autoTargetPlayer && _targetObject == null)
        {
            FindAndTargetPlayer();
        }

        if (!_targetObject)
        {
            return;
        }

        _pivot = transform.GetChild(0);
        _camera = GetComponentInChildren<Camera>().transform;
        _cameraStartY = _camera.localPosition.y;
        _pivotEulers = _pivot.rotation.eulerAngles;
        _pivotTargetRot = _pivot.transform.localRotation;
        _transformTargetRot = transform.localRotation;
    }

    protected void Update()
    {
        HandleRotationMovement();
        HandleZoom();
    }

    private void FixedUpdate()
    {
        if (_autoTargetPlayer && _targetObject == null)
        {
            FindAndTargetPlayer();
        }

        FollowTarget(Time.deltaTime);
    }

    private void FindAndTargetPlayer()
    {
        _targetObject = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FollowTarget(float deltaTime)
    {
        if (_targetObject == null)
        {
            return;
        }

        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, _targetObject.position, deltaTime * _cameraMoveSpeed);
    }

    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            _lookAngle += x * _turnSpeed;

            // Rotate the rig (the root object) around Y axis only:
            _transformTargetRot = Quaternion.Euler(0f, _lookAngle, 0f);

            _tiltAngle -= y * _turnSpeed;
            _tiltAngle = Mathf.Clamp(_tiltAngle, -_tiltMin, _tiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
            _pivotTargetRot = Quaternion.Euler(_tiltAngle, _pivotEulers.y, _pivotEulers.z);

            _pivot.localRotation = Quaternion.Slerp(_pivot.localRotation, _pivotTargetRot, 10 * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _transformTargetRot, 10 * Time.deltaTime);
        }
    }

    private void HandleZoom()
    {
        if (Time.timeScale < float.Epsilon)
        {
            return;
        }

        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            wheel = wheel * _zoomSpeed;
            bool zoomIn = wheel > 0;

            if ((!zoomIn && _camera.localPosition.y + wheel <= _cameraStartY + _zoomMaxAdditionalY) || (zoomIn && _camera.localPosition.y - wheel >= _zoomMinY))
            {
                // SOH CAH TOA (in this case TOA). To make an increase of 1 in y, we have to use the angle of the camera to determine the proportion that z gets.
                var theta = Mathf.Deg2Rad * _camera.localEulerAngles.x;
                var y = -1f * wheel;
                var tanOfAngle = Mathf.Tan(theta);
                var z = tanOfAngle != 0 ? -1 * y / Mathf.Tan(theta) : 0;

                _camera.localPosition = new Vector3(_camera.localPosition.x, _camera.localPosition.y + y, _camera.localPosition.z + z);
            }
        }
    }
}