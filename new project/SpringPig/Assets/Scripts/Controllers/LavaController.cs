using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // Targets with the "Burnable" tag must implement Burn.
        // Currently only works with the Player
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BURNABLE))
        {
            other.GetComponent<PlayerController>().Burn();
        }
    }
}
