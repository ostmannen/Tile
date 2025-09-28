using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct TileMapGenerationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<int2> tileCoords;
    [WriteOnly] public NativeArray<int> tileTypes;
    [WriteOnly] public NativeArray<float> tileHeights;

    public int noiseOffset;
    public float noiseFrequency;

    public float thresholdMountain;
    public float thresholdGrass;
    public float thresholdSand;

    public int octaves;
    public float persistence;
    public float lacunarity;


    public float heightMultiplierMountain;
    public float heightMultiplierGrass;
    public float heightMultiplierSand;
    public float heightMultiplierWater;

    public void Execute(int index)
    {
        int2 qr = tileCoords[index];
        float2 pos = new float2(qr.x, qr.y);

        float total = 0f;
        float amplitude = 1f;
        float frequency = noiseFrequency;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            if (frequency <= 0.0001f) frequency = 0.0001f;

            float2 samplePos = (pos / frequency) + noiseOffset;
            float n = noise.snoise(samplePos);

            if (float.IsNaN(n) || float.IsInfinity(n)) n = 0;

            total += n * amplitude;

            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        
        float value = (total / maxValue) * 0.5f + 0.5f;

        int type;
        float height;

        if (value > thresholdMountain)
        {
            type = 3;
            height = value * heightMultiplierMountain;
        }
        else if (value > thresholdGrass)
        {
            type = 2;
            height = value * heightMultiplierGrass;
        }
        else if (value > thresholdSand)
        {
            type = 1;
            height = value * heightMultiplierSand;
        }
        else
        {
            type = 0;
            height = value * heightMultiplierWater;
        }

        tileTypes[index] = type;
        tileHeights[index] = height;
    }
}
