using UnityEngine;

public class ButtonController : MonoBehaviour {

    public bool _toggleable;

    private bool isPressed = false;
    private Rigidbody button;
    private int numberofPressingObjects;

	void Start ()
    {
        button = GetComponent<Rigidbody>();
        numberofPressingObjects = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BUTTON_PUSHER))
        {
            numberofPressingObjects++;
            PushButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BUTTON_PUSHER))
        {
            numberofPressingObjects--;
            UnpushButton();
        }
    }

    public bool IsPressed()
    {
        return isPressed;
    }

    private void PushButton()
    {
        if (!isPressed)
        {
            button.gameObject.transform.localScale -= new Vector3(0, 0.125F, 0);
            isPressed = true;
        }
    }

    private void UnpushButton()
    {
        if (_toggleable && 
            isPressed && 
            numberofPressingObjects == 0)
        {
            button.gameObject.transform.localScale += new Vector3(0, 0.125F, 0);
            isPressed = false;
        }
    }
}
