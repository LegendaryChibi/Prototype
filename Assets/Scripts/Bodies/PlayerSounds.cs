using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField]
    AudioClip fireClip;

    [SerializeField]
    AudioClip walkFootStepsClip;

    [SerializeField]
    AudioSource gunAudioSource;

    [SerializeField]
    AudioSource footStepAudioSource;

    public void FireGun()
    {
        float randPitch = Random.Range(0.5f, 1.2f);
        gunAudioSource.clip = fireClip;
        gunAudioSource.pitch = randPitch;
        gunAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
