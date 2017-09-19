using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public GameObject button;

    private Rigidbody gate;

    // Use this for initialization
    void Start () {
		gate = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        ButtonController buttonController = button.GetComponent<ButtonController>();

        if (buttonController.activated)
        {
            gate.gameObject.SetActive(false);
        }
	}
}
