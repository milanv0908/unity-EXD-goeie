using System;
using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class Playermovement2 : MonoBehaviour
{
    public float speed;
    public float minTimeout = 3f; // Minimum time in seconds before the next button press is allowed
    public float maxTimeout = 10f; // Maximum time in seconds before the next button press is allowed
    public Animator animatorToPause; // Reference to the animator component to pause
    private SerialPort sp;
    private int currentDirection = 0; // Variable to keep track of the current direction
    private float lastButtonPressTime = -1f; // Time of the last button press, initialized to -1 to indicate no presses yet
    private float timeSinceLastPress = 0f; // Time since the last button press
    public bool isFirstPress = true; // Boolean to track the first button press
    public int buttonPressCount = 0;
    private float time2; // Time of the previous button press

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
            string serialData = sp.ReadExisting(); // Read all available bytes as a string
            if (!string.IsNullOrEmpty(serialData))
            {
                currentDirection = Convert.ToInt32(serialData[0]); // Extract the first byte
                float previousTime2 = time2; // Store the previous value of time2
                time2 = Time.time; // Update time2 with the current time
                timeSinceLastPress = time2 - previousTime2; // Calculate time since last button press
                buttonPressCount++;
                Debug.Log(timeSinceLastPress);
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

    if (timeSinceLastPress < 0.1f)
        {
            // Debug.Log("Ignoring button press due to quick succession.");
            return; // Exit the Update method to ignore the button press
        }

    // Check for button press
    if (currentDirection != 0)
    {
        if (isFirstPress)
        {
            lastButtonPressTime = Time.time; // Update last button press time
            Debug.Log("First button press detected.");
            StartCoroutine(First());

            if (animatorToPause != null)
            {
                animatorToPause.SetTrigger("moving"); // Play animation if not already playing
                Debug.Log("First button press - animation started.");
            }
            return; // Exit the Update method to prevent further checks on the first press
        }

        if (timeSinceLastPress >= minTimeout && timeSinceLastPress <= maxTimeout)
        {
            lastButtonPressTime = Time.time; // Update last button press time
            Debug.Log("Button pressed within the allowed time frame.");
            Debug.Log("Button is pressable and won't trigger the falling animation.");

            if (animatorToPause != null)
            {
                animatorToPause.ResetTrigger("falling"); // Reset the falling trigger if button is pressed within the time window
                animatorToPause.speed = 1f; // Resume animation if paused
                Debug.Log("Button pressed within allowed time frame - animation resumed.");
            }
        }
        else if (timeSinceLastPress < minTimeout || timeSinceLastPress > maxTimeout)
        {
            if (buttonPressCount >= 2)
            {
                Debug.Log("Button pressed too soon or too late, triggering falling animation.");

                if (animatorToPause != null)
                {
                    animatorToPause.speed = 0f;
                }
            }
        }
    }

    // Check if the time since the last button press has exceeded the maxTimeout
    if (!isFirstPress && lastButtonPressTime >= 0 && Time.time - lastButtonPressTime > maxTimeout)
    {
        Debug.Log("No button press detected within the allowed time frame, triggering falling animation.");
        if (animatorToPause != null)
        {
                animatorToPause.speed = 0f;
        }
        lastButtonPressTime = Time.time; // Reset the timer to avoid continuous triggering
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

    IEnumerator First()
    {
        yield return new WaitForSeconds(1);
        isFirstPress = false;

        // Reset timeSinceLastPress to 0 after the first button press
        timeSinceLastPress = 0f;
    }
}
