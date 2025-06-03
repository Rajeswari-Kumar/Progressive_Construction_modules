using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Move_around_windows : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis movementAxis = Axis.X;

    public XRGrabInteractable grabInteractable;
    public Transform interactorTransform;
    public Vector3 initialGrabOffset;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactorTransform = args.interactorObject.transform;
        initialGrabOffset = transform.position - interactorTransform.position;
        Debug.Log("Grab");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactorTransform = null;
    }

    private void Update()
    {
        if (interactorTransform == null) return;

        Vector3 targetPosition = interactorTransform.position + initialGrabOffset;
        Vector3 currentPosition = transform.position;

        Vector3 newPosition = currentPosition;

        switch (movementAxis)
        {
            case Axis.X:
                newPosition.x = targetPosition.x;
                break;
            case Axis.Y:
                newPosition.y = targetPosition.y;
                break;
            case Axis.Z:
                newPosition.z = targetPosition.z;
                break;
        }

        transform.position = newPosition;
    }
}
