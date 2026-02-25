using UnityEngine;
using UnityEngine.InputSystem;


//This script handles picking up and wearing the glasses. It should be attached to the Player (Camera) object.

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Transform wornItemAnchor;
    public GameObject glassesOverlayPrefab;

    public Camera mainCamera;

    [Header("Settings")]
    public float interactRange = 3f;
    public Vector3 wornLocalPosition = new Vector3(0f, -0.1f, 0.11f);
    public Vector3 wornLocalRotation = new Vector3(270f, 0f, 0f);

    private GameObject equippedGlasses;
    public bool glassesEquipped = false;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !glassesEquipped)
            TryPickUp();

  
    }

    void LateUpdate()
    {
        if (glassesEquipped)
            UpdateGlassesPosition();
    }
    void UpdateGlassesPosition()
    {
        Transform cam = Camera.main.transform;

        // Always place glasses directly in front of camera
        // regardless of which direction camera is facing
        equippedGlasses.transform.position = cam.position
            + cam.forward * wornLocalPosition.z
            + cam.up * wornLocalPosition.y
            + cam.right * wornLocalPosition.x;

        // Match camera rotation exactly, then apply offset
        equippedGlasses.transform.rotation = cam.rotation *
            Quaternion.Euler(wornLocalRotation);
    }

    void TryPickUp()
    {
        // Ray ray = new Ray(transform.position, transform.forward);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        int ignoreWornItems = ~(1 << LayerMask.NameToLayer("WornItems"));

        Debug.Log("Casting ray from: " + transform.position + " direction: " + transform.forward);
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, ignoreWornItems))
        {
            Debug.Log("Ray hit: " + hit.collider.gameObject.name + " | Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Pickable"))
                PickUpGlasses(hit.collider.gameObject);
            else
                Debug.Log("Object hit but tag is NOT Pickable. Tag is: " + hit.collider.tag);
        }
        else
        {
            Debug.Log("Raycast hit NOTHING. Check range or if you're looking at the glass.");
        }
    }
    void PickUpGlasses(GameObject worldGlasses)
    {
        worldGlasses.SetActive(false);

        // Instantiate at root level, NOT as child of anchor
        equippedGlasses = Instantiate(glassesOverlayPrefab);
        
        equippedGlasses.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        
        equippedGlasses.transform.localPosition = wornLocalPosition;
        equippedGlasses.transform.localEulerAngles = wornLocalRotation;

        SetLayerRecursively(equippedGlasses, LayerMask.NameToLayer("WornItems"));
        glassesEquipped = true;
    }
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}