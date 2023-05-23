using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TintMode  // MoveMode + TintMode
{
    Available,
    Killable,
    Selected,
    None
}

public class Tile : MonoBehaviour
{
    private Board _board;
    
    [SerializeField] private int x;
    [SerializeField] private int y;

    public Piece pieceOnTile;
    public TintMode tintMode = TintMode.None;

    private void Awake()
    {
        _board = transform.parent.GetComponent<Board>();
    }

    
    public void SetPosition(int xValue, int yValue)
    {
        x = xValue;
        y = yValue;
    }

    public int GetPosition()
    {
        return y * 10 + x;
    }
    

    #region TileTinting
    
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

    private void SetAvailableColor()
    {
        // set color to green
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, tileColor, 0.75f);
    }
    
    private void ResetColor()
    {
        // set color to gray
        GetComponent<SpriteRenderer>().color = tileColor;
    }
    
    private void SetOccupiedColor()
    {
        // set color to red
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, tileColor, 0.75f);
    }
    
    private void SetSelectedColor()
    {
        // set color to blue
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.cyan, tileColor, 0.75f);
    }

    public void TintUpdate()
    {
        switch (tintMode)
        {
            case TintMode.Available:
                SetAvailableColor();
                break;
            case TintMode.Killable:
                SetOccupiedColor();
                break;
            case TintMode.Selected:
                SetSelectedColor();
                break;
            case TintMode.None:
                ResetColor();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tintMode), tintMode, "tintMode is Unknown State.");
        }
    }
    
    #endregion
}
