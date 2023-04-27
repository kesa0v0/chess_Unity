using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Color brightTileColor;
    [SerializeField] private Color darkTileColor;
    
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tile;
    private readonly Dictionary<int, Tile> _tiles = new();

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize Chess Tiles ( 8 x 8 )
        for (var yValue = 0; yValue < 8; yValue++) // vertical
        {
            for (var xValue = 0; xValue < 8; xValue++) // horizontal
            {
                var temp = Instantiate(tile, this.transform);
                temp.transform.transform.position = new Vector3(xValue, yValue, 5);
                _tiles.Add(yValue * 10 + xValue, temp.GetComponent<Tile>()); // 00 ~ 77
                temp.SetPosition(xValue, yValue);

                temp.GetComponent<Tile>() // set tile color
                    .TileColor = (xValue % 2 + yValue) % 2 == 1
                    ? darkTileColor
                    : brightTileColor;
            }
        }
        
        // Set Turns who moves first
        currentTurn = Team.White;
    }
    
    public void TurnOver()
    {
        switch (currentTurn)
        {
            case Team.White:
                currentTurn = Team.Black;
                break;
            case Team.Black:
                currentTurn = Team.White;
                break;
            case Team.Unknown:
            default:
                Debug.LogError("Turn is Unknown State.");
                break;
        }
    }
    
    #region TileRelated
    
    public void CheckAvailableTiles()
    {
        foreach (var tilePair in _tiles)
        {
            if (tilePair.Value.IsPlaceAvailable())
                tilePair.Value.SetAvailableColor();
        }
    }
    
    public void ResetCheckTiles()
    {
        foreach (var tilePair in _tiles)
        {
            tilePair.Value.ResetColor();
        }
    }

    private Tile _beforeCursoredTile;
    public void TintCursorOnTile(int x, int y)
    {
        // clear color of before cursored tile
        if (_beforeCursoredTile != null)
        {
            if (_beforeCursoredTile.IsPlaceAvailable())
                _beforeCursoredTile.SetAvailableColor();
            else
                _beforeCursoredTile.ResetColor();
        }
        
        _tiles[y * 10 + x].SetSelectedColor();
        _beforeCursoredTile = _tiles[y * 10 + x];
    }
    
    #endregion
}
