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
    private Slider effectsSlider;

    public void MainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
