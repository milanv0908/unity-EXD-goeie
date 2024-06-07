using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyTree : MonoBehaviour
{
    public GameObject player;  // Reference to the player object
    public float upwardForce = 10f; // Force to apply upward when player is stuck to the tree

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

        // Ensure player's Rigidbody is not kinematic
        playerRb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        // Apply upward force when player is stuck to the tree
        if (fixedJoint != null)
        {
            playerRb.AddForce(Vector3.up * upwardForce, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player collided with the tree");
            // Attach the player to the object if not already attached
            if (fixedJoint == null)
            {
                fixedJoint = player.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = objectRb;
                fixedJoint.breakForce = Mathf.Infinity;
                fixedJoint.breakTorque = Mathf.Infinity;

                // Freeze player's position and rotation constraints
                playerRb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player left the collision with the tree");
            // Detach the player and restore physics
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
                fixedJoint = null;

                // Unfreeze player's position and rotation constraints
                playerRb.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
