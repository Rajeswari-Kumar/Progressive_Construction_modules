﻿using System;
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
    [Header("Inventory List")]
    [SerializeField]
    private List<NamedInventory> objectList = new List<NamedInventory>();

    private Dictionary<string, GameObject> objectDict;

    [Header("Currently Selected Window")]
    public GameObject selectedWindow;
    private Color originalColor;
    private bool hasOriginalColor = false;

    [Header("Fixture Selection")]
    public GameObject fixtureMarkerPrefab; // Assign a small sphere prefab in Inspector
    private GameObject selectedFixturePoint;
    public bool templateFixture = false;

    // New flag: True if a "Fixture template" is selected, blocks spawning fixture points
    public bool isTemplateFixtureSelected = false;

    // New flag: True if a fixture point is selected, blocks spawning new fixture points
    private bool isFixturePointSelected = false;

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

    /// <summary>
    /// Get object from dictionary by name.
    /// </summary>
    public GameObject GetObjectByName(string name)
    {
        if (objectDict.TryGetValue(name, out GameObject obj))
        {
            return obj;
        }

        Debug.LogWarning($"Object with name '{name}' not found in dictionary.");
        return null;
    }

    void Update()
    {
        if (FindObjectOfType<Fix_the_template>().isFixed)
        {
            if (Input.GetMouseButtonDown(0)) // Left-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObj = hit.collider.gameObject;

                    // Priority 1: Selecting/deselecting fixture points
                    if (clickedObj == selectedFixturePoint)
                    {
                        // Clicked selected fixture point again → deselect
                        DeselectFixturePoint();
                        return;
                    }
                    else if (clickedObj.CompareTag("Fixture template"))
                    {
                        // Select a new fixture point (different one)
                        SelectFixturePoint(clickedObj);
                        return;
                    }

                    // Priority 2: If fixture point is selected, prevent spawning new ones
                    if (isFixturePointSelected)
                    {
                        Debug.Log("Fixture point selected — no new fixture points can be created.");
                        return;
                    }

                    // Priority 3: Handle window or fixture template selection
                    if (clickedObj.CompareTag("Window") || clickedObj.CompareTag("Fixture template"))
                    {
                        SelectWindow(clickedObj);
                        return;
                    }

                    // Priority 4: Spawn fixture point if clicking Wall or Floor
                    if (clickedObj.CompareTag("Wall") || clickedObj.CompareTag("Floor"))
                    {
                        SpawnFixturePoint(hit.point, clickedObj.transform);
                        return;
                    }

                    // Priority 5: Clicked something else, deselect windows and fixture points
                    if (selectedWindow != null)
                    {
                        DeselectWindow();
                    }
                    if (isFixturePointSelected)
                    {
                        DeselectFixturePoint();
                    }
                }
                else
                {
                    // Clicked empty space — deselect everything
                    if (selectedWindow != null)
                    {
                        DeselectWindow();
                    }
                    if (isFixturePointSelected)
                    {
                        DeselectFixturePoint();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawn object 2 units in front of camera.
    /// </summary>
    public void SpawnObjects(string objectName)
    {
        Camera cam = Camera.main;
        Vector3 spawnPosition = cam.transform.position + cam.transform.forward * 2f;
        Instantiate(GetObjectByName(objectName), spawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// Replace selected window with new prefab.
    /// </summary>
    public void ReplaceSelectedWindow(string newObjectName)
    {
        if (selectedWindow == null)
        {
            Debug.LogWarning("No window is currently selected.");
            return;
        }

        GameObject replacementPrefab = GetObjectByName(newObjectName);

        if (replacementPrefab == null)
        {
            Debug.LogWarning($"No prefab found for '{newObjectName}'.");
            return;
        }

        Vector3 position = selectedWindow.transform.position + (selectedWindow.CompareTag("Window") ? new Vector3(0, -2f, 0.5f) : new Vector3(0, 0, 0));
        Quaternion rotation = selectedWindow.transform.rotation;
        Vector3 scaleWindow = selectedWindow.transform.localScale;
        Transform parent = selectedWindow.transform.parent;

        Destroy(selectedWindow);

        GameObject newWindow = Instantiate(replacementPrefab, position, rotation, parent);
        newWindow.transform.localScale = scaleWindow;
        selectedWindow = newWindow;

        Debug.Log($"Replaced window with '{newObjectName}'");
    }

    /// <summary>
    /// Called from UI button.
    /// </summary>
    public void OnInventoryButtonClick(string objectName)
    {
        if (FindObjectOfType<Fix_the_template>().isFixed == true)
            ReplaceSelectedWindow(objectName);
    }

    /// <summary>
    /// Select a window, or deselect if already selected.
    /// Also sets the flag to block fixture spawning if fixture template selected.
    /// </summary>
    public void SelectWindow(GameObject window)
    {
        // Clicked the same window again → toggle deselect
        if (selectedWindow == window)
        {
            DeselectWindow();
            return;
        }

        // Deselect previous if selecting a new one
        if (selectedWindow != null)
        {
            DeselectWindow();
        }

        Renderer rend = window.GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
            hasOriginalColor = true;
            rend.material.color = Color.yellow; // highlight
        }

        selectedWindow = window;
        Debug.Log($"Selected window: {window.name}");

        // Update flag depending on tag
        isTemplateFixtureSelected = window.CompareTag("Fixture template");
    }

    /// <summary>
    /// Deselect the current window and reset color and flags.
    /// </summary>
    public void DeselectWindow()
    {
        if (selectedWindow == null)
            return;

        Renderer rend = selectedWindow.GetComponent<Renderer>();
        if (rend != null && hasOriginalColor)
        {
            rend.material.color = originalColor;
        }

        Debug.Log($"Deselected window: {selectedWindow.name}");

        // Reset flag
        isTemplateFixtureSelected = false;

        selectedWindow = null;
        hasOriginalColor = false;
    }

    /// <summary>
    /// Select a fixture point and highlight it.
    /// </summary>
    public void SelectFixturePoint(GameObject fixturePoint)
    {
        if (selectedFixturePoint != null && selectedFixturePoint != fixturePoint)
        {
            DeselectFixturePoint(); // Deselect previous
        }

        selectedFixturePoint = fixturePoint;

        // Highlight selected fixture point (change color or scale)
        Renderer rend = selectedFixturePoint.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green; // example highlight color
        }

        isFixturePointSelected = true;

        Debug.Log($"Selected fixture point: {selectedFixturePoint.name}");
    }

    /// <summary>
    /// Deselect the current fixture point and reset its color.
    /// </summary>
    public void DeselectFixturePoint()
    {
        if (selectedFixturePoint == null)
            return;

        Renderer rend = selectedFixturePoint.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white; // assuming white is default
        }

        Debug.Log($"Deselected fixture point: {selectedFixturePoint.name}");

        selectedFixturePoint = null;
        isFixturePointSelected = false;
    }

    public void templateFixturebool() => templateFixture = !templateFixture;

    public void SpawnFixturePoint(Vector3 position, Transform parent)
    {
        if (templateFixture && isFixturePointSelected == false)
        {
            if (selectedFixturePoint != null)
                Destroy(selectedFixturePoint); // Replace previous

            // Instantiate WITHOUT parent first (to keep prefab scale)
            selectedFixturePoint = Instantiate(fixtureMarkerPrefab, position, Quaternion.identity);

            // Now assign parent with worldPositionStays = true to maintain world position
            selectedFixturePoint.transform.SetParent(parent, worldPositionStays: true);

            Vector3 originalGlobalScale = fixtureMarkerPrefab.transform.lossyScale;
            Vector3 parentGlobalScale = parent.lossyScale;

            // Adjust local scale to maintain original global scale despite parent scaling
            selectedFixturePoint.transform.localScale = new Vector3(
                originalGlobalScale.x / parentGlobalScale.x,
                originalGlobalScale.y / parentGlobalScale.y,
                originalGlobalScale.z / parentGlobalScale.z
            );
        }
    }
}
