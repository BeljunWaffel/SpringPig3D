using System.Collections.Generic;
using UnityEngine;

public class AssetPool : MonoBehaviour
{
    private GameObject _assetPool;

    private LevelSetup _levelSetup;

    private List<Transform> _gates = new List<Transform>();
    private List<Transform> _buttons = new List<Transform>();
    private List<Transform> _boxes = new List<Transform>();
    private List<Transform> _lava = new List<Transform>();
    private List<Transform> _pickups = new List<Transform>();
    private List<Transform> _platforms = new List<Transform>();
    private List<Transform> _enemies = new List<Transform>();
    private List<GameObject> _cubes = new List<GameObject>();

    private void Awake()
    {
        _levelSetup = gameObject.GetComponent<LevelSetup>();
        _assetPool = new GameObject("AssetPool");
    }
    
    public Transform SpawnTransform(string tag)
    {
        if (tag == Constants.TAG_BOX)
        {
            return SpawnTransformHelper(_boxes, _levelSetup.BoxPrefab, _levelSetup.InteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_BUTTON)
        {
            return SpawnTransformHelper(_buttons, _levelSetup.ButtonPrefab, _levelSetup.InteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_GATE)
        {
            return SpawnTransformHelper(_gates, _levelSetup.GatePrefab, _levelSetup.InteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_LAVA)
        {
            return SpawnTransformHelper(_lava, _levelSetup.LavaPrefab, _levelSetup.NonInteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_PICKUP)
        {
            return SpawnTransformHelper(_pickups, _levelSetup.PickupPrefab, _levelSetup.InteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_PLATFORM)
        {
            return SpawnTransformHelper(_platforms, _levelSetup.PlatformPrefab, _levelSetup.NonInteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_ENEMY)
        {
            return SpawnTransformHelper(_enemies, _levelSetup.EnemyPrefab, _levelSetup.InteractableObjectsContainer);
        }
        else if (tag == Constants.TAG_WALL)
        {
            var cube = SpawnPrimitive(_cubes, PrimitiveType.Cube, Constants.TAG_WALL);
            return cube.transform;
        }
        else if (tag == Constants.TAG_OBSTACLE)
        {
            var cube = SpawnPrimitive(_cubes, PrimitiveType.Cube, Constants.TAG_OBSTACLE);
            return cube.transform;
        }
        else
        {
            throw new System.ComponentModel.InvalidEnumArgumentException("Destroy function for this transform could not be found. Tag:" + tag);
        }
    }
    
    public void PoolTransform(Transform t)
    {
        var tagList = t.GetComponent<TagList>();
        if (tagList != null)
        {
            if (tagList.Tags.Contains(Constants.TAG_BOX))
            {
                PoolBox(t);
                t.gameObject.name = Constants.TAG_BOX;
            }
            else if (tagList.Tags.Contains(Constants.TAG_BUTTON))
            {
                PoolButton(t);
                t.gameObject.name = Constants.TAG_BUTTON;
            }
            else if (tagList.Tags.Contains(Constants.TAG_GATE))
            {
                PoolGate(t);
                t.gameObject.name = Constants.TAG_GATE;
            }
            else if (tagList.Tags.Contains(Constants.TAG_LAVA))
            {
                PoolLava(t);
                t.gameObject.name = Constants.TAG_LAVA;
            }
            else if (tagList.Tags.Contains(Constants.TAG_PICKUP))
            {
                PoolPickup(t);
                t.gameObject.name = Constants.TAG_PICKUP;
            }
            else if (tagList.Tags.Contains(Constants.TAG_PLATFORM))
            {
                PoolPlatform(t);
                t.gameObject.name = Constants.TAG_PLATFORM;
            }
            else if (tagList.Tags.Contains(Constants.TAG_WALL))
            {
                PoolWall(t);
                t.gameObject.name = "Cube";
            }
            else if (tagList.Tags.Contains(Constants.TAG_OBSTACLE))
            {
                PoolObstacle(t);
                t.gameObject.name = "Cube";
            }
            else
            {
                throw new System.ComponentModel.InvalidEnumArgumentException("Destroy function for this transform could not be found. Tags:" + string.Join(",", tagList.Tags.ToArray()));
            }

            t.gameObject.SetActive(false);
            t.parent = _assetPool.transform;
        }
    }

    private void PoolWall(Transform t)
    {
        ResetTransform(t);
        var renderer = t.GetComponent<Renderer>();
        renderer.enabled = true;

        _cubes.Add(t.gameObject);
    }

    private void PoolObstacle(Transform t)
    {
        ResetTransform(t);
        _cubes.Add(t.gameObject);
    }

    private void PoolGate(Transform gate)
    {
        ResetTransform(gate);

        var controller = gate.GetComponent<GateController>();
        controller.Button = null;

        _gates.Add(gate);
    }

    private void PoolButton(Transform button)
    {
        ResetTransform(button);

        var controller = button.GetComponent<ButtonController>();
        controller.Toggleable = false;

        _buttons.Add(button);
    }

    private void PoolBox(Transform box)
    {
        ResetTransform(box);
        _boxes.Add(box);
    }

    private void PoolLava(Transform lava)
    {
        ResetTransform(lava);
        _lava.Add(lava);
    }

    private void PoolPickup(Transform pickup)
    {
        ResetTransform(pickup);

        var controller = pickup.GetComponent<PickupController>();
        controller.PickupEffect = string.Empty;
        controller.Value = 0;
        _pickups.Add(pickup);
    }

    private void PoolPlatform(Transform platform)
    {
        ResetTransform(platform);

        var controller = platform.GetComponent<PlatformController>();
        controller.Positions.Clear();
        controller.SecondsToReachTarget.Clear();
        controller.IsSet = false;
        _platforms.Add(platform);
    }

    // HELPER METHODS

    private void ResetTransform(Transform t)
    {
        t.position = Vector3.zero;
        t.localRotation = Quaternion.identity;
        //t.localScale = Vector3.zero;
        t.parent = null;
        t.DetachChildren();
    }

    private Transform SpawnTransformHelper(List<Transform> transformList, Transform prefab, GameObject container)
    {
        var numItems = transformList.Count;
        Transform result;
        if (numItems > 0)
        {
            result = transformList[numItems - 1];
            transformList.RemoveAt(numItems - 1);
            result.localScale = prefab.localScale;
            result.parent = container.transform;
        }
        else
        {
            result = Instantiate(prefab, container.transform);
        }

        result.gameObject.SetActive(true);
        return result;
    }

    private GameObject SpawnPrimitive(List<GameObject> gameObjectList, PrimitiveType primitiveType, string tag)
    {
        var numItems = gameObjectList.Count;
        GameObject result;
        if (numItems > 0)
        {
            result = gameObjectList[numItems - 1];
            gameObjectList.RemoveAt(numItems - 1);
            result.GetComponent<TagList>().Tags.Clear();
            result.GetComponent<TagList>().Tags.Add(tag);
        }
        else
        {
            result = GameObject.CreatePrimitive(primitiveType);
            result.AddComponent<TagList>().Tags.Add(tag);
        }

        result.gameObject.SetActive(true);
        return result;
    }
}