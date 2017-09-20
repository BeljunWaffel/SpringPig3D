using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

    public bool toggleable;

    protected bool pushed = false;
    protected Rigidbody button;

	// Use this for initialization
	void Start ()
    {
        button = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {

        TagList otherTags = other.gameObject.GetComponent<TagList>();

        if (!pushed &&
            otherTags != null && 
            otherTags.ContainsTag(Constants.BUTTON_PUSHER))
        {
            PushButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TagList otherTags = other.gameObject.GetComponent<TagList>();

        if (toggleable && 
            pushed && 
            otherTags != null && 
            otherTags.ContainsTag(Constants.BUTTON_PUSHER))
        {
            UnpushButton();
        }
    }

    public bool IsPushed()
    {
        return pushed;
    }

    private void PushButton()
    {
        // TODO: Some better way to show a pushed button
        button.gameObject.transform.localScale -= new Vector3(0, 0.125F, 0);
        //button.gameObject.SetActive(false);
        pushed = true;
    }

    private void UnpushButton()
    {
        button.gameObject.transform.localScale += new Vector3(0, 0.125F, 0);
        pushed = false;
    }
}
