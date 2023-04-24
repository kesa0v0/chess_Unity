using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    
    public void SetPosition(int xValue, int yValue)
    {
        x = xValue;
        y = yValue;
    }
    public int GetX() => x;
    public int GetY() => y;
    

    #region TileAvailableCheck
    
    [SerializeField] private Color tileColor;

    public Color TileColor
    {
        get => tileColor;
        set
        {
            GetComponent<SpriteRenderer>().color = value;
            tileColor = value;
        }
    }

    public bool IsPlaceAvailable()
    {
        // if already occupied?
        if (false) return false;
        
        // if not occupied, is it available?
        if (false) return false;
        
        // then ok!
        return true;
    }
    
    public void SetAvailableColor()
    {
        // set color to green
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, tileColor, 0.75f);
    }
    
    public void ResetColor()
    {
        // set color to gray
        GetComponent<SpriteRenderer>().color = tileColor;
    }
    
    public void SetOccupiedColor()
    {
        // set color to red
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, tileColor, 0.75f);
    }
    
    public void SetSelectedColor()
    {
        // set color to blue
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.blue, tileColor, 0.75f);
    }
    
    #endregion
}
