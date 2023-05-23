using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovableTiles
{
    public List<int> MovableTile = new();
    public List<int> KillableTile = new();
    
    public int EnPassantTile;
    public int CastlingTile;
    
    // TODO: Event Special Movement (Castling, En Passant, Promotion)

    public void Add(MovableTiles other)
    {
        MovableTile.AddRange(other.MovableTile);
        KillableTile.AddRange(other.KillableTile);
    }
    
    public void AddMovable(int tile)
    {
        MovableTile.Add(tile);
    }
    
    public void AddKillable(int tile)
    {
        KillableTile.Add(tile);
    }
    
}

[Serializable]
public abstract class Piece : MonoBehaviour
{
    protected Board Board;

    [SerializeField] protected Sprite whiteSprite;
    [SerializeField] protected Sprite blackSprite;
    
    private Team _team;
    public Team Team
    {
        get => _team;
        set
        {
            _team = value;
            GetComponent<SpriteRenderer>().sprite = _team == Team.White ? whiteSprite : blackSprite;
        }
    }
    public Tile currentTile;
    
    public bool isFirstMove = true;

    public MovableTiles movabletiles;
    

    
    // Start is called before the first frame update
    protected void Awake()
    {
        Board = transform.parent.GetComponent<Board>();
    }

    protected abstract MovableTiles GetMovableTilesCode();

    public void UpdateMovableTilesCode()
    {
        movabletiles = GetMovableTilesCode();
    }

    #region MouseActions
    [Header("Mouse Actions")]
    
    [SerializeField] private bool isDragging;
    private Vector2 _originalPosition;
    private Vector2 _mouseClickPosition;

    private bool _isInBoard;
    private Vector2Int _draggingPosition;

    private void OnMouseDown()
    {
        if (Camera.main is null) return;
        
        if (!Board.CheckTurn(this)) return;

        if (isDragging) return;
        isDragging = false;

        // onSelected
        // movabletiles = GetMovableTilesCode();

        _originalPosition = transform.position;
        _mouseClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Board.ClearTintCursorHistory();
        Board.TintMovableTiles(this);   // check available tiles and tint its color
    }

    private void OnMouseDrag()
    {
        if (Camera.main is null) return;
        
        if (!Board.CheckTurn(this)) return; 
        
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Make Small Movement is not count as Drag
        if (!isDragging && Vector2.Distance(mousePosition, _mouseClickPosition) < 0.095f) return;

        
        // on Dragging
        isDragging = true;
        mousePosition.z = -15;
        transform.position = mousePosition;
        
        _draggingPosition.x = Mathf.RoundToInt(mousePosition.x);
        _draggingPosition.y = Mathf.RoundToInt(mousePosition.y);
        
        // if x or y is out of range then ignore
        if (_draggingPosition.x < 0 || _draggingPosition.x > 7 || _draggingPosition.y < 0 || _draggingPosition.y > 7)
        {
            _isInBoard = false;
            return;
        }
        _isInBoard = true;

        Board.TintCursorOnTile(_draggingPosition.x, _draggingPosition.y); // Tint Current Cursor Tile
    }
    
    private void OnMouseUp()
    {
        if (!Board.CheckTurn(this)) return;

        if (isDragging)
        {
            OnDragEnd();
            Board.ResetCheckTilesTint();
        }
        else
        {
            OnClicked();
        }

        isDragging = false;
    }

    private void OnDragEnd()
    {
        var currentPos = Board.GetPosFromCursor();
        
        // Check in Board or Not Moved
        if (!_isInBoard || Board.GetPosFromCursor() == Board.GetPosFromVec2(_originalPosition))
        {
            ResetPosition();
            return;
        }
        
        // Check is Movable tile
        if (movabletiles.MovableTile.Contains(currentPos))
        {
            Board.MovePiece(this, Board.GetTileFromPos(currentPos));
            Board.TurnOver();
        }
        // Check is Killable tile
        else if (movabletiles.KillableTile.Contains(currentPos))
        {
            Board.KillPiece(this, Board.GetPiece(currentPos));
            Board.TurnOver();
        }
        // // Check is Enpassant tile
        // else if (movabletiles.EnPassantTile == currentPos)
        // {
        //     // Board.EnPassant(this, Board.GetPiece(currentPos));
        //     Board.TurnOver();
        // }
        else
        {
            ResetPosition();
        }
    }

    private void OnClicked()
    {
        
        transform.position = _originalPosition;
    }

    private void ResetPosition()
    {
        transform.position = _originalPosition;
        isDragging = false;
        Board.ResetCheckTilesTint();
    }
    
    #endregion

    #region MovementCalculation
    
    public MovableTiles HorizontalMovement()
    {
        var movableTiles = new MovableTiles();
        var position = transform.position;
        var currentX = Mathf.RoundToInt(position.x);
        var currentY = Mathf.RoundToInt(position.y);
        
        // left movement
        for (var x = 1; x < 8; x++)
        {
            // board border check
            if (currentX - x < 0) break;
            // blocking piece check
            var pos = currentY * 10 + (currentX - x);
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }
        // right movement
        for (var x = 1; x < 8; x++)
        {
            // board border check
            if (currentX + x > 7) break;
            // blocking piece check
            var pos = currentY * 10 + (currentX + x);
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }

        return movableTiles;
    }

    public MovableTiles VerticalMovement()
    {
        var movableTiles = new MovableTiles();
        var position = transform.position;
        var currentX = Mathf.RoundToInt(position.x);
        var currentY = Mathf.RoundToInt(position.y);
        
        // up movement
        for (var y = 1; y < 8; y++)
        {
            // board border check
            if (currentY + y > 7) break;
            // blocking piece check
            var pos = (currentY + y) * 10 + currentX;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }
        // down movement
        for (var y = 1; y < 8; y++)
        {
            // board border check
            if (currentY - y < 0) break;
            // blocking piece check
            var pos = (currentY - y) * 10 + currentX;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }


        return movableTiles;
    }
    
    public MovableTiles DiagonalMovement()
    {
        var movableTiles = new MovableTiles();
        var position = transform.position;
        var currentX = Mathf.RoundToInt(position.x);
        var currentY = Mathf.RoundToInt(position.y);
        
        // left-up diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX - i < 0 || currentY + i > 7) break;
            // blocking piece check
            var pos = (currentY + i) * 10 + currentX - i;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }
        // left-down diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX - i < 0 || currentY - i < 0) break;
            // blocking piece check
            var pos = (currentY - i) * 10 + currentX - i;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }
        // right-up diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX + i > 7 || currentY + i > 7) break;
            // blocking piece check
            var pos = (currentY + i) * 10 + currentX + i;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }
        // right-down diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX + i > 7 || currentY - i < 0) break;
            // blocking piece check
            var pos = (currentY - i) * 10 + currentX + i;
            if (Board.GetPiece(pos))
            {
                if (Board.GetPiece(pos).Team != Team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }

        return movableTiles;
    }

    #endregion
}
