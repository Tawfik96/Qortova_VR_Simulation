using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TraumaEffects : MonoBehaviour
{
    [Header("UI Overlays")]
    public GameObject blackoutOverlay;
    public GameObject wastedImageObject;

    [Header("Volumes")]
    public Volume globalVolume;      // Profile with ColorAdjustments/Vignette
    public Volume eyeDamageVolume;   // Profile with Right-side Vignette

    [Header("Audio")]
    public AudioClip traumaSound;    // The 'NotWorn' scream/impact
    public AudioClip wastedMusic;
    private AudioSource audioSource;

    private ColorAdjustments colorAdjust;
    private Vignette redVignette;
    private bool canRestart = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (globalVolume.profile.TryGet(out colorAdjust) && globalVolume.profile.TryGet(out redVignette))
        {
            // Reset values on start
            colorAdjust.saturation.value = 0;
            redVignette.active = false;
        }

        if (blackoutOverlay) blackoutOverlay.SetActive(false);
        if (wastedImageObject) wastedImageObject.SetActive(false);
        if (eyeDamageVolume) eyeDamageVolume.weight = 0;
    }

    void Update()
    {
        if (canRestart && (Input.GetMouseButtonDown(0) || Input.anyKeyDown))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerDeathSequence()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // 1. Violent Shake & Blackout
        CameraShake shaker = GetComponent<CameraShake>();
        if (shaker) StartCoroutine(shaker.Shake(1.0f, 1.2f));
        if (blackoutOverlay) blackoutOverlay.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // 2. Transition to Eye Damage
        if (blackoutOverlay) blackoutOverlay.SetActive(false);
        if (eyeDamageVolume) eyeDamageVolume.weight = 1f;

        if (traumaSound) AudioSource.PlayClipAtPoint(traumaSound, transform.position);

        yield return new WaitForSeconds(1.5f);

        // 3. Wasted Screen
        if (wastedImageObject) wastedImageObject.SetActive(true);
        colorAdjust.saturation.value = -100; // B&W
        redVignette.active = true;
        redVignette.intensity.value = 0.6f;

        if (wastedMusic) audioSource.PlayOneShot(wastedMusic);

        yield return new WaitForSeconds(3.0f);
        canRestart = true;
    }
}