using UnityEngine;
using System.Collections;
 
public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        // We use localPosition so it shakes relative to the player's head
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;
 
        while (elapsed < duration)
        {
            // Focus magnitude mostly on Y for that "Up/Down" jolt
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-1.5f, 1.5f) * magnitude; // 3x stronger vertically
            float z = Random.Range(-0.5f, 0.5f) * magnitude;
 
            transform.localPosition = new Vector3(x, originalPos.y + y, originalPos.z + z);
 
            elapsed += Time.deltaTime;
            yield return null;
        }
 
        transform.localPosition = originalPos;
    }
}
 