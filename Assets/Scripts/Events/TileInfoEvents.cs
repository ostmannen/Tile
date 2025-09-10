using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TileInfoEvents
{
    public event Action<Vector3Int> OnGetTile;
    public void GetTile(Vector3Int pos)
    {
        if (OnGetTile != null)
        {
            OnGetTile(pos);
        }
    }
}
