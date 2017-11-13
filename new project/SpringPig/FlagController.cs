using UnityEngine;

public class FlagController : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_PLAYER))
        {
            
        }
    }
}
