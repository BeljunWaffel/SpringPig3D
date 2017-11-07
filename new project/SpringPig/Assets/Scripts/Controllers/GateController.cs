using UnityEngine;

public class GateController : MonoBehaviour {

    [SerializeField] public GameObject Button;

    private ButtonController _buttonController;
    private bool _isOpen = false;

    // Use this for initialization
    void Start () {
        _buttonController = Button.GetComponent<ButtonController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (_buttonController.IsPressed() && !_isOpen)
        {
            _isOpen = true;

            // Set Graphics
            Color c = GetComponent<Renderer>().material.color;
            c.a = 0.5f;
            GetComponent<Renderer>().material.color = c;

            // Set Collisions
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!_buttonController.IsPressed() && _isOpen)
        {
            _isOpen = false;

            // Set Graphics
            Color c = GetComponent<Renderer>().material.color;
            c.a = 1;
            GetComponent<Renderer>().material.color = c;

            // Set Collisions
            GetComponent<BoxCollider>().enabled = true;
        }
    }

    public bool IsOpen()
    {
        return _isOpen;
    }
}
