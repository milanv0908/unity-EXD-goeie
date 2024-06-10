using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class Playermovement2 : MonoBehaviour
{
    public float minTimeout = 5f; // Minimum time in seconds before the next button press is allowed
    public float maxTimeout = 10f; // Maximum time in seconds before the next button press is allowed
    private SerialPort sp;
    private float lastButtonPressTime = -1f; // Time of the last button press, initialized to -1 to indicate no presses yet
    private bool isFirstPress = true; // Boolean to track the first button press

    void Start()
    {
        StartCoroutine(First()); // Start the coroutine to set isFirstPress to false after 1 second

        // Initialize serial port
        sp = new SerialPort("COM3", 9600); // Change the port and baud rate as per your Arduino setup
        sp.Open();
        sp.ReadTimeout = 1; // Set a read timeout to avoid blocking the main thread
    }

    void Update()
    {
        // Check for button press from Arduino via serial port
        if (!isFirstPress && sp.IsOpen)
        {
            try
            {
                // Read data from serial port
                string data = sp.ReadLine();

                // Check if the received data indicates a button press
                if (data.Trim() == "forward")
                {
                    float currentTime = Time.time;

                    // Calculate the time difference between the current button press and the last button press
                    float elapsedTime = currentTime - lastButtonPressTime;

                    // Check if the button press is within the desired time range
                    if (elapsedTime < minTimeout || elapsedTime > maxTimeout)
                    {
                        // Trigger the falling animation directly on the player GameObject
                        GetComponent<Animator>().SetTrigger("Falling");

                        // Update the last button press time
                        lastButtonPressTime = currentTime;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }
    }

    IEnumerator First()
    {
        yield return new WaitForSeconds(1);
        isFirstPress = false;
    }

    // Additional methods for SerialPort handling
    void OnApplicationQuit()
    {
        if (sp != null && sp.IsOpen)
        {
            sp.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
