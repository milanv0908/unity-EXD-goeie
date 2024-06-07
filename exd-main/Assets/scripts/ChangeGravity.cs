using UnityEngine;

public class ChangeGravity : MonoBehaviour
{
    // Gravity direction vector
    public Vector3 newGravity = new Vector3(0, -9.81f, 0); // Example: default gravity

    void Start()
    {
        // Set the new gravity direction
        Physics.gravity = newGravity;
    }
}

