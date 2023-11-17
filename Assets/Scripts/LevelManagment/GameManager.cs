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
    public GameObject Player
    {
        get { return player; }
    }

    [SerializeField]
    private string[] levelNames;

    private bool isLoading = false;
    private int currentLevel = 0;
    private string currentLevelName;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LoadLevel", levelNames[0]);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        isLoading = true;
        player.SetActive(false);
        //Unload Current Scene
        if(!string.IsNullOrEmpty(currentLevelName))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            while(!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        //Load New Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        currentLevelName = levelName;

        //Initialize
        player.transform.position = Vector3.zero;
        player.SetActive(true);
        isLoading = false;
    }

    public void LevelComplete()
    {
        currentLevel++;
        if(currentLevel < levelNames.Length)
        {
            StartCoroutine(LoadLevel(levelNames[currentLevel]));
        }
    }
}
