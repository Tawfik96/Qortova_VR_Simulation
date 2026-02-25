using UnityEngine;
using EzySlice;
using System;

public class SliceObjectTwo : MonoBehaviour
{
    public GameObject objectToSlice;
    public Material crossSectionMaterial;
    public enum SliceAxis { X, Y, Z }
    public SliceAxis sliceAxis = SliceAxis.Y;

    [Header("Slicing Control")]
    [Range(-1f, 1f)]
    public float cutLocation = 0.0f; // -1 is bottom, 0 is center, 1 is top
    public KeyCode sliceKey = KeyCode.Return;
    public float shardScale = 1f;

    void Update()
    {
        if (Input.GetKeyDown(sliceKey) && objectToSlice != null)
        {
            SliceObject();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            separateHalves();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // Test: Log the plane position for debugging
            fullExplode();
        }
    }

    public void fullExplode()
    {
        GameObject upperHalf = GameObject.Find("UpperHalf");
        GameObject lowerHalf = GameObject.Find("LowerHalf");

        if (upperHalf != null && lowerHalf != null)
        {
             upperHalf.AddComponent<Rigidbody>(); 
             upperHalf.GetComponent<Rigidbody>().useGravity = true;

             lowerHalf.AddComponent<Rigidbody>(); 
             lowerHalf.GetComponent<Rigidbody>().useGravity = true;

             //make upper half explode upwards
             upperHalf.GetComponent<Rigidbody>().AddForce(Vector3.left * 3f, ForceMode.Impulse);
             lowerHalf.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    public void fullExplode(string distinguisher)
    {
        SliceObject();
        fullExplode();
    }

    public void separateHalves()
    {
        GameObject upperHalf = GameObject.Find("UpperHalf");
        GameObject lowerHalf = GameObject.Find("LowerHalf");

        if (upperHalf != null && lowerHalf != null)
        {
             upperHalf.AddComponent<Rigidbody>(); 
             upperHalf.GetComponent<Rigidbody>().useGravity = true;

             //make upper half explode upwards
             upperHalf.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    void SliceObject()
    {
        MeshRenderer renderer = objectToSlice.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        Vector3 normal = GetNormal();
        Vector3 planePosition = GetPlanePosition(renderer);

        // Use the extension method overload that takes world position + normal directly
        SlicedHull hull = objectToSlice.Slice(planePosition, normal, crossSectionMaterial);

        if (hull != null)
        {
            // Cache before destroying
            Vector3 originalPosition = objectToSlice.transform.position;
            Quaternion originalRotation = objectToSlice.transform.rotation;

            GameObject upper = hull.CreateUpperHull(objectToSlice, crossSectionMaterial);
            GameObject lower = hull.CreateLowerHull(objectToSlice, crossSectionMaterial);

            SetupHull(upper, "UpperHalf", originalPosition, originalRotation);
            SetupHull(lower, "LowerHalf", originalPosition, originalRotation);

            Destroy(objectToSlice);
        }
    }

    void SetupHull(GameObject obj, string name, Vector3 position, Quaternion rotation)
    {
        obj.name = name;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = Vector3.one * shardScale;
        obj.AddComponent<MeshCollider>().convex = true;
    }
    // Helper to calculate plane position based on bounds and slider
    
    Vector3 GetPlanePosition(MeshRenderer renderer)
    {
        Vector3 size = renderer.bounds.size;
        Vector3 center = renderer.bounds.center;
        Vector3 offset = Vector3.zero;

        if (sliceAxis == SliceAxis.X) offset = Vector3.right * (size.x / 2f) * cutLocation;
        if (sliceAxis == SliceAxis.Y) offset = Vector3.up * (size.y / 2f) * cutLocation;
        if (sliceAxis == SliceAxis.Z) offset = Vector3.forward * (size.z / 2f) * cutLocation;

        return center + offset;
    }

    Vector3 GetNormal()
    {
        if (sliceAxis == SliceAxis.X) return Vector3.right;
        if (sliceAxis == SliceAxis.Z) return Vector3.forward;
        return Vector3.up;
    }

    void SetupHull(GameObject obj, string name)
    {
        obj.name = name;
        obj.transform.position = objectToSlice.transform.position;
        obj.transform.localScale = Vector3.one * shardScale;
        obj.AddComponent<MeshCollider>().convex = true;
    }

    // --- GIZMO VISUALIZER ---
    private void OnDrawGizmos()
    {
        if (objectToSlice == null) return;

        MeshRenderer renderer = objectToSlice.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        Gizmos.color = Color.red;
        Vector3 pos = GetPlanePosition(renderer);
        Vector3 size = renderer.bounds.size;

        // Draw a wireframe box where the cut will happen
        Vector3 cubeSize = size;
        if (sliceAxis == SliceAxis.X) cubeSize.x *= 0.05f;
        if (sliceAxis == SliceAxis.Y) cubeSize.y *= 0.05f;
        if (sliceAxis == SliceAxis.Z) cubeSize.z *= 0.05f;

        Gizmos.DrawWireCube(pos, cubeSize);
        Gizmos.DrawSphere(pos, 0.05f);
    }
}

