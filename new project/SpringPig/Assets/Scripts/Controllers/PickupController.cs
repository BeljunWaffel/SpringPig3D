using UnityEngine;

public class PickupController : MonoBehaviour {

    [SerializeField] private string _pickupEffect;
    [SerializeField] private int _value;
    
    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_PLAYER))
        {
            // Addition - Add the value to the player's current energy
            if (_pickupEffect.Equals(Constants.PICKUP_ADDITION))
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.Energy += _value;
                Destroy(this.gameObject);
            }

            // Insert additional effects here
        }
    }
}
