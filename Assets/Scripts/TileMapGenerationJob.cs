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
    public float noiseFrequency;
    public float noiseThresholdMountain;
    public float noiseThresholdGrass;
    public float noiseThresholdSand;
    public int noiseOffset;

    public float heightMultiplierMountain;
    public float heightMultiplierGrass;
    public float heightMultiplierSand;
    public float heightMultiplierWater;

   public void Execute(int index)
    {
        // Convert flat index -> axial coords (q, r)
        // Generate all axial coords first, store in a lookup array if needed.
        int diameter = range * 2 + 1;
        int q = (index % diameter) - range;
        int r = (index / diameter) - range;

        // Optional: enforce hex shape (otherwise you get a square map)
        if (Mathf.Abs(q + r) > range)
        {
            tileTypes[index] = -1; // mark invalid
            tileHeights[index] = 0;
            return;
        }

        // Noise
        float value = Mathf.PerlinNoise(
            (q + noiseOffset) / noiseFrequency,
            (r + range + noiseOffset) / noiseFrequency
        );

        int type;
        float height;
        if (value > noiseThresholdMountain)
        {
            type = 3; height = value * heightMultiplierMountain;
        }
        else if (value > noiseThresholdGrass)
        {
            type = 2; height = value * heightMultiplierGrass;
        }
        else if (value > noiseThresholdSand)
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

