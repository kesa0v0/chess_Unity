using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MoveMode  // MoveMode + TintMode
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

    public bool isOccupied;
    public bool isPieceAvailable;
    public MoveMode moveMode = MoveMode.None;
    public MoveMode tintMode = MoveMode.None;

    private void Start()
    {
        _board = transform.parent.GetComponent<Board>();
    }

    
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

    private void OnMouseDown()
    {
        if (moveMode == MoveMode.None) return; // TODO: Show "Not Available" Floating Text
        
        
        
        _board.MovePieceTo(this);
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
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.blue, tileColor, 0.75f);
    }

    public void TintUpdate()
    {
        switch (tintMode)
        {
            case MoveMode.Available:
                SetAvailableColor();
                break;
            case MoveMode.Killable:
                SetOccupiedColor();
                break;
            case MoveMode.Selected:
                SetSelectedColor();
                break;
            case MoveMode.None:
                ResetColor();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tintMode), tintMode, "tintMode is Unknown State.");
        }
    }
    
    #endregion
}
