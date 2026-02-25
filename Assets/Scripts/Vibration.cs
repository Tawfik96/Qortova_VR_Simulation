using UnityEngine;

public class Vibration : MonoBehaviour
{
    [Header("Vibration Settings")]
    public bool isVibrating = false;    

    public bool lockHorizontal = false; // If true, vibration will only affect the horizontal plane (X and Z), not vertical (Y)

    public bool isExploding = false; // If true, applies explosion force when vibration starts
    
    
    public float increaseRate = 0.005f;     // How fast the vibration grows
    public float initialMagnitude = 0f;   // Starting intensity
    public float maxMagnitude = 0.01f;     // Cap to prevent the object from flying away


    private Vector3 originalPosition;
    private float currentMagnitude;


    public void StartVibration()
    {
        isVibrating = true;
        originalPosition = transform.localPosition;
        currentMagnitude = initialMagnitude;
        // attach vibration sound to it


    }

    void Update()
    {
        if (isVibrating)
        {
            // 1. Gradually increase the magnitude over time
            currentMagnitude += increaseRate * Time.deltaTime;
            currentMagnitude = Mathf.Min(currentMagnitude, maxMagnitude);

            // 2. Generate a random displacement
            // We use Random.insideUnitSphere to get movement in right and left
            Vector3 randomOffset = Random.insideUnitSphere * currentMagnitude;
            if(lockHorizontal)
                randomOffset.y = 0f; // Lock vertical movement to create a horizontal vibration effect

            // Vector3 randomOffset = Random.insideUnitSphere  * currentMagnitude;

            // 3. Apply the offset to the original position
            transform.localPosition = originalPosition + randomOffset;
        }

        if(isExploding)
        {
            // Apply explosion force to the object
     
            isVibrating = false;
            // transform.localPosition = originalPosition;
            // currentMagnitude = 0f;
            Explode();
        }
    }


    public void Explode(float force_magnitude=5f, string direction = "Up")
    {   // add a Rigidbody if it doesn't have one
        Vector3 force;
        switch (direction)
        {
            case "Up":
                force = Vector3.up * force_magnitude;
                break;
            case "Down":
                force = Vector3.down * force_magnitude;
                break;
            case "Left":
                force = Vector3.left * force_magnitude;
                break;
            case "Right":
                force = Vector3.right * force_magnitude;
                break;
            default:
                force = Vector3.up * force_magnitude;
                break;
        };
        
        StopVibration(); 

        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        // rb.AddExplosionForce(50f, transform.position, 3f);
        rb.AddForce(force, ForceMode.Impulse);
    }
    // Call this function from your Slicing script if you want to stop it
    public void StopVibration()
    {
        isVibrating = false;
        transform.localPosition = originalPosition;
        currentMagnitude = 0f;
    }
}