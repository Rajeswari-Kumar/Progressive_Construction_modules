using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class NamedInventory
{
    public string name;
    public GameObject gameObject;
}

public class NamedGameObjectDictionary : MonoBehaviour
{
    [SerializeField]
    private List<NamedInventory> objectList = new List<NamedInventory>();

    private Dictionary<string, GameObject> objectDict;

    private void Awake()
    {
        objectDict = new Dictionary<string, GameObject>();
        foreach (var item in objectList)
        {
            if (!string.IsNullOrEmpty(item.name) && item.gameObject != null)
            {
                objectDict[item.name] = item.gameObject;
            }
        }
    }

    // Example access method
    public GameObject GetObjectByName(string name)
    {
        if (objectDict.TryGetValue(name, out GameObject obj))
        {
            return obj;
        }
        Debug.LogWarning($"Object with name '{name}' not found.");
        return null;
    }

    public void SpawnObjects(String ObjectName)
    {
        Camera cam = Camera.main;
        Vector3 spawnPosition = cam.transform.position + cam.transform.forward * 2f; // 2 units in front
        Instantiate(GetObjectByName(ObjectName), spawnPosition, Quaternion.identity);

    }
}