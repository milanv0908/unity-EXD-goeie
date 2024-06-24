using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorEnd : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audioSource;
    public AudioClip EndNarrator;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(EndNarrator);
    }

    void Update()
    {
        
    }
}
