using UnityEngine;

public class PickupController : MonoBehaviour {

    [SerializeField] public string PickupEffect;
    [SerializeField] public int Value;
    
    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_PLAYER))
        {
            // Addition - Add the value to the player's current energy
            if (PickupEffect.Equals(Constants.PICKUP_ADDITION))
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.Energy += Value;
                Destroy(this.gameObject);
            }

            // Insert additional effects here
        }
    }
}
