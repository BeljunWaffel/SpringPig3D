using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {

    public string _pickupEffect;
    public int _value;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        TagList otherTags = other.gameObject.GetComponent<TagList>();

        if (otherTags != null &&
            otherTags.ContainsTag(Constants.TAG_PLAYER))
        {
            // Addition - Add the value to the player's current energy
            if (_pickupEffect.Equals(Constants.PICKUP_ADDITION))
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                playerController._energy+= _value;
                Destroy(this.gameObject);
            }

            // Insert additional effects here
        }
    }
}
