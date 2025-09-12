using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public TilePrefab[] tilePrefabs;
    public int poolAmount;

    private Dictionary<TileEnum, Queue<GameObject>> _pools;

    
    public void CreatePoolItem()
    {
        for (int i = 0; i < poolAmount; i++)
        {

        }
    }
    public void CreateObject()
    {
        if (poolAmount == 0) return;

        for (int i = 0; i < poolAmount; i++)
        {

        }
    }
    public void OnReturnToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    public void OnTakeFromPool(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
}
[System.Serializable]
public struct TilePrefab
{
    public TileEnum tileEnum;
    public GameObject gameObject;
}