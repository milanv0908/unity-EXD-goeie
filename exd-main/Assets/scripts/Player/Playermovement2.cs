using System;
using UnityEngine;
using System.IO.Ports;
using System.Collections;

public class Playermovement2 : MonoBehaviour
{

      private bool test1 = false;
    public float speed;
    public float minTimeout = 3f; // Minimum time in seconds before the next button press is allowed
    public float maxTimeout = 14f; // Maximum time in seconds before the next button press is allowed
    public Animator animatorToPause; // Reference to the animator component to pause
    private SerialPort sp;
    private int currentDirection = 0; // Variable to keep track of the current direction
    private float lastButtonPressTime = -1f; // Time of the last button press, initialized to -1 to indicate no presses yet
    private float timeSinceLastPress = 0f; // Time since the last button press
    public bool isFirstPress = true; // Boolean to track the first button press
    private int buttonPressCount = 0; // Count of button presses
    public AudioSource audiosource;
    public AudioClip Rythm;
    public AudioClip RythmInstant;

    private bool hasPlayedAudio = false;
    private bool hasPlayedAudio2 = false;

    // Booleans to control logging
    private bool hasLoggedFirstPress = false;
    private bool hasLoggedAllowedPress = false;
    private bool hasLoggedDisallowedPress = false;
    private bool hasLoggedNoPressTimeout = false;

    // Variables for rewinding animation
    private bool isRewinding = false;
    private float rewindStartTime;
    private float animationStartTime;
    private Vector3 originalPosition;

    private float timeFirstPress = 0f;

    private bool inTimer = true;

    void Start()
    {
        try
        {
            sp = new SerialPort("COM7", 9600);
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
                    float previousTime2 = timeSinceLastPress; // Store the previous value of timeSinceLastPress
                    timeSinceLastPress = Time.time - lastButtonPressTime; // Calculate time since last button press

                    // Reset button press count if time since last press is too long
                    if (timeSinceLastPress > maxTimeout)
                    {
                        buttonPressCount = 0;
                    }

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
            return; // Exit the Update method to ignore the button press
        }

        if (currentDirection != 0)
        {
            if (isFirstPress)
            {
                lastButtonPressTime = Time.time; // Update last button press time
                hasPlayedAudio = false; // Reset the audio play flag on first press
                hasPlayedAudio2 = false; // Reset the audio play flag on first press

                if (!hasLoggedFirstPress)
                {
                    Debug.Log("First button press detected.");
                    hasLoggedFirstPress = true;
                    audiosource.PlayOneShot(RythmInstant);
                    audiosource.PlayOneShot(Rythm);
                }

                StartCoroutine(First());

                if (animatorToPause != null && !IsAnimationReversing())
                {
                    animatorToPause.SetTrigger("moving"); // Play animation if not already playing

                    if (!hasLoggedFirstPress)
                    {
                        Debug.Log("First button press - animation started.");
                    }
                }
                return; // Exit the Update method to prevent further checks on the first press
            }

            if (timeSinceLastPress >= minTimeout && timeSinceLastPress <= maxTimeout)
            {
                inTimer = true;
                test1 = false;

                if (!hasPlayedAudio)
                {
                    audiosource.Stop();
                    audiosource.PlayOneShot(Rythm);
                    hasPlayedAudio = true;
                    StartCoroutine(PlayAudio());
                }

                if (!hasLoggedAllowedPress)
                {
                    Debug.Log("Button pressed within the allowed time frame.");
                    Debug.Log("Button is pressable and won't trigger the falling animation.");
                    hasLoggedAllowedPress = true;
                }

                if (animatorToPause != null)
                {
                    animatorToPause.ResetTrigger("falling"); // Reset the falling trigger if button is pressed within the time window
                    animatorToPause.speed = 1f; // Resume animation if paused
                    lastButtonPressTime = Time.time; // Update last button press time
                    StartCoroutine(paus());
                }
            }
            else if (timeSinceLastPress < minTimeout || timeSinceLastPress > maxTimeout)
            {
                if (buttonPressCount >= 2)
                {
                    inTimer = false;

                    if (!hasPlayedAudio2)
                    {
                        audiosource.Stop();
                        audiosource.PlayOneShot(Rythm);
                        hasPlayedAudio2 = true;
                        StartCoroutine(PlayAudio2());
                    }

                    if (!hasLoggedDisallowedPress)
                    {
                        Debug.Log("Button pressed too soon or too late, adjusting animation.");
                        hasLoggedDisallowedPress = true;
                    }

                    if (animatorToPause != null)
                    {
                        StartCoroutine(AdjustAnimationBackwards());
                    }
                }
            }
        }

        if (!isFirstPress && lastButtonPressTime >= 0 && Time.time - lastButtonPressTime > maxTimeout)
        {
            if (!hasLoggedNoPressTimeout)
            {
                Debug.Log("No button press detected within the allowed time frame, pausing animation.");
                hasLoggedNoPressTimeout = true;
            }

            if (animatorToPause != null)
            {
                animatorToPause.speed = 0f; // Pause the animation
            }
            lastButtonPressTime = Time.time; // Reset the timer to avoid continuous triggering
        }

        // Handle animation rewind if isRewinding is true
        if (isRewinding)
        {
            float elapsedTime = Time.time - rewindStartTime;
            float normalizedTime = Mathf.Clamp01(animationStartTime - (elapsedTime / 2f)); // Adjusted rewind speed

            animatorToPause.Play(animatorToPause.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, normalizedTime);

            if (elapsedTime >= 2f)
            {
                isRewinding = false;
                animatorToPause.speed = 0f; // Pause at the rewinded point
            }
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
        timeFirstPress = Time.time;
        timeSinceLastPress = 0f;
    }

    IEnumerator paus()
    {
        yield return new WaitForSeconds(2);
        animatorToPause.speed = 0f;
    }

    IEnumerator PlayAudio()
    {
        if (inTimer)
        {
            yield return new WaitForSeconds(7);
            hasPlayedAudio = false;
            Debug.Log("1e audio doet");
        }
    }

    IEnumerator PlayAudio2()
    {
        if (!inTimer)
        {
            yield return new WaitForSeconds(7);
            hasPlayedAudio2 = false;
            Debug.Log("2e audio doet");
        }
    }

    IEnumerator AdjustAnimationBackwards()
    {

        if (!test1){

        if (animatorToPause != null)
        {
            float normalizedTime = Mathf.Clamp01(animatorToPause.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.0001f); // Adjusted step back
            animatorToPause.Play(animatorToPause.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, normalizedTime);

            rewindStartTime = Time.time; // Record the start time of the rewind
            isRewinding = true;
            animationStartTime = normalizedTime;

            yield return new WaitForSeconds(2); // Wait for 2 seconds

            isRewinding = false;
            animatorToPause.speed = 0f; // Pause at the rewinded point
            if (animatorToPause.speed == 0f){
                Debug.Log("Animation paused");
                animatorToPause.speed = 0f;
            }
            test1 = true;
        }
        }
    }

    bool IsAnimationReversing()
    {
        if (animatorToPause != null)
        {
            return animatorToPause.GetCurrentAnimatorStateInfo(0).speed < 0;
        }
        return false;
    }
}
