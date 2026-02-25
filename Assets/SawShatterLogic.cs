using UnityEngine;
using System.Collections;

public class SawShatterLogic : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject brokenSawPrefab;
    public GameObject sparksVFX;

    [Header("Timing")]
    public float delayBeforeShatter = 5f;

    [Header("Explosion Settings")]
    public float explosionForce = 800f;
    public float explosionRadius = 3f;
    public float upwardModifier = 0.5f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Log")) return;

        triggered = true;

        StartCoroutine(ShatterSequence());
    }

    IEnumerator ShatterSequence()
    {
        // spawn sparks
        if (sparksVFX != null)
        {
            GameObject fx = Instantiate(
                sparksVFX,
                transform.position,
                Quaternion.identity
            );

            Destroy(fx, delayBeforeShatter);
        }

        // wait before breaking
        yield return new WaitForSeconds(delayBeforeShatter);

        // spawn broken pieces
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

        GameObject shards = Instantiate(
            brokenSawPrefab,
            spawnPos,
            transform.rotation
        );

        Rigidbody[] rbs = shards.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            if (rb.name.Contains("Shard1"))
            {
                Debug.Log("Applying explosion force to " + rb.name);
                rb.isKinematic = false;

                rb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    explosionRadius,
                    upwardModifier,
                    ForceMode.Impulse
                );
            }
            else
            {
                rb.isKinematic = true;

                rb.AddTorque(
                    Random.onUnitSphere * 40f,
                    ForceMode.Impulse
                );

            }
            // print shard1 coordinates after explosin
            Debug.Log(
                rb.name + " position after explosion: " + rb.transform.position
            );
            Debug.Log("Applied force to " + rb.name);
        }

        // hide original saw
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
