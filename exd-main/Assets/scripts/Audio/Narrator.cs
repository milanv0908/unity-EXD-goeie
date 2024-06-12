using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip NarratorStart;
    public AudioClip NarratorMid;
    public AudioClip NarratorEnd;

    public bool MidReached = false;
    public bool EndReached = false;

    void Start()
    {
        audioSource.PlayOneShot(NarratorStart);
    }


    void Update()
    {
        if (MidReached)
        {
            StartCoroutine(mid());
            MidReached = false;
        }

        if (EndReached)
        {
            StartCoroutine(end());
            EndReached = false;
        }
    }

    IEnumerator mid()
    {
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(NarratorMid);

    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(NarratorEnd);
        
    }
}
