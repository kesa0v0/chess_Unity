using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class MovableTiles
{
    public readonly List<int> MovableTile = new();
    public readonly List<int> KillableTile = new();
    
    public void Add(MovableTiles other)
    {
        MovableTile.AddRange(other.MovableTile);
        KillableTile.AddRange(other.KillableTile);
    }
}

[Serializable]
public abstract class Piece : MonoBehaviour
{
    private Board _board;

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
    
    // Start is called before the first frame update
    protected void Start()
    {
        _board = transform.parent.GetComponent<Board>();
    }

    public abstract MovableTiles GetMovableTilesCode();

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

        if (isDragging) return;
        isDragging = false;
        
        // onSelected
        Movabletiles = GetMovableTilesCode();

        _originalPosition = transform.position;
        _mouseClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        _board.TintMovableTiles(this);   // check available tiles and tint its color
    }

    private void OnMouseDrag()
    {
        if (Camera.main is null) return;
        
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

        _board.TintCursorOnTile(_draggingPosition.x, _draggingPosition.y); // Tint Current Cursor Tile
    }
    
    private void OnMouseUp()
    {
        Debug.Log("mouseUp");
        if (isDragging)
        {
            OnDragEnd();
        }
        else
        {
            OnClicked();
        }

        isDragging = false;
        _board.ResetCheckTilesTint();
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
        if (!Movabletiles.MovableTile.Contains(currentPos))
        {
            ResetPosition();
            return;
        }
        
        _board.MovePiece(this, _board.GetTileFromPos(currentPos));
    }

    private void OnClicked()
    {
        
        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.position = _originalPosition;
        isDragging = false;
        _board.ResetCheckTilesTint();
    }
    
    #endregion

    #region MovementCalculation

    public MovableTiles Movabletiles;
    
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
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
            if (_board.GetPiece(pos))
            {
                if (_board.GetPiece(pos)._team != _team) movableTiles.KillableTile.Add(pos);
                break;
            }
            
            movableTiles.MovableTile.Add(pos);
        }

        return movableTiles;
    }

    #endregion
}
