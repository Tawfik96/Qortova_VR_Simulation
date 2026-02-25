using UnityEngine;
using EzySlice;
using System.Collections.Generic;

public class Slicing : MonoBehaviour
{
    public GameObject objectToShatter;
    public Material crossSectionMaterial;

    [Header("Shatter Settings")]
    public KeyCode shatterKey = KeyCode.Return;
    public int targetPieceCount = 40;
    public float shardScale = 1f;

    [Header("Explosion Settings")]
    public float explosionForce = 5f;
    public float explosionRadius = 2f;
    public float upwardModifier = 1f;
    public float torqueForce = 3f;

    [Header("Shard Launch Settings")]
    public int shardsToLaunch = 2;        // how many shards fly at camera
    public float launchForce = 4f;       // how fast they fly

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(shatterKey) && objectToShatter != null)
        {
            Shatter();
        }
    }

    public void Shatter()
    {
        if (objectToShatter == null) return;

        List<GameObject> pieces = new List<GameObject> { objectToShatter };

        int passes = Mathf.CeilToInt(Mathf.Log(targetPieceCount, 2));

        for (int i = 0; i < passes; i++)
        {
            List<GameObject> newPieces = new List<GameObject>();

            foreach (GameObject piece in pieces)
            {
                List<GameObject> sliced = SlicePiece(piece);
                if (sliced != null && sliced.Count == 2)
                {
                    newPieces.AddRange(sliced);
                    Destroy(piece);
                }
                else
                {
                    newPieces.Add(piece);
                }
            }

            pieces = newPieces;
        }

        Vector3 explosionCenter = objectToShatter != null
            ? objectToShatter.transform.position
            : pieces[0].transform.position;

        foreach (GameObject piece in pieces)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb == null) rb = piece.AddComponent<Rigidbody>();

            rb.useGravity = true;

            rb.AddExplosionForce(
                explosionForce * Random.Range(0.6f, 1.4f),
                explosionCenter,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );

            rb.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);
        }

        // ── LAUNCH SHARDS AT CAMERA ──────────────────────────────
        // Shuffle the pieces list so selection is random
        for (int i = pieces.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (pieces[i], pieces[rand]) = (pieces[rand], pieces[i]);
        }

        int count = Mathf.Min(shardsToLaunch, pieces.Count);
        for (int i = 0; i < count; i++)
        {
            GameObject shard = pieces[i];
            if (shard == null) continue;

            Rigidbody rb = shard.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // Override the explosion velocity with a direct shot at the camera
            rb.useGravity = false;
            Vector3 direction = (mainCamera.transform.position - shard.transform.position).normalized;
            rb.linearVelocity = direction * launchForce;
        }
        // ────────────────────────────────────────────────────────

        if (objectToShatter != null)
            Destroy(objectToShatter);
    }

    List<GameObject> SlicePiece(GameObject piece)
    {
        MeshRenderer renderer = piece.GetComponent<MeshRenderer>();
        if (renderer == null) return null;

        Vector3 normal = Random.onUnitSphere;

        Bounds bounds = renderer.bounds;
        Vector3 planePos = bounds.center + new Vector3(
            Random.Range(-bounds.extents.x * 0.6f, bounds.extents.x * 0.6f),
            Random.Range(-bounds.extents.y * 0.6f, bounds.extents.y * 0.6f),
            Random.Range(-bounds.extents.z * 0.6f, bounds.extents.z * 0.6f)
        );

        SlicedHull hull = piece.Slice(planePos, normal, crossSectionMaterial);

        if (hull == null) return null;

        GameObject upper = hull.CreateUpperHull(piece, crossSectionMaterial);
        GameObject lower = hull.CreateLowerHull(piece, crossSectionMaterial);

        if (upper == null || lower == null) return null;

        SetupShard(upper, piece.transform.position, piece.transform.rotation);
        SetupShard(lower, piece.transform.position, piece.transform.rotation);

        return new List<GameObject> { upper, lower };
    }

    void SetupShard(GameObject shard, Vector3 position, Quaternion rotation)
    {
        shard.transform.position = position;
        shard.transform.rotation = rotation;
        shard.transform.localScale = Vector3.one * shardScale;

        MeshCollider col = shard.AddComponent<MeshCollider>();
        col.convex = true;
    }

    private void OnDrawGizmos()
    {
        if (objectToShatter == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        MeshRenderer r = objectToShatter.GetComponent<MeshRenderer>();
        if (r != null)
            Gizmos.DrawWireSphere(r.bounds.center, explosionRadius);
    }
}