using UnityEngine;
using UnityEngine.UI;

public class Scale_windows : MonoBehaviour
{
    public float scaleAmount = 0.01f;
    public ProportionalOrManualScaling ProportionalScalingOrManualScaling;

    private bool isSelected = false;
    private static Scale_windows currentSelectedWindow;

    public GameObject ToggleCanvas;

    private void Update()
    {
        HandleSelection();

        if (!isSelected) return;

        bool scaled = false;

        // Scale using mouse scroll

        // Scale width with Right Mouse Button + Mouse X
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            // Right mouse = width scaling
            if (Input.GetMouseButton(1))
            {
                ScaleWindowWidth(scroll * 100);
                scaled = true;
            }
            // Left mouse = height scaling
            else if (Input.GetMouseButton(0))
            {
                ScaleWindowHeight(scroll * 100);
                scaled = true;
            }
        }




        // Proportional scaling to neighbors
        if (scaled && ProportionalScalingOrManualScaling.ProportionalScalingWindow)
        {
            bool scaleWidth = Input.GetMouseButton(1);  // Right mouse
            bool scaleHeight = Input.GetMouseButton(0); // Left mouse
            RescaleNeighbourWindow(GetNeighbourWindows(), scaleWidth, scaleHeight);
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) // Left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    if (currentSelectedWindow != null)
                        currentSelectedWindow.Deselect();

                    isSelected = true;
                    currentSelectedWindow = this;
                    //GetComponent<Renderer>().material.color = Color.yellow;
                }
                else if (currentSelectedWindow == this)
                {
                    Deselect();
                }
            }
            else if (currentSelectedWindow == this)
            {
                Deselect();
            }
        }
    }

    private void Deselect()
    {
        isSelected = false;
        currentSelectedWindow = null;
        //GetComponent<Renderer>().material.color = Color.white;
    }

    private void ScaleWindowHeight(float delta)
    {
        Vector3 newScale = transform.localScale;
        newScale.y += delta * scaleAmount;
        newScale.y = Mathf.Max(newScale.y, 0.1f); // Minimum height
        transform.localScale = newScale;
        //transform.GetComponent<WindowEdgeDistanceDisplay>().UpdateCanvasAndLines();
    }

    private void ScaleWindowWidth(float delta)
    {
        Vector3 newScale = transform.localScale;
        newScale.x += delta * scaleAmount;
        newScale.x = Mathf.Max(newScale.x, 0.1f); // Minimum width
        transform.localScale = newScale;
        transform.GetComponent<WindowEdgeDistanceDisplay>().UpdateCanvasAndLines();
    }

    private Transform[] GetNeighbourWindows()
    {
        Transform parent = transform.parent;
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

    private void RescaleNeighbourWindow(Transform[] NeighbourWindows, bool scaleWidth, bool scaleHeight)
    {
        foreach (Transform t in NeighbourWindows)
        {
            Vector3 newScale = t.localScale;

            if (scaleWidth)
                newScale.x = Mathf.Max(transform.localScale.x, 0.1f);

            if (scaleHeight)
                newScale.y = Mathf.Max(transform.localScale.y, 0.1f);

            t.localScale = newScale;
        }
    }


    public void ToggleCanvasFunction()
    {
        ToggleCanvas.SetActive(!ToggleCanvas.activeSelf);
    }
}









//public class Scale_windows : MonoBehaviour
//{
//    public XRBaseInteractor currentInteractor;
//    public float scaleAmount = 0.001f;

//    [SerializeField] public InputActionProperty LeftTriggerScaleUpWidth;
//    [SerializeField] public InputActionProperty RightTriggerScaleUpHeight;
//    [SerializeField] public InputActionProperty LeftGrabScaleDownWidth;
//    [SerializeField] public InputActionProperty RightGrabScaleDownHeight;

//    public ProportionalOrManualScaling ProportionalScalingOrManualScaling;

//    //private void OnEnable()
//    //{
//    //    XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
//    //    interactable.hoverEntered.AddListener(OnHoverEnter);
//    //    interactable.hoverExited.AddListener(OnHoverExit);
//    //}

//    //private void OnDisable()
//    //{
//    //    XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
//    //    interactable.hoverEntered.RemoveListener(OnHoverEnter);
//    //    interactable.hoverExited.RemoveListener(OnHoverExit);
//    //}

//    public void OnHoverEnter(HoverEnterEventArgs args)
//    {
//        currentInteractor = args.interactor;
//    }

//    public void OnHoverExit(HoverExitEventArgs args)
//    {
//        currentInteractor = null;
//    }

//    private void Update()
//    {
//        if (currentInteractor == null) return;

//        string interactorName = currentInteractor.name.ToLower();

//        // Right hand: Trigger -> increase height, Grab -> decrease height
//        if (interactorName.Contains("right"))
//        {
//            if (RightTriggerScaleUpHeight.action.IsPressed())
//            {
//                ScaleWindow(true);  // Increase height
//                if (ProportionalScalingOrManualScaling.ProportionalScalingWindow)
//                {
//                    RescaleNeighbourWindow(true, GetNeighbourWindows());
//                }
//            }
//            else if (RightGrabScaleDownHeight.action.IsPressed())
//            {
//                ScaleWindow(false);// Decrease height
//                if (ProportionalScalingOrManualScaling.ProportionalScalingWindow)
//                {
//                    RescaleNeighbourWindow(false, GetNeighbourWindows());
//                }
//            }
//        }

//        // Left hand: Trigger -> increase width, Grab -> decrease width
//        else if (interactorName.Contains("left"))
//        {
//            if (LeftTriggerScaleUpWidth.action.IsPressed())
//            {
//                ScaleWindow(true);  // Increase width
//                if (ProportionalScalingOrManualScaling.ProportionalScalingWindow)
//                {
//                    RescaleNeighbourWindow(true, GetNeighbourWindows());
//                }
//            }
//            else if (LeftGrabScaleDownWidth.action.IsPressed())
//            {
//                ScaleWindow(false); // Decrease width
//                if (ProportionalScalingOrManualScaling.ProportionalScalingWindow)
//                {
//                    RescaleNeighbourWindow(false, GetNeighbourWindows());
//                }
//            }
//        }

//    }
//    private Transform[] GetNeighbourWindows()
//    {
//        Transform parent = transform.parent;
//        if (parent == null) return new Transform[0];

//        var neighbours = new System.Collections.Generic.List<Transform>();

//        foreach (Transform child in parent)
//        {
//            if (child != this.transform && child.CompareTag("Window"))
//            {
//                neighbours.Add(child);
//            }
//        }

//        return neighbours.ToArray();
//    }


//    //private void ScaleWindow(bool scaleUp)
//    //{
//    //    float direction = scaleUp ? 1f : -1f;
//    //    Vector3 newScale = transform.localScale + Vector3.one * direction * scaleAmount;

//    //    // Clamp to minimum scale
//    //    newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
//    //    transform.localScale = newScale;
//    //}

//    private void ScaleWindow(bool scaleUp)
//    {
//        float direction = scaleUp ? 1f : -1f;

//        Vector3 newScale = transform.localScale;

//        if (currentInteractor.name.ToLower().Contains("right"))
//        {
//            // Scale only height (Y-axis)
//            newScale.y += direction * scaleAmount;
//            newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
//        }
//        else if (currentInteractor.name.ToLower().Contains("left"))
//        {
//            // Scale only width (X-axis)
//            newScale.x += direction * scaleAmount;
//            newScale.x = Mathf.Max(newScale.x, 0.1f); // Clamp to min width
//        }
//        //if (currentInteractor.name.ToLower().Contains("left") && LeftGrabScaleDownWidth.action.IsPressed())
//        //{
//        //    // Scale only height (Y-axis)
//        //    direction = -1f;
//        //    newScale.y += direction * scaleAmount;
//        //    newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
//        //}

//        transform.localScale = newScale;
//    }


//    private void RescaleNeighbourWindow(bool scaleUp, Transform[] NeighbourWindows)
//    {
//        float direction = scaleUp ? 1f : -1f;

//        foreach (Transform t in NeighbourWindows)
//        {
//            //if((t.transform.position - transform.position).magnitude > 0.2f)
//            //{ 
//            Vector3 newScale = t.transform.localScale;

//            if (currentInteractor.name.ToLower().Contains("right"))
//            {
//                // Scale only height (Y-axis)
//                newScale.y += direction * scaleAmount;
//                newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
//            }
//            else if (currentInteractor.name.ToLower().Contains("left"))
//            {
//                // Scale only width (X-axis)
//                newScale.x += direction * scaleAmount;
//                newScale.x = Mathf.Max(newScale.x, 0.1f); // Clamp to min width
//            }
//            t.transform.localScale = newScale;

//            if (transform.gameObject.GetComponent<Renderer>().bounds.Intersects(t.transform.gameObject.GetComponent<Renderer>().bounds))
//            {
//                t.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
//            }
//            else if (!(transform.gameObject.GetComponent<Renderer>().bounds.Intersects(t.transform.gameObject.GetComponent<Renderer>().bounds)))
//            {
//                t.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
//            }

//            //}

//            if (Vector3.Distance(transform.position, t.transform.position) < 0.2f)
//            {
//                t.gameObject.GetComponent<Material>().color = Color.red;
//            }

//        }
//    }

//}
