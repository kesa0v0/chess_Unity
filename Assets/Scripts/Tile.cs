using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    
    public void SetAvailable()
    {
        // set color to green
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, tileColor, 0.75f);
    }
    
    public void SetUnavailable()
    {
        // set color to gray
        GetComponent<SpriteRenderer>().color = tileColor;
    }
    
    #endregion
}
