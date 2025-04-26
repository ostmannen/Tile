using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
            CreateMap();
        }
    }
    public void GetTile(Vector3Int pos){
        grid.WorldToCell(pos);
        tilemap.GetTile(pos);
    }
    public void CreateMap()
    {
        ClearPlacedGameObjects();
        PlaceObject();
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
                float value = Mathf.PerlinNoise((x + noiseOffset) / _noiseFrequency, (y + range + noiseOffset) / _noiseFrequency);
                Vector3Int cell = new Vector3Int(q, y, 0);
                Vector3 worldPosition = grid.CellToWorld(cell);
                if (value > _noiseThresholdMountain)
                {
                    Instantiate(mountain, worldPosition + new Vector3(0, (value + (Random.Range(0, 0.4f))) * _heightMultiplyerMountain, 0), sand.transform.rotation, transform);
                }
                else if (value > _noiseThresholdGrass)
                {
                    Instantiate(grass, worldPosition + new Vector3(0, (value + (Random.Range(-0.03f, 0.03f))) * _heightMultiplyerGrass, 0), sand.transform.rotation, transform);
                }
                else if (value > _noiseThresholdSand)
                {
                    Instantiate(sand, worldPosition + new Vector3(0, value * _heightMultiplyerSand, 0), sand.transform.rotation, transform);
                }
                else
                {
                    Instantiate(water, worldPosition + new Vector3(0, value * _heightMultiplyerWater, 0), sand.transform.rotation, transform);
                }
            }
        }
    }
}
