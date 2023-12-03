using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource musicAudioSource;

    [SerializeField]
    private AudioMixer mainMixer;

    private float overallVolume = 0f;

    private static AudioManager instance;

    public static AudioManager Instance {  get { return instance; } }


    private void Awake()
    {
        instance = this; 
    }

    public IEnumerator UnloadLevel()
    {
        yield return LerpVolume(overallVolume, overallVolume - 80, 1f);
    }

   public static void LoadLevelComplete()
    {
        AudioClip levelMusic = LevelManager.Instance.LevelMusic;

        if (levelMusic != null)
        {
            instance.musicAudioSource.clip = levelMusic;
            instance.musicAudioSource.Play();
        }

        instance.AudioFadeLevelStart();
    }

    public void AudioFadeLevelStart()
    {
        instance.StartCoroutine(LerpVolume(-80, overallVolume, 1f));
    }

    private IEnumerator LerpVolume(float startVol, float endVol, float time)
    {

        float currvol = startVol;
        float currTime = 0;
        while(currTime < time)
        {
            currTime += Time.deltaTime;
            currTime = Mathf.Clamp(currTime, 0f, time);
            currvol = Mathf.Lerp(startVol, endVol, currTime/time);
            instance.mainMixer.SetFloat("masterVolume", currvol);
            yield return null;
        }
    }
}
