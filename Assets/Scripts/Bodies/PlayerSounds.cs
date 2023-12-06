using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField]
    AudioClip fireClip;

    [SerializeField]
    AudioClip teleportClip;

    [SerializeField]
    AudioSource gunAudioSource;

    [SerializeField]
    AudioSource teleportAudioSource;

    public void FireGun()
    {
        float randPitch = Random.Range(0.5f, 1.2f);
        gunAudioSource.clip = fireClip;
        gunAudioSource.pitch = randPitch;
        gunAudioSource.Play();
    }

    public void Spawn()
    {
        teleportAudioSource.clip = teleportClip;
        teleportAudioSource.Play();
    }
}
