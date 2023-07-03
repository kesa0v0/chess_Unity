using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("BoardSettings")] 
    public bool isBoardFlipped;

    [SerializeField] private SpriteRenderer whiteCheckIndicator;
    [SerializeField] private SpriteRenderer blackCheckIndicator;
    [SerializeField] private SpriteRenderer turnIndicatorSpriteRenderer;
    [SerializeField] private SpriteRenderer boardSpriteRenderer;
    [SerializeField] private Color boardColor;
    [SerializeField] private Color brightTileColor;
    [SerializeField] private Color darkTileColor;
    
    [Header("Settings")]
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tilePrefab;
    private readonly Dictionary<int, Tile> _tiles = new();

    [SerializeField] private List<Piece> piecePrefabs;
    private readonly List<Piece> _whitePieces = new();
    private readonly List<Piece> _blackPieces = new();
    
    public readonly Dictionary<int, Pawn> EnPassantPawns = new();
    public readonly Dictionary<int, Pawn> TempEnPassantPawns = new();

    public bool isWhiteChecked;
    public bool isBlackChecked;

    // Start is called before the first frame update
    private void Start()
    {
        // Set Board Color
        boardSpriteRenderer.color = boardColor;

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
        turnIndicatorSpriteRenderer.color = Color.white;
        
        // DEBUG
        // Set Pieces
        InitPieceSet();

        _whitePieces.ForEach(piece => piece.UpdateMovableTilesCode());
        _blackPieces.ForEach(piece => piece.UpdateMovableTilesCode());
    }

    private void InitPieceSet()
    {
        for (var i = 0; i < 8; i++)
        {
            AddPiece<Pawn>(GetTileFromPos(10 + i), Team.White);
            AddPiece<Pawn>(GetTileFromPos(60 + i), Team.Black);
        }
        
        AddPiece<Rook>(GetTileFromPos(00), Team.White);
        AddPiece<Knight>(GetTileFromPos(01), Team.White);
        AddPiece<Bishop>(GetTileFromPos(02), Team.White);
        AddPiece<Queen>(GetTileFromPos(03), Team.White);
        AddPiece<King>(GetTileFromPos(04), Team.White);
        AddPiece<Bishop>(GetTileFromPos(05), Team.White);
        AddPiece<Knight>(GetTileFromPos(06), Team.White);
        AddPiece<Rook>(GetTileFromPos(07), Team.White);
        
        AddPiece<Rook>(GetTileFromPos(70), Team.Black);
        AddPiece<Knight>(GetTileFromPos(71), Team.Black);
        AddPiece<Bishop>(GetTileFromPos(72), Team.Black);
        AddPiece<Queen>(GetTileFromPos(73), Team.Black);
        AddPiece<King>(GetTileFromPos(74), Team.Black);
        AddPiece<Bishop>(GetTileFromPos(75), Team.Black);
        AddPiece<Knight>(GetTileFromPos(76), Team.Black);
        AddPiece<Rook>(GetTileFromPos(77), Team.Black);
        
    }

    public bool CheckTurn(Piece piece)
    {
        return piece != null && piece.Team == currentTurn;
    }

    public void TurnOver()
    {
        switch (currentTurn)
        {
            case Team.White:
                currentTurn = Team.Black;
                turnIndicatorSpriteRenderer.color = Color.black;
        
                _whitePieces.ForEach(piece => piece.UpdateMovableTilesCode());
                EnPassantPawns.AddRange(TempEnPassantPawns);
                TempEnPassantPawns.Clear();
                _blackPieces.ForEach(piece => piece.UpdateMovableTilesCode());
                
                isWhiteChecked = IsDangerZone(GetPieces<King>(Team.White)[0].currentTile, Team.White);
                whiteCheckIndicator.gameObject.SetActive(isWhiteChecked);
                isBlackChecked = IsDangerZone(GetPieces<King>(Team.Black)[0].currentTile, Team.Black);
                blackCheckIndicator.gameObject.SetActive(isBlackChecked);
                break;
            case Team.Black:
                currentTurn = Team.White;
                turnIndicatorSpriteRenderer.color = Color.white;
                
                _blackPieces.ForEach(piece => piece.UpdateMovableTilesCode());
                EnPassantPawns.AddRange(TempEnPassantPawns);
                TempEnPassantPawns.Clear();
                _whitePieces.ForEach(piece => piece.UpdateMovableTilesCode());
                
                isWhiteChecked = IsDangerZone(GetPieces<King>(Team.White)[0].currentTile, Team.White);
                whiteCheckIndicator.gameObject.SetActive(isWhiteChecked);
                isBlackChecked = IsDangerZone(GetPieces<King>(Team.Black)[0].currentTile, Team.Black);
                blackCheckIndicator.gameObject.SetActive(isBlackChecked);
                break;
            case Team.Unknown:
            default:
                Debug.LogError("Turn is Unknown State.");
                break;
        }
    }

    #region TileTintRelated
    
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

        if (movableTilesCodes.KingSideCastling != 0)
        {
            _tiles[movableTilesCodes.KingSideCastling].tintMode = TintMode.Available;
            _tiles[movableTilesCodes.KingSideCastling].TintUpdate();
        }
        if (movableTilesCodes.QueenSideCastling != 0)
        {
            _tiles[movableTilesCodes.QueenSideCastling].tintMode = TintMode.Available;
            _tiles[movableTilesCodes.QueenSideCastling].TintUpdate();
        }
        if (movableTilesCodes.EnPassantMoveTile != 0)
        {
            _tiles[movableTilesCodes.EnPassantMoveTile].tintMode = TintMode.Available;
            _tiles[movableTilesCodes.EnPassantMoveTile].TintUpdate();
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
    
    public void ClearTintCursorHistory()
    {
        _beforeCursoredTile = null;
        _beforeCursoredTileTintMode = TintMode.None;
    }
    
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

    public void AddPiece<T>(Tile tile, Team team) where T: Piece
    {
        var typeOfPiece = piecePrefabs.Find(p => p.GetType() == typeof(T));
        var piece = Instantiate(typeOfPiece, transform);
        piece.Team = team;
        
        var tempPos = tile.transform.position;
        tempPos.z = -10;
        piece.transform.position = tempPos;
        piece.currentTile = tile;
        tile.pieceOnTile = piece;
        
        switch (team)
        {
            case Team.White:
                _whitePieces.Add(piece);
                break;
            case Team.Black:
                _blackPieces.Add(piece);
                break;
            case Team.Unknown:
            default:
                Debug.LogError("Team is Unknown State.");
                break;
        }
    }

    public void RemovePiece(Piece piece)
    {
        piece.currentTile.pieceOnTile = null;
        switch (piece.Team)
        {
            case Team.White:
                _whitePieces.Remove(piece);
                break;
            case Team.Black:
                _blackPieces.Remove(piece);
                break;
            case Team.Unknown:
            default:
                Debug.LogError("Team is Unknown State.");
                break;
        }
        Destroy(piece.gameObject);
    }

    public void MovePiece(Piece piece, Tile toTile)
    {
        var tempPos = toTile.transform.position;
        tempPos.z = -10;
        piece.transform.position = tempPos;
        
        piece.currentTile.pieceOnTile = null;
        piece.currentTile = toTile;
        toTile.pieceOnTile = piece;

        piece.isFirstMove = false;

        if (piece is Pawn pawn)
        {
            pawn.CheckPromotion();
        }
        
    }

    public void KillPiece(Piece fromPiece, Piece toPiece)
    {
        var toPieceTile = toPiece.currentTile;
        RemovePiece(toPiece);
        MovePiece(fromPiece, toPieceTile);
    }

    public void EnPassant(Piece fromPiece, int toTile)
    {
        RemovePiece(EnPassantPawns[toTile]);
        EnPassantPawns.Remove(toTile);
        MovePiece(fromPiece, GetTileFromPos(toTile));
    }

    public void Promotion(Pawn pawn)
    {
        var tile = pawn.currentTile;
        RemovePiece(pawn);
        AddPiece<Queen>(tile, pawn.Team);
    }

    public void Castling(King king, Rook targetRook, bool isQueenSide)
    {
        // is QueenSide Castling
        if (isQueenSide)
        {
            MovePiece(king, GetTileFromPos(king.currentTile.GetPosition() - 2));
            MovePiece(targetRook, GetTileFromPos(targetRook.currentTile.GetPosition() + 3));
        }
        // is KingSide Castling
        else
        {
            MovePiece(king, GetTileFromPos(king.currentTile.GetPosition() + 2));
            MovePiece(targetRook, GetTileFromPos(targetRook.currentTile.GetPosition() - 2));
        }
    }

    #endregion

    #region Transformer

    public Tile GetTileFromPos(int pos)
    {
        return _tiles[pos];
    }

    public static int GetPosFromCursor() // Effected By isFlipped
    {
        var pos = GetPosFromVec2(Camera.main!.ScreenToWorldPoint(Input.mousePosition));
        return pos;
    }
    
    public static int GetPosFromVec2(Vector2 cursorPos)
    {
        var x = Mathf.RoundToInt(cursorPos.x);
        var y = Mathf.RoundToInt(cursorPos.y);
        return y * 10 + x;
    }

    public Piece GetPiece(int pos)
    {
        return _tiles.TryGetValue(pos, out var tile)? tile.pieceOnTile : null;
    }

    public Piece GetPiece(Tile tile)
    {
        return tile.pieceOnTile;
    }

    public TileKind GetKindOfTile(Piece piece, int pos)
    {
        if (!GetPiece(pos) && EnPassantPawns.ContainsKey(pos) && EnPassantPawns[pos].Team != piece.Team)
            return TileKind.EnPassant;
        if (!GetPiece(pos))
            return TileKind.Movable;
        if (GetPiece(pos) && GetPiece(pos).Team != piece.Team)
            return TileKind.Killable;
        if (GetPiece(pos) && GetPiece(pos).Team == piece.Team)
            return TileKind.Obstacle;
        return TileKind.None;
    }

    public List<Piece> GetPieces<T>(Team team = Team.Unknown) where T : Piece
    {
        if (team != Team.Unknown)
            return team == Team.White ? _whitePieces.FindAll(p => p is T) : _blackPieces.FindAll(p => p is T);
        {
            var pieces = _whitePieces.FindAll(p => p is T);
            pieces.AddRange(_blackPieces.FindAll(p => p is T));
            return pieces;
        }
    }
    
    public bool IsDangerZone(Tile tile, Team team)
    {
        var pieces = team == Team.White ? _blackPieces : _whitePieces;
        
        foreach (var piece in pieces)
        {
            switch (piece)
            {
                case King:
                    var king = piece.currentTile.GetPosition();
                    // King can't move to front of enemy's King
                    var checkList = new List<int>
                    {
                        king + 10,
                        king - 10,
                        king + 1,
                        king - 1,
                        king + 11,
                        king - 11,
                        king + 9,
                        king - 9
                    };
                    if (checkList.Contains(tile.GetPosition()))
                    {
                        return true;
                    }
                    break;
                
                case Pawn:
                    var pawn = piece.currentTile.GetPosition();
                    if (piece.Team == Team.White && pawn == tile.GetPosition() - 9 ||
                        piece.Team == Team.White && pawn == tile.GetPosition() - 11 ||
                        piece.Team == Team.Black && pawn == tile.GetPosition() + 9 ||
                        piece.Team == Team.Black && pawn == tile.GetPosition() + 11)
                    {
                        return true;
                    }
                    break;
                
                default:
                {
                    var movable = piece.movabletiles;
                    if (movable.MovableTile.Contains(tile.GetPosition()) || movable.KillableTile.Contains(tile.GetPosition()))
                    {
                        return true;
                    }

                    break;
                }
            }
        }

        return false;
    }
    
    #endregion

    public void RaiseCheckMate()
    {
        Debug.Log("CheckMate");
    }
}
