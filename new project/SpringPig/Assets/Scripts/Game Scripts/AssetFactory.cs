using System.Collections.Generic;
using UnityEngine;

public class AssetFactory : MonoBehaviour
{
    private LevelSetup _levelSetup;

    private List<Transform> _gates = new List<Transform>();
    private List<Transform> _buttons = new List<Transform>();
    private List<Transform> _boxes = new List<Transform>();
    private List<Transform> _lava = new List<Transform>();
    private List<Transform> _pickups = new List<Transform>();
    private List<Transform> _platforms = new List<Transform>();
    private List<GameObject> _cubes = new List<GameObject>();

    private void Awake()
    {
        _levelSetup = gameObject.GetComponent<LevelSetup>();
    }

    // GETTERS

    public Transform GetGate()
    {
        return GetTransform(_gates, _levelSetup.GatePrefab, _levelSetup.InteractableObjectsContainer);
    }

    public Transform GetButton()
    {
        return GetTransform(_buttons, _levelSetup.ButtonPrefab, _levelSetup.InteractableObjectsContainer);
    }

    public Transform GetBox()
    {
        return GetTransform(_boxes, _levelSetup.BoxPrefab, _levelSetup.InteractableObjectsContainer);
    }

    public Transform GetLava()
    {
        return GetTransform(_lava, _levelSetup.LavaPrefab, _levelSetup.NonInteractableObjectsContainer);
    }

    public Transform GetPickup()
    {
        return GetTransform(_pickups, _levelSetup.PickupPrefab, _levelSetup.InteractableObjectsContainer);
    }

    public Transform GetPlatform()
    {
        return GetTransform(_platforms, _levelSetup.PlatformPrefab, _levelSetup.NonInteractableObjectsContainer);
    }

    public GameObject GetCube()
    {
        return GetPrimitive(_cubes, PrimitiveType.Cube);
    }

    // DESTROYERS

    public void DestroyGate(Transform gate)
    {
        ResetTransform(gate);

        var controller = gate.GetComponent<GateController>();
        controller.Button = null;
    }

    public void DestroyButton(Transform button)
    {
        ResetTransform(button);

        var controller = button.GetComponent<ButtonController>();
        controller.Toggleable = false;
    }

    public void DestroyBox(Transform box)
    {
        ResetTransform(box);
    }

    public void DestroyLava(Transform lava)
    {
        ResetTransform(lava);
    }

    public void DestroyPickup(Transform pickup)
    {
        ResetTransform(pickup);

        var controller = pickup.GetComponent<PickupController>();
        controller.PickupEffect = string.Empty;
        controller.Value = 0;
    }

    public void DestroyPlatform(Transform platform)
    {
        ResetTransform(platform);

        var controller = platform.GetComponent<PlatformController>();
        controller.Positions.Clear();
        controller.SecondsToReachTarget.Clear();
    }

    public void DestroyCube(GameObject cube)
    {
        ResetTransform(cube.transform);
    }

    // HELPER METHODS

    private void ResetTransform(Transform t)
    {
        t.position = Vector3.zero;
        t.localScale = Vector3.zero;
        t.name = string.Empty;
        t.parent = null;
        t.DetachChildren();
    }

    private Transform GetTransform(List<Transform> transformList, Transform prefab, GameObject container)
    {
        var numItems = transformList.Count;
        Transform result;
        if (numItems > 0)
        {
            result = transformList[numItems - 1];
            transformList.RemoveAt(numItems - 1);
        }
        else
        {
            result = Instantiate(prefab, container.transform);
        }

        return result;
    }

    public GameObject GetPrimitive(List<GameObject> gameObjectList, PrimitiveType primitiveType)
    {
        var numItems = gameObjectList.Count;
        GameObject result;
        if (numItems > 0)
        {
            result = gameObjectList[numItems - 1];
            gameObjectList.RemoveAt(numItems - 1);
        }
        else
        {
            result = GameObject.CreatePrimitive(primitiveType);
        }

        return result;
    }
}