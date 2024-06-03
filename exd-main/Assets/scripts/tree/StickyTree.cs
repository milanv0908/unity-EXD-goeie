using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyTree : MonoBehaviour
{
    public GameObject player;  // Reference to the player object

    private FixedJoint fixedJoint;
    private Rigidbody playerRb;
    private Rigidbody objectRb;

    private void Start()
    {
        // Get the Rigidbody components
        playerRb = player.GetComponent<Rigidbody>();
        objectRb = GetComponent<Rigidbody>();

        // Ensure the object has a Rigidbody
        if (objectRb == null)
        {
            objectRb = gameObject.AddComponent<Rigidbody>();
            objectRb.isKinematic = true;  // Make sure the object itself is not affected by physics if it should be static
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered the trigger zone");
            // Ensure the player stops being affected by physics
            playerRb.isKinematic = true;

            // Attach the player to the object if not already attached
            if (fixedJoint == null)
            {
                fixedJoint = player.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = objectRb;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player exited the trigger zone");
            // Detach the player and restore physics
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
                fixedJoint = null;
            }
            playerRb.isKinematic = false;
        }
    }
}
