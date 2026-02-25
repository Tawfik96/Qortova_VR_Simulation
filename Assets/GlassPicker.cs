using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GlassPicker : MonoBehaviour
{
    [Header("Glass Visuals")]
    public Transform playerCamera;

    [Header("Audio")]
    public AudioClip notWornSound;
    public AudioClip wastedSound;
    public AudioClip vibrationSound; // If needed

    [Header("Death UI")]
    public GameObject wastedImageObject;
    public GameObject blackoutOverlay;
    
    [Header("Post Processing")]
    public Volume globalVolume;
    public Volume eyeDamageVolume;
    private ColorAdjustments colorAdjust;
    private Vignette vignette;

    private bool canRestart = false;
    private bool glassesWorn; // Set to true by default or via your wear logic
    

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main.transform;

        //get the player interact script and subscribe to the glasses not worn event
        // PlayerInteract playerInteract = playerCamera.GetComponent<PlayerInteract>();
        // if (playerInteract != null)
        // {
        //     glassesWorn = playerInteract.glassesEquipped; 
        // }
        

        // Setup Post-Processing
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out colorAdjust);
            globalVolume.profile.TryGet(out vignette);
        }

        if (wastedImageObject != null) wastedImageObject.SetActive(false);
    }

    public void Die() 
    {
        StartCoroutine(NotWornSequence());
    }

    IEnumerator NotWornSequence()
    {
        yield return new WaitForSeconds(0.3f);

        CameraShake shaker = playerCamera.GetComponent<CameraShake>();
        if (shaker != null) StartCoroutine(shaker.Shake(1.0f, 1.2f)); 

        if (blackoutOverlay != null) blackoutOverlay.SetActive(true);
        yield return new WaitForSeconds(0.5f); 

        if (blackoutOverlay != null) blackoutOverlay.SetActive(false);
        if (eyeDamageVolume != null) eyeDamageVolume.weight = 1f; 

        if (notWornSound != null)
        {
            AudioSource.PlayClipAtPoint(notWornSound, transform.position);
            yield return new WaitForSeconds(notWornSound.length);
        }

        if (wastedImageObject != null)
        {
            wastedImageObject.SetActive(true);
            if (colorAdjust != null) colorAdjust.saturation.value = -100;

            if (vignette != null)
            {
                vignette.active = true;
                vignette.color.value = Color.red;
                vignette.intensity.value = 0.6f;
            }

            yield return new WaitForSeconds(5.0f);
            canRestart = true;
        }
    }

    IEnumerator FlashBlackout(float duration)
    {
        blackoutOverlay.SetActive(true);
        yield return new WaitForSeconds(duration);
        blackoutOverlay.SetActive(false);
    }

    
}