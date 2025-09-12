using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct TileMapGenerationJob : IJobParallelFor
{
    [WriteOnly] public NativeArray<int> tileTypes;
    [WriteOnly] public NativeArray<float> tileHeights;

    public int range;
    public int noiseOffset;
    public float noiseFrequency;

    public float thresholdMountain;
    public float thresholdGrass;
    public float thresholdSand;

    public float heightMultiplierMountain;
    public float heightMultiplierGrass;
    public float heightMultiplierSand;
    public float heightMultiplierWater;

    public void Execute(int index)
    {
        int diameter = range * 2 + 1;
        int q = (index % diameter) - range;
        int r = (index / diameter) - range;

        if (Mathf.Abs(q + r) > range)
        {
            tileTypes[index] = -1;
            tileHeights[index] = 0;
            return;
        }

        float value = Mathf.PerlinNoise(
            (q + noiseOffset) / noiseFrequency,
            (r + range + noiseOffset) / noiseFrequency
        );

        int type;
        float height;
        if (value > thresholdMountain)
        {
            type = 3; height = value * heightMultiplierMountain;
        }
        else if (value > thresholdGrass)
        {
            type = 2; height = value * heightMultiplierGrass;
        }
        else if (value > thresholdSand)
        {
            type = 1; height = value * heightMultiplierSand;
        }
        else
        {
            type = 0; height = value * heightMultiplierWater;
        }

        tileTypes[index] = type;
        tileHeights[index] = height;
    }
}
