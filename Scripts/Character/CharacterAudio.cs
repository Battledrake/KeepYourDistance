using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    public AudioClip[] steps;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayRandomStep();
        }
    }
    public void PlayRandomStep()
    {
        if (steps.Length > 0)
        {
            var stepIndex= Random.Range(0, steps.Length);
            audioSource.PlayOneShot(steps[stepIndex]);
        }
    }
}
