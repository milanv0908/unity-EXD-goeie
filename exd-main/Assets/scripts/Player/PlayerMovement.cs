using System;
using UnityEngine;
using System.IO.Ports;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    SerialPort sp;

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
                int direction = sp.ReadByte();
                MoveObject(direction);
                Debug.Log("Direction: " + direction);
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
    }

    void MoveObject(int direction)
    {
        float amountToMove = speed * Time.deltaTime; // Calculate amount to move based on speed and time
        if (direction == 1)
        {
            Vector3 movement = Vector3.up * amountToMove; // Move up on the Y-axis
            transform.Translate(movement, Space.World);
        }
    }

    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
        }
    }
}


