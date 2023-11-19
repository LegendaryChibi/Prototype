using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    private static ObjectPool instance;

    public static ObjectPool Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private GameObject pooledObject;

    [SerializeField]
    private int pooledAmount;

    [SerializeField]
    private bool willGrow = true;
    public List<GameObject> pooledObjects;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        for(int i=0; i<pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if(willGrow)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pooledObjects.Add(obj);
            return obj;
        }

        Debug.LogError("Object Pool for " + pooledObject.name + " is not big enough.");

        return null;
    }
}
