using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour
{
    public float _speed = 6.0F;
    public float _gravity = 20.0F;
    public float jumpDuration = 0.5f;
    private int energy = 1;
    private float startingHeight;
    private bool inAir = false;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _rotateDirection = Vector3.zero;

    public int minEnergy = 2;
    public int maxEnergy = 10;

    public int Energy { 
        get
        {
            return energy;
        }

        set 
        {
            energy = value;
        } 
    }

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();

        var rotateSpeed = Input.GetAxis("Horizontal") / 3;
        _rotateDirection = new Vector3(0, rotateSpeed, 0);
        _rotateDirection = transform.TransformDirection(_rotateDirection);
        _rotateDirection *= _speed;

        if (controller.isGrounded)
        {
            // Check for first time hitting ground
            if (inAir)
            {
                var fallHeight = startingHeight - transform.position.y;
                var energyGained = (int) Mathf.Round(fallHeight - 1);
                IncreaseEnergy(energyGained);
                if (Energy < minEnergy)
                {
                    Energy = minEnergy;
                }
            }
            inAir = false;

            _moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= _speed;

            if (Input.GetButton("Jump"))
            {
                _moveDirection.y = CalculateVerticalSpeedToJumpToHeight();
                // Set energy to minEnergy after expending all of it.
                Energy = minEnergy;
            }

            // moving + animations
            if (Input.GetButton("Run"))
            {
                _moveDirection.x *= 2;
                _moveDirection.z *= 2;
                GetComponent<Animation>()["Take 001"].speed = 2;
            }
            else
            {
                GetComponent<Animation>()["Take 001"].speed = 1;
            }

            if (_moveDirection.z == 0)
            {
                GetComponent<Animation>().Stop();
            }
            else if (!GetComponent<Animation>().isPlaying)
            {
                GetComponent<Animation>().Play();
            }
        }
        else // In that air
        {
            // Check for apex to calculate highest point reached by jump
            // When y dir becomes negative, you reached apex.
            //
            if (_moveDirection.y <= 0 && !inAir)
            {
                inAir = true;
                startingHeight = transform.position.y;
            }
        }

        _moveDirection.y -= _gravity * Time.deltaTime;
        controller.Move(_moveDirection * Time.deltaTime);
        controller.transform.Rotate(_rotateDirection);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), string.Format("Energy: {0}", Energy));
    }

    private float CalculateVerticalSpeedToJumpToHeight()
    {
        var jumpHeight = Energy;
        return Mathf.Sqrt(-2f * jumpHeight * Physics2D.gravity.y);
    }

    public void IncreaseEnergy(int increment)
    {
        Energy += increment;
        if (Energy > 10)
        {
            Energy = 10;
        }
    }
}