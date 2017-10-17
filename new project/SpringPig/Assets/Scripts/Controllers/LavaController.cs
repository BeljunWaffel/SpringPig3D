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
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_BURNABLE))
        {
            other.GetComponent<PlayerController>().Kill();
        }
    }
}
