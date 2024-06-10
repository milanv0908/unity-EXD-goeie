using System;
using UnityEngine;
using System.IO.Ports;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float minTimeout = 5f; // Minimum time in seconds before the next button press is allowed
    public float maxTimeout = 10f; // Maximum time in seconds before the next button press is allowed
    public Animator animatorToPause; // Reference to the animator component to pause
    private SerialPort sp;
    private int currentDirection = 0; // Variable to keep track of the current direction
    private float lastButtonPressTime = 0f; // Time of the last button press
    private bool isFirstPress = true; // Boolean to track the first button press

    void Start()
    {
        try
        {
            sp = new SerialPort("COM3", 9600);
            sp.Open();
            sp.ReadTimeout = 100; // Adjusting the read timeout to 100ms
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (sp != null && sp.IsOpen)
        {
            try
            {
                if (sp.BytesToRead > 0) // Check if there are bytes to read
                {
                    currentDirection = sp.ReadByte();
                    Debug.Log("Direction: " + currentDirection);

                    float timeSinceLastPress = Time.time - lastButtonPressTime;

                    if (currentDirection != 0)
                    {
                        if (isFirstPress || (timeSinceLastPress >= minTimeout && timeSinceLastPress <= maxTimeout))
                        {
                            lastButtonPressTime = Time.time; // Update last button press time
                            Debug.Log("Button pressed within the allowed time frame or first press.");

                            if (animatorToPause != null && !animatorToPause.GetCurrentAnimatorStateInfo(0).IsName("YourAnimationName"))
                            {
                                animatorToPause.Play("YourAnimationName"); // Play animation if not already playing
                                animatorToPause.ResetTrigger("falling"); // Reset the falling trigger if button is pressed within the time window
                                animatorToPause.speed = 1f; // Resume animation if paused
                            }

                            isFirstPress = false; // Mark that the first press has occurred
                        }
                        else
                        {
                            // Trigger the falling animation if button pressed too soon or too late
                            Debug.Log("Button pressed too soon or too late, triggering falling animation.");
                            if (animatorToPause != null)
                            {
                                animatorToPause.SetTrigger("falling");
                            }
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
                // Handle timeout - it is expected to occur frequently
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }

        // Check if the time since the last button press has exceeded the maxTimeout
        if (!isFirstPress && Time.time - lastButtonPressTime > maxTimeout)
        {
            Debug.Log("No button press detected within the allowed time frame, triggering falling animation.");
            if (animatorToPause != null)
            {
                animatorToPause.SetTrigger("falling");
            }
        }

        // Move the object based on the current direction
        MoveObject(currentDirection);
    }

    void MoveObject(int direction)
    {
        float amountToMove = speed * Time.deltaTime; // Calculate amount to move based on speed and time
        Vector3 movement = Vector3.zero;

        switch (direction)
        {
            case 1: // Move up
                movement = Vector3.up * amountToMove; // Move up on the Y-axis
                break;
            case 2: // Move down
                movement = Vector3.down * amountToMove; // Move down on the Y-axis
                break;
            // Add other cases for different directions as needed
            default:
                // No movement for direction 0 or unknown direction
                break;
        }

        if (movement != Vector3.zero)
        {
            transform.Translate(movement, Space.World);
            Debug.Log("Moving in direction: " + direction + " with movement: " + movement);
        }
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
            Debug.Log("Serial port closed.");
        }
    }
}





