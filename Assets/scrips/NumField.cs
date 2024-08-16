using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumField : MonoBehaviour
{
    private Tile tile;
    private GameObject oneNum;

    public void setTile(Tile tile)
    {
        this.tile = tile;
    }

    public void onClose()
    {
        this.transform.parent.GetChild(0).gameObject.SetActive(false);
        this.transform.parent.GetChild(1).gameObject.SetActive(false);
        tile.setTouched(false);
    }

    public void setOne()
    {
        tile.displayNumbers(1);
    }
    public void setTwo()
    {
        tile.displayNumbers(2);
    }
    public void setThree()
    {
        tile.displayNumbers(3);
    }
    public void setFour()
    {
        tile.displayNumbers(4);
    }
    public void setFive()
    {
        tile.displayNumbers(5);
    }
    public void setSix()
    {
        tile.displayNumbers(6);
    }
    public void setSeven()
    {
        tile.displayNumbers(7);
    }
    public void setEight()
    {
        tile.displayNumbers(8);
    }
    public void setNine()
    {
        tile.displayNumbers(9);
    }
    public void setZero()
    {
        tile.displayNumbers(0);
    }
}
