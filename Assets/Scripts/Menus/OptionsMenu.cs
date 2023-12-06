using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField]
    private Slider masterVolume;

    [SerializeField]
    private Slider musicVolume;

    [SerializeField]
    private Slider effectsVolume;


    private void Awake()
    {
        masterVolume.value = AudioManager.Instance.getMasterVolume();
        musicVolume.value = AudioManager.Instance.getMusicVolume();
        effectsVolume.value = AudioManager.Instance.getEffectsVolume();
/*        AudioManager.Instance.setMasterVolume(masterVolume.value);
        AudioManager.Instance.setMusicVolume(musicVolume.value);
        AudioManager.Instance.setEffectsVolume(effectsVolume.value);*/
    }

    public void MainMenu()
    {
        GameManager.Instance.SavePreset();
        GameManager.Instance.LoadPreset();
        GameManager.Instance.ReturnToMainMenu();
    }

    public void UpdateMasterVolume()
    {
        AudioManager.Instance.setMasterVolume(masterVolume.value);
    }

    public void UpdateMusicVolume()
    {
        AudioManager.Instance.setMusicVolume(musicVolume.value);
    }

    public void UpdateEffectsVolume()
    {
        AudioManager.Instance.setEffectsVolume(effectsVolume.value);
    }
}
