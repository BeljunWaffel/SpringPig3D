using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public string _levelName;

	// Use this for initialization
	void Start () {
        LevelDefinition levelDefinition;
        LevelParser.ParseLevelJson(_levelName, out levelDefinition);

        // Create level based on contents of levelDefinition
	}
}