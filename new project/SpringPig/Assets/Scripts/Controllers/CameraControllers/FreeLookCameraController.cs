using System.Collections;
using UnityEngine;

public class FreeLookCameraController : MonoBehaviour
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:
    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [Range(0f, 10f)] [SerializeField] private float _turnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [Range(0f, 1f)] [SerializeField] private float _panSpeed = .5f;   // How fast the rig will rotate from user input.
    [SerializeField] private float doubleClickDelta = 0.35f;
    [SerializeField] private float timeToWaitAfterReset = 1f;
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
    private Quaternion _pivotStartingRotation;
    private Vector3 _pivotStartingLocalPosition;
    private Vector3 _pivotEulers;

    // Camera
    private Transform _camera;
    private Vector3 _cameraStartingLocalPosition;

    // DoubleClick
    private bool _doubleClicked = false;
    private bool _hasClickedOnce = false;
    private float _firstClickTime;
    private float _doubleClickTime;

    // Transform
    private float _lookAngle;
    private float _tiltAngle;
    private Quaternion _transformStartRotation;

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

        // Pivot
        _pivot = transform.GetChild(0);
        _pivotStartingLocalPosition = _pivot.localPosition;
        _pivotStartingRotation = _pivot.localRotation;
        _pivotEulers = _pivot.rotation.eulerAngles;

        // Camera
        _camera = GetComponentInChildren<Camera>().transform;
        _cameraStartingLocalPosition = _camera.localPosition;

        // Transform
        _transformStartRotation = transform.localRotation;
    }

    protected void Update()
    {
        if (!_doubleClicked)
        {
            _doubleClicked = HandleDoubleClick();
            if (_doubleClicked)
            {
                _doubleClickTime = Time.time;
            }
            else
            {
                HandleRotationMovement();
                HandleZoom();
                HandlePan();
            }
        }
        else
        {
            if (Time.time > _doubleClickTime + timeToWaitAfterReset)
            {
                _doubleClicked = false;
                _doubleClickTime = 0;
            }
        }
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
            var transformTargetRot = Quaternion.Euler(0f, _lookAngle, 0f);

            _tiltAngle -= y * _turnSpeed;
            _tiltAngle = Mathf.Clamp(_tiltAngle, -_tiltMin, _tiltMax);

            // Tilt input around X is applied to the pivot (the child of this object)
            var pivotTargetRot = Quaternion.Euler(_tiltAngle, _pivotEulers.y, _pivotEulers.z);

            _pivot.localRotation = Quaternion.Slerp(_pivot.localRotation, pivotTargetRot, 10 * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, 10 * Time.deltaTime);
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

            if ((!zoomIn && _camera.localPosition.y + wheel <= _cameraStartingLocalPosition.y + _zoomMaxAdditionalY) || (zoomIn && _camera.localPosition.y - wheel >= _zoomMinY))
            {
                // SOH CAH TOA (in this case TOA). To make an increase of 1 in y, we have to use the angle of the camera to determine the proportion that z gets.
                var theta = Mathf.Deg2Rad * _camera.localEulerAngles.x;
                var y = -1f * wheel;
                var tanOfAngle = Mathf.Tan(theta);
                var z = tanOfAngle != 0 ? -1 * y / Mathf.Tan(theta) : 0;

                var cameraEndPos = new Vector3(_camera.localPosition.x, _camera.localPosition.y + y, _camera.localPosition.z + z);

                // Makes the zooming transition a bit smoother by lerping the positions.
                StartCoroutine(MoveLocalPositionTo(_camera, cameraEndPos, .075f));
            }
        }
    }

    private void HandlePan()
    {
        if (Time.timeScale < float.Epsilon)
        {
            return;
        }

        if (Input.GetMouseButton(1))
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            var panX = x * _panSpeed;
            var panY = y * _panSpeed;

            _pivot.localPosition = new Vector3(_pivot.localPosition.x - panX, _pivot.localPosition.y - panY, _pivot.localPosition.z);
        }
    }
    
    // Resets camera to original position
    private bool HandleDoubleClick()
    {
        if (Time.time > _firstClickTime + doubleClickDelta)
        {
            _hasClickedOnce = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_hasClickedOnce && Time.time <= _firstClickTime + doubleClickDelta)
            {
                ResetRig();
                _hasClickedOnce = false;
                return true;
            }
            else
            {
                _hasClickedOnce = true;
                _firstClickTime = Time.time;
            }
        }

        return false;
    }

    public IEnumerator MoveLocalPositionTo(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.localPosition;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.localPosition = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    private void ResetRig()
    {
        _pivot.localPosition = _pivotStartingLocalPosition;
        _pivot.localRotation = _pivotStartingRotation;
        _camera.localPosition = _cameraStartingLocalPosition;
        transform.localRotation = _transformStartRotation;
        _lookAngle = 0;
        _tiltAngle = 0;
    }
}