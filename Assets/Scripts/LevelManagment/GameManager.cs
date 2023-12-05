using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            if (!instance)
            {
                Debug.LogError("No Game Manager Found.");
            }

            return instance;
        }
    }

    [SerializeField]
    private GameObject player;
    public static GameObject Player
    {
        get { return instance.player; }
    }


    [SerializeField]
    private PlayerBody body;

    [SerializeField]
    private string[] levelNames;

    [SerializeField]
    private string mainMenuName;

    [SerializeField]
    private LoadingScreen loadingScreen;

    private int currentLevel = 0;
    private string currentLevelName;

    [SerializeField]
    private ParticleSystem spawnEffect;

    [SerializeField]
    private PauseMenu pauseMenu;

    [SerializeField]
    private GameOverScreen gameOverScreen;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        gameOverScreen.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        pauseMenu.CanPause = false;
        StartCoroutine("LoadLevel", mainMenuName);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        player.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        pauseMenu.CanPause = false;
        yield return new WaitForSeconds(.25f);

        //Unload Current Scene
        if(!string.IsNullOrEmpty(currentLevelName))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            yield return AudioManager.Instance.UnloadLevel();
            while(!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        //Load New Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            loadingScreen.UpdateSlider(asyncLoad.progress);
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        currentLevelName = levelName;
        AudioManager.LoadLevelComplete();
        loadingScreen.gameObject.SetActive(false);
        pauseMenu.CanPause = true;
        //Initialize
        player.transform.position = LevelManager.Instance.GetSpawnPoint().position;
        player.transform.rotation = LevelManager.Instance.GetSpawnPoint().rotation;
        player.SetActive(true);
        spawnEffect.Play();
    }

    public void LevelComplete()
    {
        currentLevel++;
        if(currentLevel < levelNames.Length)
        {
            StartCoroutine(LoadLevel(levelNames[currentLevel]));
        }
    }

    public void StartNewGame()
    {
        StartCoroutine("LoadLevel", levelNames[currentLevel]);
        body.Controller.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(false);
    }

    public void PlayerDeath()
    {
        body.Controller.gameObject.SetActive(false);
        pauseMenu.CanPause = false;
        gameOverScreen.gameObject.SetActive(true);
    }

    public void PlayerRespawn()
    {
        player.transform.position = LevelManager.Instance.GetSpawnPoint().position;
        player.transform.rotation = LevelManager.Instance.GetSpawnPoint().rotation;
        body.GameReset();
        LevelManager.Instance.Reset();
        ObjectPoolManager.Instance.GameReset();
        player.SetActive(true);
        gameOverScreen.gameObject.SetActive(false);
    }
}
