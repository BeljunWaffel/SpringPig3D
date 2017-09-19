using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public GameObject _button;

    private Rigidbody gate;
    private ButtonController buttonController;

    // Use this for initialization
    void Start () {
		gate = GetComponent<Rigidbody>();

        buttonController = _button.GetComponent<ButtonController>();
    }
	
	// Update is called once per frame
	void Update () {
        gate.gameObject.SetActive(!buttonController.isPushed());
	}
}
