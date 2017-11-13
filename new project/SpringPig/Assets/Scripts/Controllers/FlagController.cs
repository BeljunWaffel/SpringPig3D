using UnityEngine;

public class FlagController : MonoBehaviour {

    [SerializeField] public GameState _gameState;
    
    private void OnTriggerEnter(Collider other)
    {
        if (TagList.ContainsTag(other.gameObject, Constants.TAG_PLAYER))
        {
            _gameState.CompleteLevel();
        }
    }
}
