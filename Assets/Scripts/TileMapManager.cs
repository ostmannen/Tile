using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using System.Collections.Generic;
using Unity.Mathematics;

public class TileMapManager : MonoBehaviour
{
    public ObjectPool pool;
    public Transform tileHolder;
    public CreateTileEnum createTileEnum;
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
    [Header("Layered Noise")]
    [SerializeField] private int octaves = 1;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float _lacunarity = 2f;

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
            CreateMap();
        }
    }

    public void ClearPlacedGameObjects()
    {
        if (tileHolder == null) return;

        var children = new List<GameObject>();
        foreach (Transform child in tileHolder) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }
    public void CreateMap()
    {
        Debug.Log("default");
        var temp = Time.realtimeSinceStartup;

        pool.ReturnAll();
        if (createTileEnum == CreateTileEnum.Instantiate) ClearPlacedGameObjects();

        int totalTiles = 3 * range * (range + 1) + 1;
        int noiseOffset = UnityEngine.Random.Range(0, 10000);

        NativeArray<int> tileTypes = new NativeArray<int>(totalTiles, Allocator.Temp);
        NativeArray<float> tileHeights = new NativeArray<float>(totalTiles, Allocator.Temp);
        NativeArray<int2> tileCoords = new NativeArray<int2>(totalTiles, Allocator.Temp);

        //Creates tilecords
        int index = 0;
        for (int q = -range; q <= range; q++)
        {
            for (int r = -range; r <= range; r++)
            {
                int s = -q - r;
                if (math.abs(s) > range) continue;

                tileCoords[index] = new int2(q, r);
                index++;
            }
        }

        if (index != totalTiles)
        {
            Debug.LogWarning($"Tile coord count mismatch: idx={index} totalTiles={totalTiles}");
            totalTiles = index;
        }

        for (int i = 0; i < totalTiles; i++)
        {
            int2 qr = tileCoords[i];
            float2 pos = new float2(qr.x, qr.y);

            float total = 0f;
            float amplitude = 1f;
            float frequency = _noiseFrequency;
            float maxValue = 0f;

            for (int o = 0; o < octaves; o++)
            {
                if (frequency <= 0.0001f) frequency = 0.0001f;

                float2 samplePos = (pos / frequency) + noiseOffset;
                float n = noise.snoise(samplePos);

                if (float.IsNaN(n) || float.IsInfinity(n)) n = 0;

                total += n * amplitude;

                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= _lacunarity;
            }

            float value = (total / maxValue) * 0.5f + 0.5f;

            int type;
            float height;

            if (value > _noiseThresholdMountain)
            {
                type = 3;
                height = value * _heightMultiplyerMountain;
            }
            else if (value > _noiseThresholdGrass)
            {
                type = 2;
                height = value * _heightMultiplyerGrass;
            }
            else if (value > _noiseThresholdSand)
            {
                type = 1;
                height = value * _heightMultiplyerSand;
            }
            else
            {
                type = 0;
                height = value * _heightMultiplyerWater;
            }

            tileTypes[i] = type;
            tileHeights[i] = height;
        }

        float hexWidth = 1f;
        float hexHeight = Mathf.Sqrt(3f) * 0.5f * hexWidth;

        for (int i = 0; i < totalTiles; i++)
        {
            int2 qr2 = tileCoords[i];
            int q = qr2.x;
            int r = qr2.y;

            float worldX = hexWidth * (q + r * 0.5f);
            float worldZ = hexHeight * r;

            Vector3 worldPos = new Vector3(worldX, tileHeights[i], worldZ);

            if (createTileEnum == CreateTileEnum.Instantiate)
            {
                switch (tileTypes[i])
                {
                    case 3: Instantiate(mountain, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 2: Instantiate(grass, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 1: Instantiate(sand, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 0: Instantiate(water, worldPos, mountain.transform.rotation, tileHolder); break;
                    default: Instantiate(grass, worldPos, mountain.transform.rotation, tileHolder); break;
                }
            }
            else if (createTileEnum == CreateTileEnum.ObjectPool)
            {
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
            else
            {

            }
        }

        tileCoords.Dispose();
        tileTypes.Dispose();
        tileHeights.Dispose();

        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
    public void CreateMap2()
    {
        Debug.Log("parallel");

        var temp = Time.realtimeSinceStartup;

        pool.ReturnAll();
        if (createTileEnum == CreateTileEnum.Instantiate) ClearPlacedGameObjects();

        int totalTiles = 3 * range * (range + 1) + 1;


        NativeArray<int> tileTypes = new NativeArray<int>(totalTiles, Allocator.TempJob);
        NativeArray<float> tileHeights = new NativeArray<float>(totalTiles, Allocator.TempJob);
        NativeArray<int2> tileCoords = new NativeArray<int2>(totalTiles, Allocator.TempJob);

        //Creates tilecords
        int index = 0;
        for (int q = -range; q <= range; q++)
        {
            for (int r = -range; r <= range; r++)
            {
                int s = -q - r;
                if (math.abs(s) > range) continue;

                tileCoords[index] = new int2(q, r);
                index++;
            }
        }
        if (index != totalTiles)
        {
            Debug.LogWarning($"Tile coord count mismatch: idx={index} totalTiles={totalTiles}");
            totalTiles = index;
        }

        var job = new TileMapGenerationJob
        {
            tileTypes = tileTypes,
            tileHeights = tileHeights,
            tileCoords = tileCoords,


            noiseOffset = UnityEngine.Random.Range(0, 10000),
            noiseFrequency = _noiseFrequency,

            thresholdMountain = _noiseThresholdMountain,
            thresholdGrass = _noiseThresholdGrass,
            thresholdSand = _noiseThresholdSand,

            heightMultiplierMountain = _heightMultiplyerMountain,
            heightMultiplierGrass = _heightMultiplyerGrass,
            heightMultiplierSand = _heightMultiplyerSand,
            heightMultiplierWater = _heightMultiplyerWater,

            octaves = octaves,
            persistence = persistence,
            lacunarity = _lacunarity
        };

        JobHandle handle = job.Schedule(totalTiles, 64);
        handle.Complete();

        float hexWidth = 1f;
        float hexHeight = Mathf.Sqrt(3f) * 0.5f * hexWidth;

        for (int i = 0; i < totalTiles; i++)
        {
            int2 qr = tileCoords[i];
            int q = qr.x;
            int r = qr.y;

            float worldX = hexWidth * (q + r * 0.5f);
            float worldZ = hexHeight * r;

            Vector3 worldPos = new Vector3(worldX, tileHeights[i], worldZ);

            if (createTileEnum == CreateTileEnum.Instantiate)
            {
                switch (tileTypes[i])
                {
                    case 3: Instantiate(mountain, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 2: Instantiate(grass, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 1: Instantiate(sand, worldPos, mountain.transform.rotation, tileHolder); break;
                    case 0: Instantiate(water, worldPos, mountain.transform.rotation, tileHolder); break;
                    default: Instantiate(grass, worldPos, mountain.transform.rotation, tileHolder); break;
                }
            }
            else if (createTileEnum == CreateTileEnum.ObjectPool)
            {
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
            else
            {

            }
        }

        tileCoords.Dispose();
        tileTypes.Dispose();
        tileHeights.Dispose();

        Debug.Log((Time.realtimeSinceStartup - temp) * 100);
    }
}