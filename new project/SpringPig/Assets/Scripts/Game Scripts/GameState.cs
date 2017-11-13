using System.Collections.Generic;
using UnityEngine;

class GameState : MonoBehaviour
{
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private bool _loadLevel = true;

    private Dictionary<int, string> levelIdNameMappings;
    private LevelSetup _levelSetup;

    void Start()
    {
        _levelSetup = gameObject.GetComponent<LevelSetup>();
        if (!LevelParser.ParseLevelsList("LevelList.json", out levelIdNameMappings))
        {
            Debug.Log("Couldn't load LevelList");
        }

        if (_loadLevel)
        {
            LoadCurrentLevel();
        }
    }

    public void LoadCurrentLevel()
    {
        if (levelIdNameMappings.ContainsKey(_currentLevel))
        {
            _levelSetup.SetupLevel(levelIdNameMappings[_currentLevel]);
        }
        else
        {
            Debug.Log("Level #" + _currentLevel + " does not exist in Level List");
        }
    }

    public void CompleteLevel()
    {

    }
}