using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public GameObject _button;

    private ButtonController buttonController;
    private bool isOpen = false;

    // Use this for initialization
    void Start () {
        buttonController = _button.GetComponent<ButtonController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (buttonController.IsPressed() && !isOpen)
        {
            isOpen = true;

            // Set Graphics
            Color c = GetComponent<Renderer>().material.color;
            c.a = 0.5f;
            GetComponent<Renderer>().material.color = c;

            // Set Collisions
            GetComponent<BoxCollider>().enabled = false;
        }
        else if (!buttonController.IsPressed() && isOpen)
        {
            isOpen = false;

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
        return isOpen;
    }
}
