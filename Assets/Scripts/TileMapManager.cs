using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Tilemaps;
using Unity.Jobs;
public class TileMapManager : MonoBehaviour
{
    public ObjectPool pool;
    public Tilemap tilemap;
    public Grid grid;
    public Transform tileHolder;
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
        pool.ReturnAll();
        PlaceObject();
        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
    public void ClearPlacedGameObjects()
    {
        if (tileHolder == null) return;

        var children = new List<GameObject>();
        foreach (Transform child in tileHolder) children.Add(child.gameObject);
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
                    pool.Get(TileEnum.mountain, worldPosition + new Vector3(0, (value + (Random.Range(0, 0.4f)))
                    * _heightMultiplyerMountain, 0), sand.transform.rotation);
                }
                else if (value > _noiseThresholdGrass)
                {
                    pool.Get(TileEnum.Grass, worldPosition + new Vector3(0, (value + Random.Range(-0.03f, 0.03f))
                    * _heightMultiplyerGrass, 0), sand.transform.rotation);
                }
                else if (value > _noiseThresholdSand)
                {
                    pool.Get(TileEnum.Sand, worldPosition + new Vector3(0, value * _heightMultiplyerSand, 0), sand.transform.rotation);
                }
                else
                {
                    pool.Get(TileEnum.Water, worldPosition + new Vector3(0, value * _heightMultiplyerWater, 0), sand.transform.rotation);
                }
            }
        }
    }
    public void CreateMap2()
    {
        var temp = Time.realtimeSinceStartup;

        pool.ReturnAll();

        int diameter = (range * 2 + 1);
        int totalTiles = diameter * diameter;

        NativeArray<int> tileTypes = new NativeArray<int>(totalTiles, Allocator.TempJob);
        NativeArray<float> tileHeights = new NativeArray<float>(totalTiles, Allocator.TempJob);

        var job = new TileMapGenerationJob
        {
            tileTypes = tileTypes,
            tileHeights = tileHeights,

            range = range,
            noiseOffset = Random.Range(0, 1000000),
            noiseFrequency = _noiseFrequency,

            thresholdMountain = _noiseThresholdMountain,
            thresholdGrass = _noiseThresholdGrass,
            thresholdSand = _noiseThresholdSand,

            heightMultiplierMountain = _heightMultiplyerMountain,
            heightMultiplierGrass = _heightMultiplyerGrass,
            heightMultiplierSand = _heightMultiplyerSand,
            heightMultiplierWater = _heightMultiplyerWater
        };

        JobHandle handle = job.Schedule(totalTiles, 64);
        handle.Complete();

        float hexWidth = 1f;
        float hexHeight = Mathf.Sqrt(3f) * 0.5f * hexWidth;

        for (int i = 0; i < totalTiles; i++)
        {
            if (tileTypes[i] == -1) continue;

            int q = (i % diameter) - range;
            int r = (i / diameter) - range;

            float worldX = hexWidth * (q + r * 0.5f);
            float worldZ = hexHeight * r;

            Vector3 worldPos = new Vector3(worldX, tileHeights[i], worldZ);

            TileEnum prefab;
            switch (tileTypes[i])
            {
                case 3: prefab = TileEnum.mountain; break;
                case 2: prefab = TileEnum.Grass; break;
                case 1: prefab = TileEnum.Sand; break;
                case 0: prefab = TileEnum.Water; break;
                default: prefab = TileEnum.Grass; break;
            }

            pool.Get(prefab, worldPos, sand.transform.rotation);
        }

        tileTypes.Dispose();
        tileHeights.Dispose();
        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
}