// using UnityEngine;

// public class GlassCrack : MonoBehaviour
// {
//     public GameObject crackDecal; // drag CrackDecal here in prefab

//     public GameObject shardPrefab; // 

//     bool isCracked = false;
//     public KeyCode startKey = KeyCode.C;
//     public void Update()
//     {
//         if (Input.GetKeyDown(startKey))
//         {
//             isCracked = !isCracked;
//             crackDecal.SetActive(isCracked);
//         }
//     }

//     public void Crack()
//     {
//         isCracked = !isCracked;
//         crackDecal.SetActive(true);
//         shardPrefab.SetActive(true);
//     }
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Shard"))
//         {
//             crackDecal.SetActive(true);
//         }
//     }
// }

using System;
using UnityEngine;
using System.Collections;

public class GlassCrack : MonoBehaviour
{
    [Header("Visuals & Audio")]
    public GameObject crackDecal;

    public GameObject shardPrefab;

    public AudioClip crackSound;
    private AudioSource audioSource;

    public AudioSource person;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (crackDecal) crackDecal.SetActive(false);
        if (shardPrefab) shardPrefab.SetActive(false);
    }

    // This is ONLY called if the glasses are actually on the player
    public void Crack()
    {
        if (crackDecal) crackDecal.SetActive(true);
        if (shardPrefab) shardPrefab.SetActive(true);

        if (person != null)
            person.Play();
        
        //start delay (0.3f) coroutine for cracksound to happen 
            StartCoroutine(PlayCrackSoundWithDelay(0f));

        

        Debug.Log("Glasses took the hit! Player is safe.");
    }

    private IEnumerator PlayCrackSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (crackSound && audioSource)
            audioSource.PlayOneShot(crackSound);
        else if (crackSound)
            AudioSource.PlayClipAtPoint(crackSound, transform.position);
    }
        
}
