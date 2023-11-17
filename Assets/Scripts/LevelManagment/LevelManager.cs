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
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected");
        if (other.gameObject == GameManager.Instance.Player) 
        {
            GameManager.Instance.LevelComplete();
        }
    }
}
