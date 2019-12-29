using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    [Header("Object pool properties")]
    protected List<GameObject>[] pools;
    [SerializeField] protected GameObject[] objectsToPool;
    [SerializeField] protected int[] amountsToPool;
    [SerializeField] protected bool[] shouldExpand;

    int[] poolLocations;

    protected virtual void Start()
    {
        poolLocations = new int[objectsToPool.Length];
        pools = new List<GameObject>[objectsToPool.Length];
        for (int poolNum = 0; poolNum < objectsToPool.Length; poolNum++)
        {
            pools[poolNum] = new List<GameObject>();
            poolLocations[poolNum] = 0;
            for (int i = 0; i < amountsToPool[poolNum]; i++)
            {
                AddObjectToPool(poolNum);
            }
        }
    }

    protected GameObject GetPooledObject(int poolNumber)
    {
        int pass = 0;
        while (true)
        {
            pass++;
            if (!pools[poolNumber][poolLocations[poolNumber]].activeInHierarchy)
            {
                return pools[poolNumber][poolLocations[poolNumber]];
            }
            poolLocations[poolNumber] = (poolLocations[poolNumber] + 1) % pools[poolNumber].Count;
            if(pass >= pools[poolNumber].Count)
            {
                break;
            }
        }
        //Debug.Log("No more objects!!");
        if (shouldExpand[poolNumber])
        {
            AddObjectToPool(poolNumber);
            int poolSize = pools[poolNumber].Count;
            return pools[poolNumber][poolSize - 1];
        }
        else
        {
            return null;
        }
    }
    void AddObjectToPool(int poolNumber)
    {
        GameObject obj = Instantiate(objectsToPool[poolNumber]);
        obj.SetActive(false);
        pools[poolNumber].Add(obj);
    }
}

