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
    [SerializeField] private int _cameraMoveSpeed;
    [SerializeField] private float _tiltMin = 45f;  // The min/max values of the x-axis rotation of the pivot. (Determines how far up/down the camera can look)
    [SerializeField] private float _tiltMax = 75f;

    // Pivot
    private Transform _pivot;
    private Quaternion _pivotTargetRot;
    
    private float _lookAngle;
    private float _tiltAngle;
    private Vector3 m_PivotEulers;
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
        m_PivotEulers = _pivot.rotation.eulerAngles;
        _pivotTargetRot = _pivot.transform.localRotation;
        _transformTargetRot = transform.localRotation;
    }

    protected void Update()
    {
        HandleRotationMovement();
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
            _pivotTargetRot = Quaternion.Euler(_tiltAngle, m_PivotEulers.y, m_PivotEulers.z);

            _pivot.localRotation = Quaternion.Slerp(_pivot.localRotation, _pivotTargetRot, 10 * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _transformTargetRot, 10 * Time.deltaTime);
        }
    }
}
