using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Audio;

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
    private string optionsMenuName;

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

    [SerializeField]
    private GameObject victoryScreen;

    private bool saveGamePresent = false;
    private string saveFilePath;
    public bool SaveGamePresent { get { return saveGamePresent; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        gameOverScreen.gameObject.SetActive(false);
        saveFilePath = Application.persistentDataPath + "/Prototype.dat";
    }

    // Start is called before the first frame update
    void Start()
    {
        instance.LoadGame();
        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        pauseMenu.CanPause = false;
        StartCoroutine("LoadLevel", mainMenuName);
    }

    public void OptionsMenu()
    {
        pauseMenu.CanPause = false;
        StartCoroutine("LoadLevel", optionsMenuName);
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
            body.GameReset();
            StartCoroutine(LoadLevel(levelNames[currentLevel]));
        }
        else
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            victoryScreen.gameObject.SetActive(true);
            player.SetActive(false );
        }
        instance.SaveGame();
    }

    public void StartNewGame()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        currentLevel = 0;
        StartCoroutine("LoadLevel", levelNames[currentLevel]);
        body.GameReset();
        gameOverScreen.gameObject.SetActive(false);
    }

    public void ContinueGame()
    {
        body.GameReset();
        StartCoroutine("LoadLevel", levelNames[currentLevel]);
    }

    public void PlayerDeath()
    {
        body.Controller.gameObject.SetActive(false);
        player.SetActive(false);
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
        pauseMenu.CanPause = true;
    }

    private void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("Save path " + saveFilePath);

        FileStream file = File.Create(saveFilePath);

        SaveGame data = new SaveGame();
        data.CurrentLevel = currentLevel;

        bf.Serialize(file, data);
        file.Close();
    }

    private void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            saveGamePresent = true;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            SaveGame data = (SaveGame) bf.Deserialize(file);
            file.Close();

            currentLevel = data.CurrentLevel;
        }
    }
}

[Serializable]
class SaveGame
{
    public int CurrentLevel { get; set;}
}