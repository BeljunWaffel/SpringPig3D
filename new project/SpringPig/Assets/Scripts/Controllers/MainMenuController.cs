using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Use this for initialization
    public void ChangeScene(bool startAtBeginning)
    {
        if (startAtBeginning)
        {
            InterSceneState.UseGameStateLevelNumber = true;
        }
        else
        {
            InterSceneState.UseGameStateLevelNumber = false;
        }

        SceneManager.LoadScene("PremadeLevelScene");
    }
}