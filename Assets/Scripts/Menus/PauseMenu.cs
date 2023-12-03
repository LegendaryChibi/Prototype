using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{

    [SerializeField]
    AudioMixerSnapshot pausedSnapshot;

    [SerializeField]
    AudioMixerSnapshot unpausedSnapshot;

    [SerializeField]
    private GameObject pauseMenuCanvas;

    private bool paused = false;

    private bool canPause = false;

    public bool CanPause 
    {  
        get { return canPause; }
        set { canPause = value; }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && CanPause)
        {
            Pause(!paused);
        }
    }

    private void Pause(bool pause)
    {
        paused = pause;
        pauseMenuCanvas.SetActive(paused);
        if(pause)
        {
            Time.timeScale = 0;
            pausedSnapshot.TransitionTo(0.1f);
        }
        else
        {
            Time.timeScale = 1;
            unpausedSnapshot.TransitionTo(0.1f);
        }
    }
}
