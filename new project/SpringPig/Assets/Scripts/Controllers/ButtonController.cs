using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    public bool activated = false;
    private Rigidbody button;

	// Use this for initialization
	void Start () {

        button = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // TODO: Some better way to show a pushed button
            button.gameObject.SetActive(false);

            activated = true;
        }
    }
}
