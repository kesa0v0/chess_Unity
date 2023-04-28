using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Board : MonoBehaviour
{
    [SerializeField] private Color brightTileColor;
    [SerializeField] private Color darkTileColor;
    
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tilePrefab;
    private readonly Dictionary<int, Tile> _tiles = new();

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize Chess Tiles ( 8 x 8 )
        for (var yValue = 0; yValue < 8; yValue++) // vertical
        {
            for (var xValue = 0; xValue < 8; xValue++) // horizontal
            {
                var temp = Instantiate(tilePrefab, this.transform);
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
    
    public void TintMovableTiles(Piece piece)
    {
        var movableTilesCodes = piece.movabletiles;
        foreach (var availableTile in movableTilesCodes.MovableTile)
        {
            _tiles[availableTile].tintMode = MoveMode.Available;
            _tiles[availableTile].TintUpdate();
        }
        foreach (var killableTile in movableTilesCodes.KillableTile)
        {
            _tiles[killableTile].tintMode = MoveMode.Killable;
            _tiles[killableTile].TintUpdate();
        }
        
        
    }
    
    public void ResetCheckTilesTint()
    {
        foreach (var tilePair in _tiles)
        {
            tilePair.Value.moveMode = MoveMode.None;
            tilePair.Value.TintUpdate();
        }
    }

    private Tile _beforeCursoredTile;
    private MoveMode _beforeCursoredTileTintMode;
    public void TintCursorOnTile(int x, int y)
    {
        // clear color of before cursored tile
        if (_beforeCursoredTile != null)
        {
            _beforeCursoredTile.tintMode = _beforeCursoredTileTintMode;
        }
        
        _tiles[y * 10 + x].TintUpdate();
        _beforeCursoredTile = _tiles[y * 10 + x];
        _beforeCursoredTileTintMode = _beforeCursoredTile.tintMode;
    }
    
    #endregion

    public void MovePieceTo(Tile toTile)
    {
        
    }

    public bool IsPieceOnTile(int pos)
    {
        // return _tiles[pos].isOccupied;
        return false;
    }
}
