using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endgoal : MonoBehaviour
{
    // Reference to the Blaadje script or object
    public blaadje Blaadje;

    // This method is called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the AnimatieActiveer property to true
            Blaadje.AnimatieActiveer = true;
            Debug.Log("Player has entered the trigger area!");
        }
    }
}

