using UnityEngine;

public class RotateSAW : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    public Vector3 rotationSpeed = new Vector3(0, 0, 10024.8f);
 
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
