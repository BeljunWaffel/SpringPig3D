using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private string _levelName;
    [SerializeField] private Transform _gatePrefab;
    [SerializeField] private Transform _buttonPrefab;
    [SerializeField] private Transform _boxPrefab;
    [SerializeField] private Transform _platformPrefab;
    [SerializeField] private GameObject _plane;
    [SerializeField] private GameObject _player;
    [SerializeField] private PhysicMaterial noFrictionMaterial;

    private int _planeZScale;
    private int _planeXScale;

    private Dictionary<string, int> _itemCounts;
    private List<VerticalDefinitions> _verticalDefinitionsList;

    private Dictionary<int, GameObject> _buttons; // Maps the button number to actual button.
    private Dictionary<int, List<Transform>> _gates; // Maps the gate button to the gates opened by that button.
    private Dictionary<int, Transform> _platforms; // Dictionary of platforms on this level. Index = platform #.
    private Dictionary<int, List<Vector3>> _platformPositions; // Mapping of platforms to their positions.
    private Dictionary<int, List<int>> _platformMoveTimes; // Mapping of platforms to their move times.

    // Containers
    private GameObject _wallsContainer;
    private GameObject _interactableObjectsContainer;
    private GameObject _nonInteractableObjectsContainer;

    private Dictionary<int, GameObject> Buttons
    {
        get
        {
            if (_buttons == null)
            {
                _buttons = new Dictionary<int, GameObject>();
            }

            return _buttons;
        }

        set
        {
            Buttons = value;
        }
    }

    private Dictionary<int, List<Transform>> Gates
    {
        get
        {
            if (_gates == null)
            {
                _gates = new Dictionary<int, List<Transform>>();
            }

            return _gates;
        }

        set
        {
            Gates = value;
        }
    }

    private Dictionary<int, Transform> Platforms
    {
        get
        {
            if (_platforms == null)
            {
                _platforms = new Dictionary<int, Transform>();
            }

            return _platforms;
        }

        set
        {
            Platforms = value;
        }
    }

    private Dictionary<int, List<Vector3>> PlatformPositions
    {
        get
        {
            if (_platformPositions == null)
            {
                _platformPositions = new Dictionary<int, List<Vector3>>();
            }

            return _platformPositions;
        }

        set
        {
            _platformPositions = value;
        }
    }

    private Dictionary<int, List<int>> PlatformMoveTimes
    {
        get
        {
            if (_platformMoveTimes == null)
            {
                _platformMoveTimes = new Dictionary<int, List<int>>();
            }

            return _platformMoveTimes;
        }

        set
        {
            _platformMoveTimes = value;
        }
    }

    // Use this for initialization
    void Start () {
        if (_levelName == "")
        {
            return;
        }

        _itemCounts = new Dictionary<string, int>();

        LevelDefinition levelDefinition;
        LevelParser.ParseLevelJson(_levelName, out levelDefinition);

        // Create level based on contents of levelDefinition
        if (levelDefinition == null)
        {
            throw new ArgumentNullException("levelDefinition is null, cannot parse and create level.");
        }
        else
        {
            _planeXScale = levelDefinition.X_Scale;
            _planeZScale = levelDefinition.Z_Scale;

            ScalePlane();
            CreateOuterWalls();
            _verticalDefinitionsList = levelDefinition.VerticalDefinitions;
            SetupLevelContents(levelDefinition);
            
            var playerCoordinates = new Vector3(levelDefinition.Player.X, levelDefinition.Player.Y, levelDefinition.Player.Z);
            // For some reason cylinders in unity start with a default height of 2, so player_height is scaled to half.
            var playerDimensions = new Vector3(Constants.PLAYER_WIDTH, 1f, Constants.PLAYER_LENGTH);
            _player.transform.position = TransformUtils.GetLocalPositionFromGridCoordinates(playerCoordinates, playerDimensions);
        }
	}

    private void ScalePlane()
    {
        // Default plane dim is 10x10.
        _plane.transform.localScale = new Vector3(_planeXScale / 10.0f, 1f, _planeZScale / 10.0f);

        // Top left corner is 0,0
        _plane.transform.position = new Vector3(_planeXScale / 2.0f, 0f, -_planeZScale / 2.0f);
    }

    private void CreateOuterWalls()
    {
        _wallsContainer = new GameObject("Walls");
        var westWall = CreateWall(_planeZScale, "West Wall", _wallsContainer);
        var eastWall = CreateWall(_planeZScale, "East Wall", _wallsContainer);
        var northWall = CreateWall(_planeXScale, "North Wall", _wallsContainer);
        var southWall = CreateWall(_planeXScale, "South Wall", _wallsContainer);

        var widthOffset = Constants.DEFAULT_WALL_WIDTH / 2;

        westWall.transform.position = new Vector3(-1.0f * widthOffset, 0f, -_planeZScale / 2.0f);
        eastWall.transform.position = new Vector3(_planeXScale + widthOffset, 0f, -_planeZScale / 2.0f);

        northWall.transform.position = new Vector3(_planeXScale / 2.0f, 0f, widthOffset);
        northWall.transform.Rotate(new Vector3(0f, 90f, 0f));
        southWall.transform.position = new Vector3(_planeXScale / 2.0f, 0f, -1.0f * _planeZScale - widthOffset);
        southWall.transform.Rotate(new Vector3(0f, 90f, 0f));

        // Create invisible walls so you can't fall off the sides
        var invisWestWall = CreateWall(_planeZScale, "Invisible West Wall", _wallsContainer, visible: false);
        var invisEastWall = CreateWall(_planeZScale, "Invisible East Wall", _wallsContainer, visible: false);
        var invisNorthWall = CreateWall(_planeXScale, "Invisible North Wall", _wallsContainer, visible: false);
        var invisSouthWall = CreateWall(_planeXScale, "Invisible South Wall", _wallsContainer, visible: false);

        // Overlay the walls
        invisWestWall.transform.position = westWall.transform.position;
        invisEastWall.transform.position = eastWall.transform.position;
        invisNorthWall.transform.position = northWall.transform.position;
        invisSouthWall.transform.position = southWall.transform.position;

        // Offset the invis walls
        invisWestWall.transform.Translate(-Constants.PLAYER_WIDTH, Constants.MAX_ENERGY / 2, 0);
        invisEastWall.transform.Translate(Constants.PLAYER_WIDTH, Constants.MAX_ENERGY / 2, 0);
        invisNorthWall.transform.Translate(0, Constants.MAX_ENERGY / 2, Constants.PLAYER_WIDTH);
        invisSouthWall.transform.Translate(0, Constants.MAX_ENERGY / 2, -Constants.PLAYER_WIDTH);

        // Rotate north and south after translation, otherwise it applies the translation wrong :/
        invisNorthWall.transform.rotation = northWall.transform.rotation;
        invisSouthWall.transform.rotation = southWall.transform.rotation;
    }

    private void SetupLevelContents(LevelDefinition levelDefinition)
    {
        // Ensure # of items in levelBase is equal to specified length/width
        if (_planeXScale * _planeZScale != levelDefinition.LevelBase.Count)
        {
            throw new ArgumentNullException("Invalid JSON. LevelBase size does not match length/width arguments.");
        }

        // Set up containers
        _interactableObjectsContainer = new GameObject("InteractableObjects");
        _nonInteractableObjectsContainer = new GameObject("NonInteractableObjects");

        for (int i = 0; i < levelDefinition.LevelBase.Count; i++)
        {
            var item = levelDefinition.LevelBase[i];
            if (item == "0")
            {
                continue;
            }

            var row = i / _planeXScale;
            var col = i % _planeXScale;

            VerticalDefinition itemVerticalDefinition = new VerticalDefinition()
            {
                Id = item,
                StartHeight = 0
            };

            ParseStringsAndCreateGameObjects(itemVerticalDefinition, col, row, lookAtVerticalDefinitions: true);
        }

        // Update platform positions and timers
        foreach (var key in Platforms.Keys)
        {
            var platform = Platforms[key];
            var controller = platform.GetComponent<PlatformController>();

            for (int i = 0; i < PlatformPositions[key].Count; i++)
            {
                var position = PlatformPositions[key][i];
                controller.Positions.Add(position);
            }

            for (int i = 0; i < PlatformMoveTimes[key].Count; i++)
            {
                var moveTime = PlatformMoveTimes[key][i];
                controller.SecondsToReachTarget.Add(moveTime);
            }
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
            var periodIndex = id.IndexOf('.', Constants.GATE_PREFIX.Length);
            var height = Convert.ToInt32(id.Substring(Constants.GATE_PREFIX.Length, periodIndex - Constants.GATE_PREFIX.Length));
            var buttonNumber = Convert.ToInt32(id.Substring(periodIndex + 1));

            CreateGate(height, col, row, startHeight, buttonNumber);
        }
        else if (id.StartsWith(Constants.NO_TOGGLE_BUTTON_PREFIX))
        {
            var buttonNumber = Convert.ToInt32(id.Substring(Constants.NO_TOGGLE_BUTTON_PREFIX.Length));
            CreateButton(col, row, startHeight, buttonNumber, isToggle: false);
        }
        else if (id.StartsWith(Constants.TOGGLE_BUTTON_PREFIX))
        {
            var buttonNumber = Convert.ToInt32(id.Substring(Constants.TOGGLE_BUTTON_PREFIX.Length));
            CreateButton(col, row, startHeight, buttonNumber, isToggle: true);
        }
        else if (id.StartsWith(Constants.BOX_PREFIX))
        {
            var boxHeight = Convert.ToInt32(id.Substring(Constants.BOX_PREFIX.Length));
            CreateBox(boxHeight, col, row, startHeight);
        }
        else if (id.StartsWith(Constants.PLATFORM_PREFIX))
        {
            var periodIndex = id.IndexOf('.', Constants.PLATFORM_PREFIX.Length);
            var periodIndex2 = id.IndexOf('.', periodIndex + 1);
            var periodIndex3 = id.IndexOf(".", periodIndex2 + 1);

            var platNumber = Convert.ToInt32(id.Substring(Constants.PLATFORM_PREFIX.Length, periodIndex - Constants.PLATFORM_PREFIX.Length));
            var platPos = Convert.ToInt32(id.Substring(periodIndex + 1, periodIndex2 - (periodIndex + 1)));
            var platHeight = Convert.ToInt32(id.Substring(periodIndex2 + 1, periodIndex3 - (periodIndex2 + 1)));
            var platTimer = Convert.ToInt32(id.Substring(periodIndex3 + 1));

            CreatePlatform(platNumber, platPos, platTimer, col, row, platHeight);
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
        foreach (var verticalDefinitions in _verticalDefinitionsList)
        {
            if (id == verticalDefinitions.Id)
            {
                return verticalDefinitions.VerticalDefinition;
            };
        }

        return null;
    }

    private GameObject CreateWall(float length, string name, GameObject parent, bool visible = true)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.GetComponent<Collider>().material = noFrictionMaterial;

        // +1 to give a buffer on both sides, so the walls superpose at the edges
        wall.transform.localScale = new Vector3(Constants.DEFAULT_WALL_WIDTH,
                                                Constants.DEFAULT_WALL_HEIGHT,
                                                length + 1f);

        wall.transform.parent = parent.transform;
        wall.name = name;
        wall.GetComponent<Collider>().material = noFrictionMaterial;

        if (!visible)
        {
            var renderer = wall.GetComponent<Renderer>();
            renderer.enabled = false;

            // Make the invis wall taller
            wall.transform.localScale += new Vector3(0, Constants.MAX_ENERGY, 0);
        }

        return wall;
    }

    private GameObject CreateCube(float height, int col, int row, int startHeight)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Collider>().material = noFrictionMaterial;

        cube.transform.localScale = new Vector3(1f, height, 1f);

        var cubeCoordinates = new Vector3(col, startHeight, row);
        var cubeDimensions = new Vector3(1f, height, 1f);
        cube.transform.position = TransformUtils.GetLocalPositionFromGridCoordinates(cubeCoordinates, cubeDimensions);

        cube.transform.parent = _nonInteractableObjectsContainer.transform;
        cube.name = CreateUniqueItemName("Cube_" + height);

        return cube;
    }

    private Transform CreateGate(float height, int col, int row, int startHeight, int buttonNumber)
    {
        var gate = Instantiate(_gatePrefab, _interactableObjectsContainer.transform);
        gate.GetComponent<Collider>().material = noFrictionMaterial;

        gate.localScale = new Vector3(1f, height, 1f);

        var gateCoordinates = new Vector3(col, startHeight, row);
        var gateDimensions = new Vector3(1f, height, 1f);
        gate.position = TransformUtils.GetLocalPositionFromGridCoordinates(gateCoordinates, gateDimensions);
        gate.name = CreateUniqueItemName("Gate_H" + height + "_" + buttonNumber);        

        // If button has not been created yet, create an empty GameObject that will be replaced later.
        GameObject button;
        if (!Buttons.TryGetValue(buttonNumber, out button)) {
            button = null;
            Buttons[buttonNumber] = button;
        }

        gate.GetComponent<GateController>().Button = button;

        List<Transform> gatesList;
        if (!Gates.TryGetValue(buttonNumber, out gatesList))
        {
            Gates[buttonNumber] = new List<Transform>
            {
                gate
            };
        }
        else
        {
            Gates[buttonNumber].Add(gate);
        }

        return gate;
    }
    
    private Transform CreateButton(int col, int row, int startHeight, int buttonNumber, bool isToggle)
    {
        // Button dimensions are the same as the prefab, so I don't change them here.
        var button = Instantiate(_buttonPrefab, _interactableObjectsContainer.transform);
        button.GetComponent<Collider>().material = noFrictionMaterial;

        var buttonCoordinates = new Vector3(col, startHeight, row);
        var buttonDimensions = new Vector3(.5f, .25f, .5f);
        button.position = TransformUtils.GetLocalPositionFromGridCoordinates(buttonCoordinates, buttonDimensions);
        button.name = CreateUniqueItemName("Button_" + (isToggle ? "T_" : "NT_") + buttonNumber);
        button.GetComponent<ButtonController>().Toggleable = isToggle;
        
        // If gates already exist for this button, make sure to assign this button to them.
        List<Transform> gatesList;
        if (Gates.TryGetValue(buttonNumber, out gatesList)) {
            foreach (var gate in gatesList)
            {
                gate.GetComponent<GateController>().Button = button.gameObject;
            }
        }

        Buttons[buttonNumber] = button.gameObject;
        return button;
    }

    private Transform CreateBox(float height, int col, int row, int startHeight)
    {
        var box = Instantiate(_boxPrefab, _interactableObjectsContainer.transform);
        box.GetComponent<Collider>().material = noFrictionMaterial;

        box.localScale = new Vector3(1f, height, 1f);

        var boxCoordinates = new Vector3(col, startHeight, row);
        var boxDimensions = new Vector3(1f, height, 1f);
        box.position = TransformUtils.GetLocalPositionFromGridCoordinates(boxCoordinates, boxDimensions);
        box.name = CreateUniqueItemName("Box_" + height);

        return box;
    }

    private Transform CreatePlatform(int platformNumber, int positionNumber, int moveTime, int col, int row, int startHeight)
    {
        // Create the platform if it doesn't exist yet.
        Transform platform;
        if (!Platforms.TryGetValue(platformNumber, out platform))
        {
            platform = Instantiate(_platformPrefab, _nonInteractableObjectsContainer.transform);
            platform.GetComponent<Collider>().material = noFrictionMaterial;
            platform.name = CreateUniqueItemName("Platform");

            var controller = platform.GetComponent<PlatformController>();
            controller.Positions = new List<Vector3>();
            controller.SecondsToReachTarget = new List<int>();
            Platforms[platformNumber] = platform;
        }

        // Determine where this point falls on the grid.
        var pointCoordinates = new Vector3(col, startHeight, row);
        var platformDimensions = platform.localScale;
        var platformPosition = TransformUtils.GetLocalPositionFromGridCoordinates(pointCoordinates, platformDimensions);

        // Add the position to PlatformPositions
        List<Vector3> positions;
        if (!PlatformPositions.TryGetValue(platformNumber, out positions)) {
            positions = new List<Vector3>();
        }

        while (positionNumber >= positions.Count)
        {
            positions.Add(Vector3.zero);
        }
        positions[positionNumber] = platformPosition;
        PlatformPositions[platformNumber] = positions;

        // Add the move time to PlatformMoveTimes
        List<int> moveTimes;
        if (!PlatformMoveTimes.TryGetValue(platformNumber, out moveTimes))
        {
            moveTimes = new List<int>();
        }

        while (positionNumber >= moveTimes.Count)
        {
            moveTimes.Add(0);
        }
        moveTimes[positionNumber] = moveTime;
        PlatformMoveTimes[platformNumber] = moveTimes;

        // Set the platform start location
        if (positionNumber == 0)
        {
            platform.position = platformPosition;
            Platforms[platformNumber] = platform;
        }

        return platform;
    }

    private string CreateUniqueItemName(string key)
    {
        if (!_itemCounts.ContainsKey(key))
        {
            _itemCounts[key] = 0;
        }
        _itemCounts[key]++;
        int itemNumber = _itemCounts[key];
        var result = key + " (" + itemNumber + ")";

        return result;
    }
}