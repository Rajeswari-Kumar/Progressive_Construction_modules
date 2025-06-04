using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Move_around_windows : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis movementAxis = Axis.X;

    public XRGrabInteractable grabInteractable;

    [SerializeField] public InputActionProperty moveAction; // From controller
    [SerializeField] private float moveSpeed = 0.5f;

    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void Update()
    {
        if (!isGrabbed) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        if (input == Vector2.zero) return;

        Vector3 moveDirection = Vector3.zero;

        if(input.x > 0 || input.x < 0)
            moveDirection = new Vector3( 0, 0, input.x);
        if (input.y > 0 || input.y < 0)
            moveDirection = new Vector3(0,input.y, 0);
        // Choose axis
        //switch (movementAxis)
        //{
        //    case Axis.X:
        //        moveDirection = new Vector3(input.x, 0, 0);
        //        break;
        //    case Axis.Y:
        //        moveDirection = new Vector3(0, input.y, 0);
        //        break;
        //    case Axis.Z:
        //        moveDirection = new Vector3(0, 0, input.y); // forward/back
        //        break;
        //}

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
