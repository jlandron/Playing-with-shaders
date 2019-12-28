using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    [Header("Object pool properties")]
    protected List<GameObject> pooledObects;
    [SerializeField] protected GameObject objectToPool;
    [SerializeField] protected int amountToPool;
    [SerializeField] protected bool shouldExpand = true;

    int poolLocation = 0;

    protected virtual void Start()
    {
        pooledObects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            AddObjectToPool();
        }
    }

    protected GameObject GetPooledObject()
    {
        //for (int i = 0; i < pooledObects.Count; i++)
        //{
        //    if (!pooledObects[i].activeInHierarchy)
        //    {
        //        return pooledObects[i];
        //    }
        //}
        int pass = 0;
        while (true)
        {
            pass++;
            if (!pooledObects[poolLocation].activeInHierarchy)
            {
                return pooledObects[poolLocation];
            }
            poolLocation = (poolLocation + 1) % pooledObects.Count;
            if(pass >= pooledObects.Count)
            {
                break;
            }
        }
        Debug.Log("No more objects!!");
        if (shouldExpand)
        {
            AddObjectToPool();
            return pooledObects[pooledObects.Count - 1];
        }
        else
        {
            return null;
        }
    }
    void AddObjectToPool()
    {
        GameObject obj = Instantiate(objectToPool);
        obj.SetActive(false);
        pooledObects.Add(obj);
    }
}

