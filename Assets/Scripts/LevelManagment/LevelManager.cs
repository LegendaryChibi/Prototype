using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }
            if (!instance)
            {
                Debug.LogError("No Level Manager Found.");
            }

            return instance;
        }
    }

    private List<AssassinControllerAI> enemies;

    private Transform spawnPoint;

    [SerializeField]
    private Renderer goalRenderer;
    private Material goalMaterial;
    private Color startColor;
    private Color endColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        enemies = new List<AssassinControllerAI>();
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;

        if (goalRenderer)
        {
            goalMaterial = goalRenderer.material;
            startColor = goalMaterial.GetColor("_Color");
            endColor = Color.green;
            endColor.a = startColor.a;
        }
    }

    public void Update()
    {
        if (CheckAllEnemiesDead())
        {
            goalMaterial.SetColor("_Color", endColor);
        }
    }

    public void ResisterEnemy(AssassinControllerAI assasin)
    {
        enemies.Add(assasin);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Player && CheckAllEnemiesDead()) 
        {
            GameManager.Instance.LevelComplete();
        }
    }

    private bool CheckAllEnemiesDead()
    {
        bool retVal = true;
        for (int i = 0; i < enemies.Count; i++)
        {
            retVal = retVal && enemies[i].IsDead;
        }

        return retVal;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }

   
}
