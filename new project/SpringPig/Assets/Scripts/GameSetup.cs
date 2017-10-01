using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public string _levelName;
    public Transform _gatePrefab;
    public Transform _buttonPrefab;
    public Transform _boxPrefab;
    public GameObject _plane;
    public GameObject _player;

    private int planeWidth;
    private int planeLength;

    private Dictionary<string, int> itemCounts;
    private List<VerticalDefinitions> verticalDefinitionsList;

    private Dictionary<int, GameObject> buttons; // Maps the button number to actual button.
    private Dictionary<int, List<Transform>> gates; // Maps the gate button to the gates opened by that button.

    // Containers
    private GameObject wallsContainer;
    private GameObject interactableObjectsContainer;
    private GameObject nonInteractableObjectsContainer;

    // Use this for initialization
    void Start () {
        if (_levelName == "")
        {
            return;
        }

        itemCounts = new Dictionary<string, int>();

        LevelDefinition levelDefinition;
        LevelParser.ParseLevelJson(_levelName, out levelDefinition);

        // Create level based on contents of levelDefinition
        if (levelDefinition == null)
        {
            throw new ArgumentNullException("levelDefinition is null, cannot parse and create level.");
        }
        else
        {
            planeLength = levelDefinition.Length;
            planeWidth = levelDefinition.Width;

            ScalePlane();
            CreateOuterWalls();
            verticalDefinitionsList = levelDefinition.VerticalDefinitions;
            SetupLevelContents(levelDefinition);
            
            var playerCoordinates = new Vector3(levelDefinition.Player.X, levelDefinition.Player.Y, levelDefinition.Player.Z);
            var playerDimensions = new Vector3(Constants.PLAYER_WIDTH, Constants.PLAYER_HEIGHT, Constants.PLAYER_LENGTH);
            _player.transform.position = TransformUtils.GetLocalPositionFromGridCoordinates(playerCoordinates, playerDimensions);
        }
	}

    private void ScalePlane()
    {
        // Default plane dim is 10x10.
        _plane.transform.localScale = new Vector3(planeLength / 10.0f, 1f, planeWidth / 10.0f);

        // Top left corner is 0,0
        _plane.transform.position = new Vector3(planeWidth / 2.0f, 0f, -planeLength / 2.0f);
    }

    private void CreateOuterWalls()
    {
        wallsContainer = new GameObject("Walls");
        var westWall = CreateWall(planeWidth, "West Wall", wallsContainer);
        var eastWall = CreateWall(planeWidth, "East Wall", wallsContainer);
        var northWall = CreateWall(planeLength, "North Wall", wallsContainer);
        var southWall = CreateWall(planeLength, "South Wall", wallsContainer);

        var widthOffset = Constants.DEFAULT_WALL_WIDTH / 2;

        westWall.transform.position = new Vector3(-1.0f * widthOffset, 0f, -planeLength / 2.0f);
        eastWall.transform.position = new Vector3(planeWidth + widthOffset, 0f, -planeLength / 2.0f);

        northWall.transform.position = new Vector3(planeWidth / 2.0f, 0f, widthOffset);
        northWall.transform.Rotate(new Vector3(0f, 90f, 0f));
        southWall.transform.position = new Vector3(planeWidth / 2.0f, 0f, -1.0f * planeLength - widthOffset);
        southWall.transform.Rotate(new Vector3(0f, 90f, 0f));
    }

    private GameObject CreateWall(float length, string name, GameObject parent)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.AddComponent<BoxCollider>();

        // +1 to give a buffer on both sides, so the walls superpose at the edges
        wall.transform.localScale = new Vector3(Constants.DEFAULT_WALL_WIDTH, 
                                                Constants.DEFAULT_WALL_HEIGHT, 
                                                length + 1f);

        wall.transform.parent = parent.transform;
        wall.name = name;

        return wall;
    }

    private void SetupLevelContents(LevelDefinition levelDefinition)
    {
        // Ensure # of items in levelBase is equal to specified length/width
        if (planeLength * planeWidth != levelDefinition.LevelBase.Count)
        {
            throw new ArgumentNullException("Invalid JSON. LevelBase size does not match length/width arguments.");
        }

        // Set up containers
        interactableObjectsContainer = new GameObject("InteractableObjects");
        nonInteractableObjectsContainer = new GameObject("NonInteractableObjects");

        for (int i = 0; i < levelDefinition.LevelBase.Count; i++)
        {
            var item = levelDefinition.LevelBase[i];
            if (item == "0")
            {
                continue;
            }

            var row = i / planeWidth;
            var col = i % planeLength;

            VerticalDefinition itemVerticalDefinition = new VerticalDefinition()
            {
                Id = item,
                StartHeight = 0
            };

            ParseStringsAndCreateGameObjects(itemVerticalDefinition, col, row, lookAtVerticalDefinitions: true);
        }
    }

    private void ParseStringsAndCreateGameObjects(VerticalDefinition itemVerticalDefinition, int col, int row, bool lookAtVerticalDefinitions)
    {
        var id = itemVerticalDefinition.Id;
        var startHeight = itemVerticalDefinition.StartHeight;

        if (id.StartsWith(Constants.CUBE_PREFIX))
        {
            var height = Convert.ToInt32(id.Substring(Constants.CUBE_PREFIX.Length));
            CreateCube(height, col, row, startHeight);
        }
        else if (id.StartsWith(Constants.GATE_PREFIX))
        {
            InstantiateGatesAndButtonDictionaries();
            var periodIndex = id.IndexOf('.', Constants.GATE_PREFIX.Length);
            var height = Convert.ToInt32(id.Substring(Constants.GATE_PREFIX.Length, periodIndex - Constants.GATE_PREFIX.Length));
            var buttonNumber = Convert.ToInt32(id.Substring(periodIndex + 1));

            CreateGate(height, col, row, startHeight, buttonNumber);
        }
        else if (id.StartsWith(Constants.NO_TOGGLE_BUTTON_PREFIX))
        {
            InstantiateGatesAndButtonDictionaries();
            var buttonNumber = Convert.ToInt32(id.Substring(Constants.NO_TOGGLE_BUTTON_PREFIX.Length));
            CreateButton(col, row, startHeight, buttonNumber, isToggle: false);
        }
        else if (id.StartsWith(Constants.TOGGLE_BUTTON_PREFIX))
        {
            InstantiateGatesAndButtonDictionaries();
            var buttonNumber = Convert.ToInt32(id.Substring(Constants.TOGGLE_BUTTON_PREFIX.Length));
            CreateButton(col, row, startHeight, buttonNumber, isToggle: true);
        }
        else if (lookAtVerticalDefinitions)
        {
            var verticalDefinitionList = GetVerticalDefinitionList(id);
            if (verticalDefinitionList == null)
            {
                Debug.Log("Could not find height definition for " + id);
            }
            else
            {
                foreach (var verticalDefinition in verticalDefinitionList)
                {
                    // Doesn't allow nesting of vertical definitions within vertical definitions.
                    ParseStringsAndCreateGameObjects(verticalDefinition, col, row, lookAtVerticalDefinitions: false);
                }
            }
        }
    }

    private List<VerticalDefinition> GetVerticalDefinitionList(string id)
    {
        foreach (var verticalDefinitions in verticalDefinitionsList)
        {
            if (id == verticalDefinitions.Id)
            {
                return verticalDefinitions.VerticalDefinition;
            };
        }

        return null;
    }

    private GameObject CreateCube(float height, int col, int row, int startHeight)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoxCollider>();
        
        cube.transform.localScale = new Vector3(1f, height, 1f);

        var cubeCoordinates = new Vector3(col, startHeight, row);
        var cubeDimensions = new Vector3(1f, height, 1f);
        cube.transform.position = TransformUtils.GetLocalPositionFromGridCoordinates(cubeCoordinates, cubeDimensions);

        cube.transform.parent = nonInteractableObjectsContainer.transform;
        cube.name = CreateUniqueItemName("Cube_" + height);

        return cube;
    }

    private Transform CreateGate(float height, int col, int row, int startHeight, int buttonNumber)
    {
        var gate = Instantiate(_gatePrefab, interactableObjectsContainer.transform);

        gate.localScale = new Vector3(1f, height, 1f);

        var gateCoordinates = new Vector3(col, startHeight, row);
        var gateDimensions = new Vector3(1f, height, 1f);
        gate.position = TransformUtils.GetLocalPositionFromGridCoordinates(gateCoordinates, gateDimensions);
        gate.name = CreateUniqueItemName("Gate_H" + height + "_" + buttonNumber);

        // If button has not been created yet, create an empty GameObject that will be replaced later.
        GameObject button;
        if (!buttons.TryGetValue(buttonNumber, out button)) {
            button = new GameObject();
            buttons[buttonNumber] = button;
        }

        gate.GetComponent<GateController>()._button = button;

        List<Transform> gatesList;
        if (!gates.TryGetValue(buttonNumber, out gatesList))
        {
            gates[buttonNumber] = new List<Transform>
            {
                gate
            };
        }
        else
        {
            gates[buttonNumber].Add(gate);
        }

        return gate;
    }

    // A button opens a gate.
    private Transform CreateButton(int col, int row, int startHeight, int buttonNumber, bool isToggle)
    {
        var button = Instantiate(_buttonPrefab, interactableObjectsContainer.transform);

        button.localScale = new Vector3(.5f, .25f, .5f);
        button.position = new Vector3(0.5f + col, .25f / 2.0f + startHeight, -.5f - row);
        button.name = CreateUniqueItemName("Button_" + buttonNumber);
        
        // If gates already exist for this button, make sure to assign this button to them.
        List<Transform> gatesList;
        if (gates.TryGetValue(buttonNumber, out gatesList)) {
            foreach (var gate in gatesList)
            {
                gate.GetComponent<GateController>()._button = button.gameObject;
            }
        }

        button.GetComponent<ButtonController>()._toggleable = isToggle;

        buttons[buttonNumber] = button.gameObject;

        return button;
    }

    private Transform CreateBox(int col, int row, int startHeight, int boxNumber)
    {
        var box = Instantiate(_boxPrefab, interactableObjectsContainer.transform);

        box.localScale = new Vector3(1f, 1f, 1f);
        // TODO: box position
        box.name = CreateUniqueItemName("Box_" + boxNumber);

        return box;
        
    }

    private string CreateUniqueItemName(string key)
    {
        if (!itemCounts.ContainsKey(key))
        {
            itemCounts[key] = 0;
        }
        itemCounts[key]++;
        int itemNumber = itemCounts[key];
        var result = key + " (" + itemNumber + ")";

        return result;
    }

    private void InstantiateGatesAndButtonDictionaries()
    {
        if (buttons == null)
        {
            buttons = new Dictionary<int, GameObject>();
        }

        if (gates == null)
        {
            gates = new Dictionary<int, List<Transform>>();
        }
    }
}