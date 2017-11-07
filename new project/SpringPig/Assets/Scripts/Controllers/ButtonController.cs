using UnityEngine;

public class ButtonController : MonoBehaviour {

    [SerializeField] public bool Toggleable;

    private bool _isPressed = false;
    private Rigidbody _button;
    private int _numberofPressingObjects;

	void Start ()
    {
        _button = GetComponent<Rigidbody>();
        _numberofPressingObjects = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BUTTON_PUSHER))
        {
            _numberofPressingObjects++;
            PushButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BUTTON_PUSHER))
        {
            _numberofPressingObjects--;
            UnpushButton();
        }
    }

    public bool IsPressed()
    {
        return _isPressed;
    }

    private void PushButton()
    {
        if (!_isPressed)
        {
            _button.gameObject.transform.localScale -= new Vector3(0, 0.125F, 0);
            _isPressed = true;
        }
    }

    private void UnpushButton()
    {
        if (Toggleable && 
            _isPressed && 
            _numberofPressingObjects == 0)
        {
            _button.gameObject.transform.localScale += new Vector3(0, 0.125F, 0);
            _isPressed = false;
        }
    }
}
