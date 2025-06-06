using UnityEngine;
using TMPro;

public class WindowEdgeDistanceDisplay : MonoBehaviour
{
    public GameObject canvasPrefab; // Assign a world space canvas prefab
    public Vector3 canvasOffset = new Vector3(0, 0.2f, 0); // Optional Y offset for readability

    private GameObject canvasInstance;
    private TextMeshProUGUI distanceText;

    private Transform wall;
    private float wallHalfWidth;

    void Start()
    {
        UpdateEdgeDistance();
    }


    public void UpdateEdgeDistance()
    {
        wall = transform.parent;

        if (wall == null)
        {
            Debug.LogWarning("Window is not parented to a wall.");
            return;
        }

        wallHalfWidth = wall.localScale.x / 2f;

        canvasInstance = Instantiate(canvasPrefab);
        canvasInstance.name = $"EdgeDistanceCanvas_{name}";
        canvasInstance.transform.SetParent(null); // World-space

        distanceText = canvasInstance.GetComponentInChildren<TextMeshProUGUI>();
        //UpdateCanvasPosition();
    }

    void Update()
    {
        //UpdateCanvasPosition();
    }

    public void UpdateCanvasPosition()
    {
        if (canvasInstance == null || wall == null) return;

        float windowX = transform.localPosition.x;
        float windowHalfWidth = transform.localScale.x / 2f;

        float leftDistance = Mathf.Abs(-wallHalfWidth - (windowX - windowHalfWidth));
        float rightDistance = Mathf.Abs(wallHalfWidth - (windowX + windowHalfWidth));
        bool isCloserToLeft = leftDistance < rightDistance;

        float distance = isCloserToLeft ? leftDistance : rightDistance;

        // Calculate world position of the canvas between window edge and wall edge
        Vector3 edgeWorld = wall.TransformPoint(new Vector3(isCloserToLeft ? -wallHalfWidth : wallHalfWidth, transform.localPosition.y, transform.localPosition.z));
        Vector3 windowEdgeWorld = transform.TransformPoint(new Vector3(isCloserToLeft ? -0.5f : 0.5f, 0, 0) * transform.localScale.x);

        Vector3 midpoint = (edgeWorld + windowEdgeWorld) / 2f ;

        canvasInstance.transform.position = midpoint;
        canvasInstance.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward); // Face camera

        if (distanceText != null)
        {
            distanceText.text = $"{distance:F2} m";
        }
    }

    void OnDestroy()
    {
        if (canvasInstance != null)
        {
            Destroy(canvasInstance);
        }
    }
}
