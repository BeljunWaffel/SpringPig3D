using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private bool _loadLevel = true;

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
        _levelSetup.ResetAndPoolLevel();
        _currentLevel++;
        LoadCurrentLevel();
    }
}