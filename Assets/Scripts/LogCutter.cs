
using UnityEngine;
using EzySlice;

public class LogCutter : MonoBehaviour
{
    [Header("References")]
    public Material crossSectionMaterial;
    public Material cutPreviewMaterial; // a simple transparent material to show cut progress

    public ParticleSystem woodChopEffect; // Drag your particle system here

    public AudioClip woodChopSound; // Sound to play when cutting starts
    public AudioSource audioSource;

    [Header("Settings")]
    public float separationForce = 3f;

    private GameObject currentLog;
    private bool sawInsideLog = false;
    private Vector3 entryPoint;         // where saw entered the log
    private Vector3 cutPlaneNormal;     // the cut direction (always horizontal = Vector3.right)
    private GameObject cutPreviewPlane; // visual slice preview

    // ── ENTRY ────────────────────────────────────────────────────
    void Start()
    {
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = woodChopSound;
        audioSource.loop = true;

        if (woodChopEffect != null) woodChopEffect.Stop();
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Log")) return;

        if (audioSource != null && woodChopSound != null )
        {
            audioSource.clip = woodChopSound;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }

        if(woodChopEffect != null)
        {
            woodChopEffect.Play();
            
            woodChopEffect.emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 20) 
            });

        }else
        {
            Debug.LogWarning("WoodChopEffect object not found in scene!");
        }

        currentLog = other.gameObject;
        sawInsideLog = true;
        entryPoint = transform.position;

        cutPlaneNormal = Vector3.forward;

    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Log")) return;
        if (currentLog == null) return;

        Bounds logBounds = currentLog.GetComponent<Renderer>().bounds;
        Collider sawCollider = GetComponent<Collider>();
        float sawX = sawCollider.bounds.max.x;

        // Debug.Log("Saw X: " + sawX + " | Log X range: " + logBounds.min.x + " to " + logBounds.max.x);
    
        float progress = Mathf.Clamp01(
          ((sawX-0.1f) - logBounds.min.x) / logBounds.size.x
        );

        Debug.Log("Cut progress: " + Mathf.Round(progress * 100f) + "%");

        if (cutPreviewPlane != null)
            cutPreviewPlane.transform.position = new Vector3(sawX, logBounds.center.y,logBounds.center.z);

        if (progress >= 1f)
        {
            audioSource.Stop();
            audioSource.clip=null;
            woodChopEffect.Stop();
            Debug.Log("Cut complete!");
            ExecuteCut(currentLog);
        }
    }

    // ── EXIT (fallback if dragged out sideways) ───────────────────
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Log")) return;
        
        woodChopEffect.Stop();
        audioSource.Stop();
        audioSource.clip=null;


        // Only cut on exit if saw actually passed through enough
        if (sawInsideLog && currentLog != null)
        {
            Bounds logBounds = currentLog.GetComponent<Renderer>().bounds;
            float halfExtent = Vector3.Dot(logBounds.extents, new Vector3(
                Mathf.Abs(cutPlaneNormal.x),
                0f,
                Mathf.Abs(cutPlaneNormal.z)
            ));

            float logCenterDist = Vector3.Dot(
                currentLog.transform.position - transform.position,
                cutPlaneNormal
            );

            float progress = 1f - Mathf.Clamp01((logCenterDist + halfExtent) / (halfExtent * 2f));

            if (progress >= 0.8f)
            {
                Debug.Log("Saw exited after 80%+ — completing cut!");
                ExecuteCut(currentLog);
            }
            else
            {
                Debug.Log("Log pulled away too early — no cut.");
                CleanupPreview();
                sawInsideLog = false;
                currentLog = null;
            }
        }
    }

    // ── EXECUTE THE ACTUAL SLICE ──────────────────────────────────
    void ExecuteCut(GameObject log)
    {
        sawInsideLog = false;
        CleanupPreview();

        Bounds bounds = log.GetComponent<Renderer>().bounds;

        // Cut point must be inside the mesh — use log center
        // but place it at the saw's X position (where the blade actually is)

        Vector3 cutPoint = new Vector3(
            bounds.center.x,
            bounds.center.y,
            Mathf.Clamp(transform.position.z, bounds.min.z + 0.01f, bounds.max.z - 0.01f)
        );

        Debug.Log("Cutting at: " + cutPoint + " | Log bounds: " + bounds.min + " to " + bounds.max);

        SlicedHull hull = log.Slice(cutPoint, cutPlaneNormal, crossSectionMaterial);

        if (hull == null)
        {
            Debug.LogWarning("Slice failed — cut point was: " + cutPoint);
            currentLog = null;
            return;
        }

        GameObject upperPiece = hull.CreateUpperHull(log, crossSectionMaterial);
        GameObject lowerPiece = hull.CreateLowerHull(log, crossSectionMaterial);

    
        upperPiece.name = log.name + "_Upper";
        lowerPiece.name = log.name + "_Lower";

        upperPiece.tag = "Grabbable";
        lowerPiece.tag = "Grabbable";

        

        SetupPiece(upperPiece, log);
        SetupPiece(lowerPiece, log);

        //make the  force (forward left and back left) to make the pieces fly apart
        Vector3 forceDirection1 = new Vector3(-0.5f, 0f, 1f); // forward
        Vector3 forceDirection2 = new Vector3(-0.5f, 0f, -1f); // back
        

        upperPiece.GetComponent<Rigidbody>().AddForce(forceDirection1* separationForce, ForceMode.Impulse);
        lowerPiece.GetComponent<Rigidbody>().AddForce(forceDirection2 * separationForce, ForceMode.Impulse);

        Destroy(log);
        currentLog = null;
    }


    void CleanupPreview()
    {
        if (cutPreviewPlane != null)
        {
            Destroy(cutPreviewPlane);
            cutPreviewPlane = null;
        }
    }

    // ── SETUP PIECES ──────────────────────────────────────────────
    void SetupPiece(GameObject piece, GameObject originalLog)
    {
        piece.transform.position = originalLog.transform.position;
        piece.transform.rotation = originalLog.transform.rotation;
        piece.transform.localScale = originalLog.transform.localScale;

        MeshCollider col = piece.AddComponent<MeshCollider>();
        col.convex = true;

        Rigidbody rb = piece.AddComponent<Rigidbody>();
        rb.useGravity = true;
    }
}
