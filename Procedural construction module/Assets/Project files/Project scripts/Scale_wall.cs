using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Scale_wall : MonoBehaviour
{
    public XRBaseInteractor currentInteractor;
    public float scaleAmount = 0.001f;
    public float LastWallWidth;
    [SerializeField] public InputActionProperty LeftTriggerScaleUpWidth;
    [SerializeField] public InputActionProperty RightTriggerScaleUpHeight;
    [SerializeField] public InputActionProperty XButtonScaleDownWidth;


    //private void OnEnable()
    //{
    //    XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
    //    interactable.hoverEntered.AddListener(OnHoverEnter);
    //    interactable.hoverExited.AddListener(OnHoverExit);
    //}

    //private void OnDisable()
    //{
    //    XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
    //    interactable.hoverEntered.RemoveListener(OnHoverEnter);
    //    interactable.hoverExited.RemoveListener(OnHoverExit);
    //}
    private void Start()
    {
        LastWallWidth = this.transform.localScale.x;
    }
    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        currentInteractor = args.interactor;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void Update()
    {
        if (currentInteractor == null) return;

        string interactorName = currentInteractor.name.ToLower();

        if (interactorName.Contains("right") && RightTriggerScaleUpHeight.action.IsPressed())
        {
            ScaleWall(true); // Scale up height
            //RescaleNeighbourWindow(false, GetNeighbourWindows());
        }
        else if (interactorName.Contains("left") && LeftTriggerScaleUpWidth.action.IsPressed())
        {
            ScaleWall(true);// Scale down width
        }
        else if (RightTriggerScaleUpHeight.action.IsPressed() && LeftTriggerScaleUpWidth.action.IsPressed())
        {
            ScaleWall(false);
        }
    }
    private Transform[] GetNeighbourWindows()
    {
        Transform parent = this.transform;
        if (parent == null) return new Transform[0];

        var neighbours = new System.Collections.Generic.List<Transform>();

        foreach (Transform child in parent)
        {
            if (child != this.transform && child.CompareTag("Window"))
            {
                neighbours.Add(child);
            }
        }

        return neighbours.ToArray();
    }


    //private void ScaleWindow(bool scaleUp)
    //{
    //    float direction = scaleUp ? 1f : -1f;
    //    Vector3 newScale = transform.localScale + Vector3.one * direction * scaleAmount;

    //    // Clamp to minimum scale
    //    newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
    //    transform.localScale = newScale;
    //}

    private void ScaleWall(bool scaleUp)
    {
        float direction = scaleUp ? 1f : -1f;

        Vector3 newScale = transform.localScale;

        if (currentInteractor.name.ToLower().Contains("right"))
        {
            // Scale only height (Y-axis)
            newScale.y += direction * scaleAmount;
            newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
        }
        else if (currentInteractor.name.ToLower().Contains("left"))
        {
            // Scale only width (X-axis)
            newScale.x += direction * scaleAmount;
            newScale.x = Mathf.Max(newScale.x, 0.1f); // Clamp to min width


            Transform[] existingWindows = GetNeighbourWindows();
            int NumberOfWindowsPerWall = existingWindows.Length;

            DeletePreviousWindows(existingWindows);
            FindObjectOfType<Procedural_generation>().PlaceWindows(this.transform, newScale.x);

            float wallWidth = transform.localScale.x;

            float WallWidthExceeds = LastWallWidth / NumberOfWindowsPerWall;
            
            if (Mathf.RoundToInt(WallWidthExceeds) == Mathf.RoundToInt(wallWidth - LastWallWidth))
            {
                FindObjectOfType<Procedural_generation>().windowsPerWall += 1;
                LastWallWidth = wallWidth;
            }
        }

        // ------------------------width decrease + window decrease--------------------------------------- //


        //float wallWidth = transform.localScale.x;

        //float WallWidthExceeds = LastWallWidth / NumberOfWindowsPerWall;

        //if (Mathf.RoundToInt(LastWallWidth) == Mathf.RoundToInt(wallWidth - WallWidthExceeds))
        //{
        //    FindObjectOfType<Procedural_generation>().windowsPerWall -= 1;
        //    LastWallWidth = wallWidth;
        //}


        // ------------------------width decrease + window decrease--------------------------------------- //



        //if (currentInteractor.name.ToLower().Contains("left") && LeftGrabScaleDownWidth.action.IsPressed())
        //{
        //    // Scale only height (Y-axis)
        //    direction = -1f;
        //    newScale.y += direction * scaleAmount;
        //    newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
        //}

        transform.localScale = newScale;
    }


    private void DeletePreviousWindows(Transform[] NeighbourWindows)
    {
        foreach (Transform t in NeighbourWindows)
        {
            Destroy(t.gameObject);
        }
    }

}
