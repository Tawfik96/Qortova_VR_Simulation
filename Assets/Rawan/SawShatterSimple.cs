using UnityEngine;
using System.Collections;

public class SawShatterSimple : MonoBehaviour
{
    public GameObject shard1;
    public GameObject shard2;

    [Header("Trigger manually")]
    public bool triggered = false; // Set this to true to spawn shards

    private bool hasShattered = false; // Prevent multiple triggers

    public float shardScale = 0.0037f; // Scale for the spawned shards

    void Start()
    {
        Debug.Log("Saw script initialized on " + gameObject.name);
    }

    void Update()
    {
        // Check if we should trigger
        if (triggered && !hasShattered)
        {
            Debug.Log("Triggered via public bool → spawning shards now.");
            hasShattered = true;
            SpawnShards();
        }
    }

    private void SpawnShards()
    {
        Vector3 sawPos = transform.position;
        Quaternion sawRot = transform.rotation;

        // Spawn Shard1 (falling)
        if (shard1 != null)
        {
            GameObject s1 = Instantiate(shard1, sawPos, sawRot);
            s1.transform.localScale = new Vector3(shardScale,shardScale,shardScale);
            Rigidbody rb1 = s1.GetComponent<Rigidbody>();
            if (rb1 != null)
            {
                rb1.isKinematic = false; // Gravity enabled
            }
            else
            {
                Debug.LogWarning("Shard1 has no Rigidbody!");
            }
            Debug.Log("Shard1 spawned at saw position.");
        }

        // Spawn Shard2 (spinning)
        if (shard2 != null)
        {
            GameObject s2 = Instantiate(shard2, sawPos, sawRot);
            s2.transform.localScale = new Vector3(shardScale,shardScale,shardScale);
            Rigidbody rb2 = s2.GetComponent<Rigidbody>();
            if (rb2 != null)
            {
                rb2.isKinematic = true; // Floating
            }

            RotateSAW rot = s2.GetComponent<RotateSAW>();
            if (rot == null)
            {
                s2.AddComponent<RotateSAW>();
                Debug.Log("RotateSAW script added to Shard2.");
            }
            Debug.Log("Shard2 spawned at saw position.");
        }
    }
}
