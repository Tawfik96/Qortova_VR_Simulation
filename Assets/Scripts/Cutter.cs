// // using UnityEngine;
// // using EzySlice;
// // using System.Collections;

// // public class Cutter : MonoBehaviour
// // {
// //     public Material crossSectionMaterial;
// //     public float vibrationIntensity = 0.1f;
    
// //     [Header("Detection Settings")]
// //     public LayerMask sliceableLayer; 

// //     private GameObject currentLog;
// //     private Vector3 entryPoint;
// //     private bool isCutting = false;

// //     void OnTriggerEnter(Collider other)
// //     {
// //         // Only start cutting if the object is in the correct layer
// //         if (((1 << other.gameObject.layer) & sliceableLayer) != 0)
// //         {
// //             currentLog = other.gameObject;
// //             isCutting = true;
            
// //             // Add vibration for feedback
// //             if (currentLog.GetComponent<Vibration>() == null)
// //                 currentLog.AddComponent<Vibration>();
// //         }
// //     }

// //     void Update()
// //     {
// //         if (isCutting && currentLog != null)
// //         {
// //             HandleCuttingLogic();
// //         }
// //     }

// //     void HandleCuttingLogic()
// //     {
// //         // 1. Calculate how deep the saw is inside the log
// //         // We compare the saw position to the log's center relative to its size
// //         Bounds logBounds = currentLog.GetComponent<MeshRenderer>().bounds;
        
// //         // This calculates a 0 to 1 value based on penetration
// //         // We assume we are cutting along the Log's Y or X axis
// //         float distanceIntoLog = Vector3.Distance(transform.position, logBounds.min);
// //         float totalLogWidth = logBounds.size.y; // Change to .x or .z depending on log orientation
        
// //         float progress = Mathf.Clamp01(distanceIntoLog / totalLogWidth);

// //         // 2. Visual/Haptic Feedback
// //         var vib = currentLog.GetComponent<Vibration>();
// //         if (vib != null) vib.increaseRate = progress * 2f;

// //         // 3. Trigger the Slice once we've passed through 90% of the log
// //         if (progress > 0.9f)
// //         {
// //             FinishCut();
// //         }
// //     }

// //     void FinishCut()
// //     {
// //         isCutting = false;
        
// //         // Use the slice logic from your previous script
// //         // We slice exactly at the saw's current position
// //         EzySlice.Plane slicePlane = new EzySlice.Plane(transform.position, transform.up);
// //         SlicedHull hull = currentLog.Slice(slicePlane, crossSectionMaterial);

// //         if (hull != null)
// //         {
// //             GameObject upper = hull.CreateUpperHull(currentLog, crossSectionMaterial);
// //             GameObject lower = hull.CreateLowerHull(currentLog, crossSectionMaterial);

// //             // Add physics so the pieces fall away
// //             SetupPiece(upper);
// //             SetupPiece(lower);

// //             Destroy(currentLog);
// //         }
// //     }

// //     void SetupPiece(GameObject obj)
// //     {
// //         obj.AddComponent<MeshCollider>().convex = true;
// //         Rigidbody rb = obj.AddComponent<Rigidbody>();
// //         rb.AddExplosionForce(5f, transform.position, 2f);
// //     }

// //     void OnTriggerExit(Collider other)
// //     {
// //         if (other.gameObject == currentLog)
// //         {
// //             isCutting = false;
// //             currentLog = null;
// //         }
// //     }
// // }

// using UnityEngine;

// public class Cutter : MonoBehaviour
// {
//     [Tooltip("The axis along which logs are pushed into the blade")]
//     public enum FeedAxis { X, Y, Z }
//     public FeedAxis feedAxis = FeedAxis.Z;

//     [Tooltip("The plane the cut happens on — usually perpendicular to feed axis")]
//     public Vector3 CutNormal => feedAxis switch
//     {
//         FeedAxis.X => Vector3.right,
//         FeedAxis.Y => Vector3.up,
//         _ => Vector3.forward
//     };

//     public Vector3 BladeWorldPosition => transform.position;

//     private void OnTriggerEnter(Collider other)
//     {
//         LogCutter cutter = other.GetComponent<LogCutter>();
//         if (cutter != null)
//             cutter.OnEnterSaw(this);
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         LogCutter cutter = other.GetComponent<LogCutter>();
//         if (cutter != null)
//             cutter.OnExitSaw();
//     }
// }