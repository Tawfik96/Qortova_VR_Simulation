using UnityEngine;
using UnityEngine.InputSystem;

public class MouseDrag : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;

    // --- Configuration ---
    [Header("Settings")]
    [Tooltip("If true: Moves Up, Down, Left, Right.\nIf false: Moves ONLY Up and Down.")]
    public bool allowVerical = false; 

    // --- State Variables ---
    private float mZCoord; // Actual distance from camera to object
    private Vector3 mOffset;
    private float fixedZ; // The World Z position to lock to
    private float fixedY; // The World X position to lock to (if horizontal is disabled)

    void Awake() => mainCamera = Camera.main;

    void Update()
    {
        // 1. Start Grab
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGrab();
        }

        // 2. Dragging
        if (grabbedObject != null && Mouse.current.leftButton.isPressed)
        {
            DragObject();
        }

        // 3. Release
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseGrab();
        }
    }

    void StartGrab()
    {
        int dragMask = ~(1 << LayerMask.NameToLayer("WornItems"));
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());


        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, dragMask))
        {
            if (hit.collider.CompareTag("Grabbable")  || hit.collider.CompareTag("Log"))
            {
                grabbedObject = hit.collider.gameObject;
                grabbedRb = grabbedObject.GetComponent<Rigidbody>();

                // --- 1. Fix the Z Distance Math ---
                // Calculate the true distance from the camera to the object's plane
                mZCoord = mainCamera.WorldToScreenPoint(grabbedObject.transform.position).z;

                // Store initial positions to lock them later
                fixedZ = grabbedObject.transform.position.z;
                fixedY = grabbedObject.transform.position.y; 

                // Calculate offset vector
                mOffset = grabbedObject.transform.position - GetMouseWorldPos();

                // Physics setup
                grabbedRb.useGravity = false;
                grabbedRb.linearDamping = 10f; 
            }
        }
    }

    void DragObject()
    {
        Vector3 currentMousePos = GetMouseWorldPos();
        Vector3 targetPos = currentMousePos + mOffset;

        // --- 2. Apply Axis Constraints ---
        
        // Always lock Depth (Z)
        targetPos.z = fixedZ;

        // Conditional Lock Horizontal (X)
        if (!allowVerical)
        {
            targetPos.y = fixedY; // Force X to stay where it was when we grabbed it
        }

        grabbedRb.MovePosition(targetPos);
    }

    void ReleaseGrab()
    {
        if (grabbedRb != null)
        {
            grabbedRb.useGravity = true;
            grabbedRb.linearDamping = 0f;
        }
        grabbedObject = null;
        grabbedRb = null;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        // Use mZCoord (Distance from camera) instead of the World Z position
        return mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mZCoord));
    }
}
