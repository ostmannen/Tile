using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour, IClickable
{
    private TileEnum tileEnum;

    public bool OnClick()
    {
        return false;
    }
    void Start()
    {
        /*
        if (tileEnum == TileEnum.Grass){
            transform.tag = "Grass";
        }
        else if (tileEnum == TileEnum.mountain){
            transform.tag = "Mountain";
        }
        else if (tileEnum == TileEnum.Sand){
            transform.tag = "Sand";
        }
        else if (tileEnum == TileEnum.Water){
            transform.tag = "Water";
        }*/
    }
}
