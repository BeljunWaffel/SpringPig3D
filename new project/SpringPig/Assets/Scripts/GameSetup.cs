using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public string _levelName;

    private Dictionary<string, int> itemCounts;

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
            CreateWalls(length, width);

            // Ensure # of items in levelBase is equal to specified length/width
            if (length * width != levelDefinition.LevelBase.Count)
            {
                throw new ArgumentNullException("Invalid JSON. LevelBase size does not match length/width arguments.");
            }

            // Set up containers
            //var interactableObjectsContainer = new GameObject("InteractableObjects");
            var nonInteractableObjectsContainer = new GameObject("NonInteractableObjects");

            for (int i = 0; i < levelDefinition.LevelBase.Count; i++) {
                var item = levelDefinition.LevelBase[i];
                var row = i / width;
                var col = i % length;

                // Refer to LevelBaseDefinitions.json, which contains definitions for each itemType.
                if (item is Int64)
                {
                    var itemType = Convert.ToInt32(item);
                    switch (itemType)
                    {
                        case 0:
                            // Do Nothing
                            break;
                        case 1:
                            // Cube of Height 1
                            CreateCube(1f, col, row, CreateUniqueItemName("Cube_1"), nonInteractableObjectsContainer);
                            break;
                        default:
                            // Do Nothing
                            break;
                    }
                }
                else if (item is string)
                {
                    var itemType = Convert.ToString(item);
                    if (itemType.StartsWith("1."))
                    {
                        var height = Convert.ToInt32(itemType.Substring(2));
                        CreateCube(height, col, row, CreateUniqueItemName("Cube_" + height), nonInteractableObjectsContainer);
                    }
                    else
                    {

                    }
                }
            }
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

    private void CreateWalls(int length, int width)
    {
        var wallContainer = new GameObject("Walls");
        var westWall = CreateWall(width, "West Wall", wallContainer);
        var eastWall = CreateWall(width, "East Wall", wallContainer);
        var northWall = CreateWall(length, "North Wall", wallContainer);
        var southWall = CreateWall(length, "South Wall", wallContainer);

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

    private GameObject CreateCube(float height, int column, int row, string name, GameObject parent)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoxCollider>();

        // +1 to give a buffer on both sides, so the walls superpose at the edges
        cube.transform.localScale = new Vector3(1f, height, 1f);
        cube.transform.position = new Vector3(0.5f + column, height / 2.0f, -.5f - row);

        cube.transform.parent = parent.transform;
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