using UnityEngine;

public class RotateSprite : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of oscillation
    public float rotationAmplitude = 15.0f; // Maximum rotation angle (in degrees)
    private float startTime; // To keep track of time

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the oscillation angle
        float angle = Mathf.Sin((Time.time - startTime) * rotationSpeed) * rotationAmplitude;
        
        // Apply the rotation to the sprite
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
