

using UnityEngine;
using System.Collections;

public class SawLogBreak : MonoBehaviour
{
    private Vibration sawMachineVib;
    private Vibration logVib;

    [Header("Sound Settings")]
    public AudioClip vibrationSound;
    public AudioClip explosionSound;

    private AudioSource audioSource;


    [Header("Effect Settings")]
    public ParticleSystem woodChipsEffect; // Drag your particle system here
    public GameObject explosionEffectPrefab; // Optional: Spawn a prefab for the boom

    private GlassCrack glass_crack;

    private GlassPicker glassPicker;


    [Header("Cutting Settings")]
    public float timeToBreak = 3.0f;
    public float jitterGracePeriod = 0.2f;

    private Coroutine cuttingCoroutine;
    private Coroutine gracePeriodCoroutine;
    private bool isCutting = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = vibrationSound;
        audioSource.loop = true;

        if (transform.parent != null && transform.parent.parent != null)
        {
            sawMachineVib = transform.parent.parent.GetComponent<Vibration>();
        }

        if (woodChipsEffect != null) woodChipsEffect.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        logVib = other.GetComponent<Vibration>();
        if (logVib != null)
        {
            if (gracePeriodCoroutine != null)
            {
                StopCoroutine(gracePeriodCoroutine);
                gracePeriodCoroutine = null;
            }
            else if (!isCutting)
            {
                cuttingCoroutine = StartCoroutine(TrackCuttingProgress());

                // --- AUDIO & EFFECTS: Start ---
                if (vibrationSound != null && !audioSource.isPlaying) audioSource.Play();

                if (woodChipsEffect != null && !woodChipsEffect.isPlaying)
                {
                    woodChipsEffect.gameObject.SetActive(true);
                    woodChipsEffect.Play();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (sawMachineVib != null) sawMachineVib.StartVibration();
        Vibration currentLog = other.GetComponent<Vibration>();
        if (currentLog != null) currentLog.StartVibration();
    }

    private void OnTriggerExit(Collider other)
    {
        if (sawMachineVib != null) sawMachineVib.StopVibration();
        Vibration exitingLog = other.GetComponent<Vibration>();
        if (exitingLog != null)
        {
            if (gracePeriodCoroutine != null) StopCoroutine(gracePeriodCoroutine);
            gracePeriodCoroutine = StartCoroutine(WaitToReset(exitingLog));
        }
    }

    private IEnumerator TrackCuttingProgress()
    {
        isCutting = true;
        float elapsed = 0;

        while (elapsed < timeToBreak)
        {
            if (gracePeriodCoroutine == null)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        TriggerGlobalExplosion();
        isCutting = false;
        cuttingCoroutine = null;
    }

    private IEnumerator WaitToReset(Vibration log)
    {
        yield return new WaitForSeconds(jitterGracePeriod);

        // --- AUDIO & EFFECTS: Stop (Reset actually happened) ---
        if (audioSource.isPlaying) audioSource.Stop();
        if (woodChipsEffect != null) woodChipsEffect.Stop();

        if (cuttingCoroutine != null)
        {
            StopCoroutine(cuttingCoroutine);
            cuttingCoroutine = null;
        }

        isCutting = false;
        if (sawMachineVib != null) sawMachineVib.isVibrating = false;
        if (log != null) log.isVibrating = false;

        gracePeriodCoroutine = null;
    }

    public void TriggerGlobalExplosion()
    {
        // Find the components on the Main Camera
        PlayerInteract interaction = Camera.main.GetComponent<PlayerInteract>();
        TraumaEffects dieLogic = Camera.main.GetComponent<TraumaEffects>();



        if (GameObject.Find("ImpactTrigger") != null)
        {
            glass_crack = GameObject.Find("ImpactTrigger").GetComponent<GlassCrack>();
        }
        else
        {
            Debug.LogWarning("No ImpactTrigger found!");
        }


        // --- AUDIO & EFFECTS: Explosion ---
        if (audioSource.isPlaying) audioSource.Stop();
        if (woodChipsEffect != null) woodChipsEffect.Stop();
        Debug.Log("Cut complete! Triggering global explosion.");

        if (explosionSound != null)
        {
            Debug.Log("Playing explosion sound!");
            audioSource.PlayOneShot(explosionSound);
        }

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1f);

        // Check if glasses are equipped
        if (interaction != null && interaction.glassesEquipped)
        {
            Debug.Log("PLAYER SURVIVED: GlassLive logic would trigger here.");
            // (We will add LIVE logic later)
        }
        else
        {
            Debug.Log("PLAYER DIED: Triggering TraumaEffects on Camera.");
            if (dieLogic != null) dieLogic.TriggerDeathSequence(); // Starts the "DIE" path
        }
        // Logic for vibrations and slicing
        if (logVib != null) logVib.Explode(3, "Right");
        if (sawMachineVib != null) sawMachineVib.Explode(3, "Left");

        Slicing slicSaw = gameObject.GetComponentInParent<Slicing>();
        if (slicSaw != null) slicSaw.Shatter();

        if (glass_crack != null)
            glass_crack.Crack();

    }
}