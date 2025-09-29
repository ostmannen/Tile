using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPool : MonoBehaviour
{
    public Transform tileHolder;
    public TilePrefab[] tilePrefabs;
    public int poolAmount;

    public Dictionary<TileEnum, Queue<GameObject>> _pools = new Dictionary<TileEnum, Queue<GameObject>>();
    public Dictionary<TileEnum, List<GameObject>> _activeTiles = new Dictionary<TileEnum, List<GameObject>>();


    public void CreatePoolItems()
    {
        DestroyAll();
        
        foreach (var item in tilePrefabs)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < poolAmount; i++)
            {
                GameObject gameObject = Instantiate(item.gameObject, transform.position, item.gameObject.transform.rotation, tileHolder);
                gameObject.SetActive(false);
                queue.Enqueue(gameObject);
            }

            _pools.Add(item.tileEnum, queue);
        }
    }
    public GameObject Get(TileEnum name, Vector3 position, Quaternion rotation)
    {
        if (_pools.TryGetValue(name, out Queue<GameObject> queue) && queue.Count > 0)
        {
            if (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();

                obj.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);

                if (!_activeTiles.ContainsKey(name)) _activeTiles[name] = new List<GameObject>();

                _activeTiles[name].Add(obj);

                return obj;
            }

            Debug.LogWarning($"Pool is empty!");

        }
        return null;
    }
    public void Return(TileEnum tileEnum, GameObject obj)
    {
        obj.SetActive(false);
        _pools[tileEnum].Enqueue(obj);

        if (_activeTiles.ContainsKey(tileEnum))
            _activeTiles[tileEnum].Remove(obj);
    }
    public void ReturnAll()
    {
        foreach (var kvp in _activeTiles)
        {
            TileEnum type = kvp.Key;
            foreach (var obj in kvp.Value)
            {
                obj.SetActive(false);
                _pools[type].Enqueue(obj);
            }
        }
        _activeTiles.Clear();
    }
    public void DestroyAll()
    {
        foreach (var kvp in _activeTiles)
        {
            foreach (var obj in kvp.Value)
            {

                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }
        }
        _activeTiles.Clear();

        foreach (var kvp in _pools)
        {
            foreach (var obj in kvp.Value)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }
        }
        _pools.Clear();
        
        if (tileHolder == null) return;

        var children = new List<GameObject>();
        foreach (Transform child in tileHolder) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }
}
[System.Serializable]
public struct TilePrefab
{
    public TileEnum tileEnum;
    public GameObject gameObject;
}