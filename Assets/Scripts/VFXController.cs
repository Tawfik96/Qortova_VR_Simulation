using UnityEngine;
using System.Collections;

public class VFXController : MonoBehaviour
{
    public ParticleSystem smoke; 
    public ParticleSystem sparks;
    public FlickLighto lights;

    public AudioSource electricitySource;
    public AudioSource screamSource;

    public AudioClip ElectricitySound;
    public AudioClip ScreamSound;

    public AudioClip BombSound;

    public float duration = 10f; // Duration for which the VFX and sounds will play

    void Start()
    {
        smoke?.Stop();
        sparks?.Stop();
    }

    public void TriggerMachineStartVFX()
    {
        smoke?.Play();
        sparks?.Play();
        lights?.StartFlickering();

        // Play both sounds at the same time
        if (electricitySource != null && ElectricitySound != null)
        {
            electricitySource.clip = ElectricitySound;
            electricitySource.Play();
        }

        if (screamSource != null && ScreamSound != null)
        {
            screamSource.clip = ScreamSound;
            screamSource.loop = true;
            screamSource.Play();
        }

        // Stop both after 10 seconds
        StartCoroutine(stopVFXAfter(duration));

        Debug.Log("VFX Started!");
    }

    private IEnumerator stopVFXAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // smoke?.Stop();
        sparks?.Stop();
        lights?.StopFlickering();

        if (electricitySource != null) electricitySource.Stop();
        if (screamSource != null) screamSource.Stop();

        Debug.Log("All effextes stopped after " + seconds + " seconds.");
    }


    public void TriggerBombVFX()
    {
        lights?.BombFlicker();

        if (sparks != null)
            sparks.Emit(200);

        if (smoke != null)
            smoke.Emit(100);

        if (BombSound != null)
        {
            // Create temporary GameObject
            GameObject tempAudio = new GameObject("TempBombAudio");
            tempAudio.transform.position = transform.position;

            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = BombSound;
            tempSource.spatialBlend = 1f; // 3D sound (0 = 2D, 1 = 3D)
            tempSource.Play();

            Destroy(tempAudio, BombSound.length);
        }
    }
    
}
