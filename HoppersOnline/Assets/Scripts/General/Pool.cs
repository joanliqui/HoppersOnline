using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int initialPoolSize = 8;

    private Queue<GameObject> objectPool;

    private void Start()
    {
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddObject();
        }
    }
    public GameObject Get()
    {
        if(objectPool.Count == 0)
        {
            AddObject();
        }

        return objectPool.Dequeue();
    }

    private void AddObject()
    {
        GameObject newObject = GameObject.Instantiate(prefab);
        newObject.SetActive(false);
        objectPool.Enqueue(newObject);

        newObject.GetComponent<IPooleable>().Pool = this;
    }

    public void ReturnToPool(IPooleable objectToReturn)
    {
        objectToReturn.GameObject.SetActive(false);
        objectPool.Enqueue(objectToReturn.GameObject);
    }
}
