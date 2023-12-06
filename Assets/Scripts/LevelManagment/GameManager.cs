using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject HUD;

    [SerializeField]
    private Slider playerHealth;

    [SerializeField]
    private Slider enemyCount;

    [SerializeField]
    private Slider enemyHealth;

    private bool saveGamePresent = false;
    private string saveFilePath;
    private string savePresetPath;
    public bool SaveGamePresent { get { return saveGamePresent; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        gameOverScreen.gameObject.SetActive(false);
        saveFilePath = Application.persistentDataPath + "/Prototype.dat";
        savePresetPath = Application.persistentDataPath + "/PrototypePresets.dat";
    }

    private void LateUpdate()
    {
        if (HUD.gameObject.activeInHierarchy && LevelManager.Instance != null && LevelManager.Instance.TotalEnemies() != 0)
        {
            playerHealth.value = body.CurrHealth()/100;
            float aliveEnemies = LevelManager.Instance.TotalEnemies() - LevelManager.Instance.CheckAllEnemiesDeadCount();
            enemyCount.value = (float) (aliveEnemies / LevelManager.Instance.TotalEnemies());

            float totalEnemyHealth = LevelManager.Instance.CheckAllEnemiesHealth() / LevelManager.Instance.TotalEnemies();
            enemyHealth.value = totalEnemyHealth / 100;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance.LoadPreset();
        instance.LoadGame();
        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        pauseMenu.CanPause = false;
        StartCoroutine("LoadLevel", mainMenuName);
    }

    public void LoadOptionsMenu()
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
        HUD.gameObject.SetActive(true);
    }

    public void ContinueGame()
    {
        body.GameReset();
        StartCoroutine("LoadLevel", levelNames[currentLevel]);
    }

    public void PlayerDeath()
    {
        HUD.gameObject.SetActive(false);
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
        HUD.gameObject.SetActive(true);
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

    public void SavePreset()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("Save path " + savePresetPath);

        FileStream file = File.Create(savePresetPath);

        SavePresets data = new SavePresets();
        data.CurrentMasterVolume = AudioManager.Instance.getMasterVolume();
        data.CurrentMusicVolume = AudioManager.Instance.getMusicVolume();
        data.CurrentEffectsVolume = AudioManager.Instance.getEffectsVolume();

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadPreset()
    {
        if (File.Exists(savePresetPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePresetPath, FileMode.Open);
            SavePresets data = (SavePresets) bf.Deserialize(file);
            file.Close();

            AudioManager.Instance.setMasterVolume(data.CurrentMasterVolume);
            AudioManager.Instance.setMusicVolume(data.CurrentMusicVolume);
            AudioManager.Instance.setEffectsVolume(data.CurrentEffectsVolume);
        }
    }
}

[Serializable]
class SaveGame
{
    public int CurrentLevel { get; set;}
}

[Serializable]
class SavePresets
{
    public float CurrentMasterVolume { get; set;}
    public float CurrentMusicVolume { get; set; }
    public float CurrentEffectsVolume { get; set; }
}