using UnityEngine;

public class Move_around_windows : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 0.005f;

    private bool isSelected = false;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        HandleSelection();

        if (isSelected)
        {
            MoveWithMouse();
        }
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isSelected = !isSelected; // Toggle selection
                    transform.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                }
                else
                {
                    isSelected = false; // Deselect if clicked elsewhere
                }
            }
            else
            {
                isSelected = false; // Deselect if clicked empty space
            }
        }
    }

    void MoveWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 moveDirection = Vector3.zero;

        moveDirection = new Vector3(mouseX, mouseY, 0);
        transform.localPosition += moveDirection * moveSpeed;
        transform.GetComponent<WindowEdgeDistanceDisplay>().UpdateCanvasPosition();
    }
}
