using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Board : MonoBehaviour
{
    [Header("BoardSettings")] 
    public bool isBoardFlipped;

    [SerializeField] private int tileSize = 1;
    [SerializeField] private Vector2 anchorPosition;

    [SerializeField] private Color brightTileColor;
    [SerializeField] private Color darkTileColor;
    
    [Header("Settings")]
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tilePrefab;
    private readonly Dictionary<int, Tile> _tiles = new();

    [SerializeField] private List<Piece> piecePrefabs;

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
        
        
        // DEBUG
        // Set Pieces
        AddPiece<Pawn>(GetTileFromPos(44));
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
            _tiles[availableTile].tintMode = TintMode.Available;
            _tiles[availableTile].TintUpdate();
        }
        foreach (var killableTile in movableTilesCodes.KillableTile)
        {
            _tiles[killableTile].tintMode = TintMode.Killable;
            _tiles[killableTile].TintUpdate();
        }
    }
    
    public void ResetCheckTilesTint()
    {
        foreach (var tilePair in _tiles)
        {
            tilePair.Value.tintMode = TintMode.None;
            tilePair.Value.TintUpdate();
        }
    }

    private Tile _beforeCursoredTile;
    private TintMode _beforeCursoredTileTintMode;
    public void TintCursorOnTile(int x, int y)
    {
        // clear color of before cursored tile
        if (_beforeCursoredTile != null)
        {
            _beforeCursoredTile.tintMode = _beforeCursoredTileTintMode;
            _beforeCursoredTile.TintUpdate();
        }
        
        // set color of current cursored tile
        var tile = GetTileFromPos(10 * y + x);
        _beforeCursoredTile = tile;
        _beforeCursoredTileTintMode = tile.tintMode;
        
        tile.tintMode = TintMode.Selected;
        tile.TintUpdate();
    }
    
    #endregion

    #region PieceRelated

    public void AddPiece<T>(Tile tile) where T: Piece
    {
        var typeOfPiece = piecePrefabs.Find(p => p.GetType() == typeof(T));
        var piece = Instantiate(typeOfPiece, transform);
        
        MovePiece(piece, tile);
    }

    public void RemovePiece(Piece piece)
    {
        piece.currentTile.pieceOnTile = null;
        
        Destroy(piece.gameObject);
    }

    public void MovePiece(Piece piece, Tile toTile)
    {
        var tempPos = toTile.transform.position;
        tempPos.z += -5;
        piece.transform.position = tempPos;
        
        if (piece.currentTile != null)
            piece.currentTile.pieceOnTile = null;
        piece.currentTile = toTile;
        toTile.pieceOnTile = piece;
    }

    public bool IsPieceOnTile(int pos) // TODO: Move To GetPieceFromPos() because it can return null
    {
        // return _tiles[pos].isOccupied;
        return false;
    }

    #endregion

    #region Transformer

    public Tile GetTileFromPos(int pos)
    {
        return _tiles[pos];
    }

    public int GetPosFromCursor() // Effected By isFlipped
    {
        var pos = GetPosFromVec2(Camera.main!.ScreenToWorldPoint(Input.mousePosition));
        return pos;
    }
    
    public int GetPosFromVec2(Vector2 cursorPos)
    {
        var x = (int) cursorPos.x;
        var y = (int) cursorPos.y;
        return y * 10 + x;
    }

    public Piece GetPiece(int pos)
    {
        return _tiles[pos].pieceOnTile;
    }

    public Piece GetPiece(Tile tile)
    {
        return tile.pieceOnTile;
    }

    public IEnumerable<Piece> GetPieces<T>()
    {
        return null;    // TODO: Get kinds of Pieces
    }
    
    #endregion
}
