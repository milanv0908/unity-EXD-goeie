using System;
using UnityEngine;
using System.IO.Ports;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float timeout = 5f; // Time in seconds before stopping movement if no button press
    private SerialPort sp;
    private int currentDirection = 0; // Variable to keep track of the current direction
    private float lastButtonPressTime = 0f; // Time of the last button press

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

                    if (currentDirection != 0)
                    {
                        lastButtonPressTime = Time.time; // Update last button press time
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

        // Check if the timeout has been exceeded
        if (Time.time - lastButtonPressTime > timeout)
        {
            currentDirection = 0; // Stop movement if the button hasn't been pressed for 'timeout' seconds
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

