using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endgoal : MonoBehaviour
{
    // Reference to the Blaadje script or object
    public blaadje Blaadje;
    public Playermovement2 playermovement;
    public Narrator narrator;
    private bool useonce = true;

    // This method is called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player") && useonce)
        {
            playermovement.EndIsNow = true;
            // Set the AnimatieActiveer property to true
            Blaadje.AnimatieActiveer = true;
            narrator.EndReached = true;
            Debug.Log("Player has entered the trigger area!");
            useonce = false;
        }
    }
}

