using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _levelToStartAt = 0;
    [SerializeField] private bool _loadLevel = true;
    [SerializeField] private bool _overrideInterSceneState = false;

    private Dictionary<int, string> levelIdNameMappings;
    private LevelSetup _levelSetup;

    private void Awake()
    {
        _levelSetup = gameObject.GetComponent<LevelSetup>();
    }

    void Start()
    {
        if (!LevelParser.ParseLevelsList("LevelList.json", out levelIdNameMappings))
        {
            Debug.Log("Couldn't load LevelList");
        }

        if (_loadLevel)
        {
            if (InterSceneState.UseGameStateLevelNumber || _overrideInterSceneState)
            {
                InterSceneState.CurrentLevelNumber = _levelToStartAt;
            }
            LoadCurrentLevel();
        }
    }

    private void FixedUpdate()
    {
        var escape = Input.GetAxis("Escape");
        if (escape > 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadCurrentLevel()
    {
        if (levelIdNameMappings.ContainsKey(InterSceneState.CurrentLevelNumber))
        {
            _levelSetup.SetupLevel(levelIdNameMappings[InterSceneState.CurrentLevelNumber]);
        }
        else
        {
            Debug.Log("Level #" + InterSceneState.CurrentLevelNumber + " does not exist in Level List");
        }
    }

    public void CompleteLevel()
    {
        _levelSetup.ResetAndPoolLevel();
        InterSceneState.CurrentLevelNumber++;
        LoadCurrentLevel();
    }
}