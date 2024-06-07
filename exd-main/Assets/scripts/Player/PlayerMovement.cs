using System;
using UnityEngine;
using System.IO.Ports;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    SerialPort sp;
    int currentDirection = 0; // Variable to keep track of the current direction

    void Start()
    {
        try
        {
            sp = new SerialPort("COM7", 9600);
            sp.Open();
            sp.ReadTimeout = 100; // Adjusting the read timeout to 100ms
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

        // Move the object based on the current direction
        MoveObject(currentDirection);
    }

    void MoveObject(int direction)
    {
        float amountToMove = speed * Time.deltaTime; // Calculate amount to move based on speed and time

        if (direction == 1) // Move up
        {
            Vector3 movement = Vector3.up * amountToMove; // Move up on the Y-axis
            transform.Translate(movement, Space.World);
        }
        else if (direction == 2) // Move down (example)
        {
            Vector3 movement = Vector3.down * amountToMove; // Move down on the Y-axis
            transform.Translate(movement, Space.World);
        }
        // Add other directions as needed
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
        }
    }
}

