using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidNarActivate : MonoBehaviour
{
    public Narrator narrator;
    private bool useonce = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            Debug.Log("MidCollided");
            if (useonce)
            {
                narrator.MidReached = true;
                useonce = false;
            }
        }
    }
}
