using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class blaadje : MonoBehaviour
{

    public bool AnimatieActiveer = false;
    private bool playOnce = false;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (AnimatieActiveer && !playOnce) {
            animator.SetTrigger("leafactivate");
            StartCoroutine(EndScene());
            playOnce = true;
        }
    }

        IEnumerator EndScene()
    {
        yield return new WaitForSeconds(16);
        SceneManager.LoadScene("Cutscene");

    }
}
