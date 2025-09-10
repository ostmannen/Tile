using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }
    [NonSerialized] public TileInfoEvents tileInfoEvents;
    public void Awake()
    {
        instance = this;
        tileInfoEvents = new TileInfoEvents();
    }
    public event Action<Vector3Int> OnGetTile;
    public void GetTile(Vector3Int pos)
    {
        if (OnGetTile != null)
        {
            OnGetTile(pos);
        }
    }
}
