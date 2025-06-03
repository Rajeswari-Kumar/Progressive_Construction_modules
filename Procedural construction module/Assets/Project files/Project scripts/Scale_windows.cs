using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Scale_windows : MonoBehaviour
{
    public XRBaseInteractor currentInteractor;
    public float scaleAmount = 0.001f;

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
            ScaleWindow(true); // Scale up height
            //RescaleNeighbourWindow(false, GetNeighbourWindows());
        }
        else if (interactorName.Contains("left") && LeftTriggerScaleUpWidth.action.IsPressed())
        {
            ScaleWindow(true);// Scale down width
            RescaleNeighbourWindow(false, GetNeighbourWindows());
        }
        else if (RightTriggerScaleUpHeight.action.IsPressed() && LeftTriggerScaleUpWidth.action.IsPressed())
        {
            //ScaleWindow(false);
        }
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


    //private void ScaleWindow(bool scaleUp)
    //{
    //    float direction = scaleUp ? 1f : -1f;
    //    Vector3 newScale = transform.localScale + Vector3.one * direction * scaleAmount;

    //    // Clamp to minimum scale
    //    newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
    //    transform.localScale = newScale;
    //}

    private void ScaleWindow(bool scaleUp)
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
        }
        //if (currentInteractor.name.ToLower().Contains("left") && LeftGrabScaleDownWidth.action.IsPressed())
        //{
        //    // Scale only height (Y-axis)
        //    direction = -1f;
        //    newScale.y += direction * scaleAmount;
        //    newScale.y = Mathf.Max(newScale.y, 0.1f); // Clamp to min height
        //}

        transform.localScale = newScale;
    }


    private void RescaleNeighbourWindow(bool scaleUp, Transform[] NeighbourWindows)
    {
        float direction = scaleUp ? 1f : -1f;
        
        foreach (Transform t in NeighbourWindows)
        {
            if((t.transform.position - transform.position).magnitude > 0.2f)
            { 
                Vector3 newScale = t.transform.localScale + Vector3.one * direction * scaleAmount;

            // Clamp to minimum scale
                newScale = Vector3.Max(newScale, Vector3.one * 0.1f);

                t.transform.localScale = newScale;
                if(transform.gameObject.GetComponent<Renderer>().bounds.Intersects(t.transform.gameObject.GetComponent<Renderer>().bounds))
                {
                    t.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
                else if(!(transform.gameObject.GetComponent<Renderer>().bounds.Intersects(t.transform.gameObject.GetComponent<Renderer>().bounds)))
                {
                    t.transform.gameObject.GetComponent<Renderer>().material.color = Color.green;
                }

            }

            if(Vector3.Distance(transform.position,t.transform.position) < 0.2f)
            {
                t.gameObject.GetComponent<Material>().color = Color.red;
                Debug.Log("Too close");
            }
            
        }
    }

}
