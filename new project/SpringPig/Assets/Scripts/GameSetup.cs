using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public string _levelName;

    private Dictionary<string, int> itemCounts;
    private Dictionary<string, List<Heights>> verticalDefinitions;

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
            var length = levelDefinition.Length;
            var width = levelDefinition.Width;

            ScalePlane(length, width);
            CreateOuterWalls(length, width);
            ParseVerticalDefinitions(levelDefinition);
            SetupLevelContents(length, width, levelDefinition);
        }
	}

    private void ScalePlane(int length, int width)
    {
        var plane = GameObject.Find("Plane");
        if (plane == null)
        {
            throw new ArgumentNullException("Could not find plane.");
        }
        else
        {
            // Default plane dim is 10x10.
            plane.transform.localScale = new Vector3(length / 10.0f, 1f, width / 10.0f);

            // Top left corner is 0,0
            plane.transform.position = new Vector3(width / 2.0f, 0f, -length / 2.0f);
        }
    }

    private void CreateOuterWalls(int length, int width)
    {
        wallsContainer = new GameObject("Walls");
        var westWall = CreateWall(width, "West Wall", wallsContainer);
        var eastWall = CreateWall(width, "East Wall", wallsContainer);
        var northWall = CreateWall(length, "North Wall", wallsContainer);
        var southWall = CreateWall(length, "South Wall", wallsContainer);

        var widthOffset = Constants.DEFAULT_WALL_WIDTH / 2;

        westWall.transform.position = new Vector3(-1.0f * widthOffset, 0f, -length / 2.0f);
        eastWall.transform.position = new Vector3(width + widthOffset, 0f, -length / 2.0f);

        northWall.transform.position = new Vector3(width / 2.0f, 0f, widthOffset);
        northWall.transform.Rotate(new Vector3(0f, 90f, 0f));
        southWall.transform.position = new Vector3(width / 2.0f, 0f, -1.0f * length - widthOffset);
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

    private void SetupLevelContents(int length, int width, LevelDefinition levelDefinition)
    {
        // Ensure # of items in levelBase is equal to specified length/width
        if (length * width != levelDefinition.LevelBase.Count)
        {
            throw new ArgumentNullException("Invalid JSON. LevelBase size does not match length/width arguments.");
        }

        // Set up containers
        interactableObjectsContainer = new GameObject("InteractableObjects");
        nonInteractableObjectsContainer = new GameObject("NonInteractableObjects");

        for (int i = 0; i < levelDefinition.LevelBase.Count; i++)
        {
            var item = levelDefinition.LevelBase[i];
            var row = i / width;
            var col = i % length;

            // Refer to LevelBaseDefinitions.json, which contains definitions for each itemType.
            if (item is Int64)
            {
                var id = Convert.ToInt32(item);
                ParseIntsAndCreateGameObjects(id, col, row);
            }
            else if (item is string)
            {
                var itemType = Convert.ToString(item);
                if (itemType.StartsWith("1."))
                {
                    var height = Convert.ToInt32(itemType.Substring(2));
                    CreateCube(height, col, row, 0, CreateUniqueItemName("Cube_" + height));
                }
                else
                {
                    List<Heights> heights;
                    if (!verticalDefinitions.TryGetValue(itemType, out heights))
                    {
                        Debug.Log("Could not find height definition for " + itemType);
                    }
                    else
                    {
                        foreach (var height in heights)
                        {
                            var id = Convert.ToInt32(height.Id);
                            ParseIntsAndCreateGameObjects(id, col, row, height.StartHeight, height.EndHeight);
                        }
                    }
                }
            }
        }
    }

    private void ParseIntsAndCreateGameObjects(int id, int col, int row, int startHeight = 0, int endHeight = 0)
    {
        switch (id)
        {
            case 0:
                // Do Nothing
                break;
            case 1:
                // Cube of Height 1
                int height = endHeight - startHeight;
                CreateCube(height, col, row, startHeight, CreateUniqueItemName("Cube_1"));
                break;
            default:
                // Do Nothing
                break;
        }
    }

    private void ParseVerticalDefinitions(LevelDefinition levelDefinition)
    {
        verticalDefinitions = new Dictionary<string, List<Heights>>();
        foreach (var verticalDefinition in levelDefinition.VerticalDefinitions)
        {
            var id = verticalDefinition.Id;
            verticalDefinitions.Add(id, verticalDefinition.Heights);
        }
    }

    private GameObject CreateCube(float height, int column, int row, int startHeight, string name)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoxCollider>();

        // +1 to give a buffer on both sides, so the walls superpose at the edges
        cube.transform.localScale = new Vector3(1f, height, 1f);
        cube.transform.position = new Vector3(0.5f + column, height / 2.0f + startHeight, -.5f - row);

        cube.transform.parent = nonInteractableObjectsContainer.transform;
        cube.name = name;

        return cube;
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
}