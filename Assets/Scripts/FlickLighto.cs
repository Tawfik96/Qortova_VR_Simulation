using UnityEngine;
using System.Collections;

public class FlickLighto : MonoBehaviour
{
    public Light lampLight;
    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.2f;

    public float bombDuration = 0.2f; // Longer duration for bomb effect
    public float flickerDuration = 2f; 

    

    void Start()
    {
        if (lampLight == null)
            lampLight = GetComponent<Light>();
        
        lampLight.enabled = true; // Ensure the light starts ON
        // Remove StartCoroutine from here so it doesn't play on its own!
    }

    // NEW FUNCTION: This is what you will call from the other script
    public void StartFlickering()
    {
        StopAllCoroutines(); // Stop any existing flicker to prevent bugs
        StartCoroutine(FlickerRoutine());
    }


    IEnumerator FlickerRoutine()
    {
        float timer = 0f;
        lampLight.intensity = 1f; // Start with light ON
        while (timer < flickerDuration)
        {
            lampLight.enabled = !lampLight.enabled;
            float waitTime = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(waitTime);
            timer += waitTime;
        }
        lampLight.enabled = false; // Usually better to end with Light ON
    }

    public void BombFlicker()
    {
        StopAllCoroutines();
        StartCoroutine(BombFlickerRoutine());
    }

    public IEnumerator BombFlickerRoutine()
    {
        lampLight.enabled = false;
        yield return new WaitForSeconds(0.1f); 
        lampLight.enabled = true;
        lampLight.intensity = 500f;
        yield return new WaitForSeconds(bombDuration); 

        // Turn light OFF
        lampLight.enabled = false;
       
    }

    public void StopFlickering()
    {
        StopAllCoroutines();
        lampLight.enabled = false; // Ensure light is off when stopped
    }
}