using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using UnityEngine.Profiling;
public class TileMapManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Grid grid;
    [Header("Tiles")]
    public GameObject grass;
    public GameObject sand;
    public GameObject mountain;
    public GameObject water;
    [Header("Map Generation")]
    [SerializeField] private float _noiseFrequency = 100f;
    [SerializeField] private float _noiseThresholdMountain = 0.5f;
    [SerializeField] private float _noiseThresholdGrass = 0.5f;
    [SerializeField] private float _noiseThresholdSand = 0.5f;
    [Header("Tile Position")]
    [SerializeField] private float _heightMultiplyerMountain = 2f;
    [SerializeField] private float _heightMultiplyerGrass = 2f;
    [SerializeField] private float _heightMultiplyerSand = 2f;
    [SerializeField] private float _heightMultiplyerWater = 2f;
    [Header("On Start")]
    [SerializeField] private bool _createMapOnStart = false;



    public int range = 3;
    void Start()
    {
        if (_createMapOnStart)
        {
            CreateMap2();
        }
    }
    public void GetTile(Vector3Int pos)
    {
        grid.WorldToCell(pos);
        tilemap.GetTile(pos);
    }
    public void GetNeighbour(Vector3Int pos)
    {
        tilemap.GetTile(pos);
    }
    public void CreateMap()
    {
        var temp = Time.realtimeSinceStartup;
        ClearPlacedGameObjects();
        PlaceObject();
        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
    void ClearPlacedGameObjects()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }
    void PlaceObject()
    {
        int noiseOffset = Random.Range(0, 1000000);
        int size = range * 2 + 1;

        for (int y = -range; y <= range; y++)
        {
            int rowLength = size - Mathf.Abs(y);
            int xOffset = -rowLength / 2;

            for (int x = 0; x < rowLength; x++)
            {
                int q = x + xOffset;
                float value = Mathf.PerlinNoise((x + noiseOffset) / _noiseFrequency,
                 (y + range + noiseOffset) / _noiseFrequency);

                Vector3Int cell = new Vector3Int(q, y, 0);
                Vector3 worldPosition = grid.CellToWorld(cell);
                if (value > _noiseThresholdMountain)
                {
                    Instantiate(mountain, worldPosition + new Vector3(0, (value + (Random.Range(0, 0.4f))) * _heightMultiplyerMountain, 0), sand.transform.rotation, transform);
                    //Instantiate(mountain, worldPosition + new Vector3(0, (value) * _heightMultiplyerMountain, 0), sand.transform.rotation, transform);
                }
                else if (value > _noiseThresholdGrass)
                {
                    Instantiate(grass, worldPosition + new Vector3(0, (value + Random.Range(-0.03f, 0.03f)) * _heightMultiplyerGrass, 0), sand.transform.rotation, transform);
                }
                else if (value > _noiseThresholdSand)
                {
                    Instantiate(sand, worldPosition + new Vector3(0, value * _heightMultiplyerSand, 0), sand.transform.rotation, transform);
                }
                else
                {
                    Instantiate(water, worldPosition + new Vector3(0, value * _heightMultiplyerWater, 0),
                    sand.transform.rotation, transform);
                }
            }
        }
    }
    public void CreateMap2()
    {
        var temp = Time.realtimeSinceStartup;

        ClearPlacedGameObjects();

        int size = (range * 2 + 1);
        int totalTiles = size * size;

        NativeArray<int> tileTypes = new NativeArray<int>(totalTiles, Allocator.TempJob);
        NativeArray<float> tileHeights = new NativeArray<float>(totalTiles, Allocator.TempJob);

        var job = new TileMapGenerationJob
        {
            tileTypes = tileTypes,
            tileHeights = tileHeights,

            range = range,
            noiseFrequency = _noiseFrequency,
            noiseThresholdMountain = _noiseThresholdMountain,
            noiseThresholdGrass = _noiseThresholdGrass,
            noiseThresholdSand = _noiseThresholdSand,
            noiseOffset = Random.Range(0, 1000000),

            heightMultiplierMountain = _heightMultiplyerMountain,
            heightMultiplierGrass = _heightMultiplyerGrass,
            heightMultiplierSand = _heightMultiplyerSand,
            heightMultiplierWater = _heightMultiplyerWater
        };

        JobHandle handle = job.Schedule(totalTiles, 64);
        handle.Complete();

        float hexWidth = 1f;
        float hexHeight = Mathf.Sqrt(3f) / 2f * hexWidth;

        for (int i = 0; i < totalTiles; i++)
        {
            if (tileTypes[i] == -1) continue; // skip invalid

            int diameter = range * 2 + 1;
            int q = (i % diameter) - range;
            int r = (i / diameter) - range;

            float worldX = hexWidth * (q + r * 0.5f);
            float worldZ = hexHeight * r;

            Vector3 worldPos = new Vector3(worldX, tileHeights[i], worldZ);

            GameObject prefab = null;
            switch (tileTypes[i])
            {
                case 3: prefab = mountain; break;
                case 2: prefab = grass; break;
                case 1: prefab = sand; break;
                case 0: prefab = water; break;
            }

            if (prefab != null)
                Instantiate(prefab, worldPos, prefab.transform.rotation, transform);
        }

        tileTypes.Dispose();
        tileHeights.Dispose();

        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
}