using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    protected bool pushed = false;
    protected Rigidbody button;

	// Use this for initialization
	void Start () {
        button = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {

        TagList otherTags = other.gameObject.GetComponent<TagList>();

        if (otherTags != null && otherTags.ContainsTag(Constants.BUTTON_PUSHER)) {
            // TODO: Some better way to show a pushed button
            button.gameObject.SetActive(false);

            pushed = true;
        }
    }

    public bool isPushed()
    {
        return pushed;
    }
}
