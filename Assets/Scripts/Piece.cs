using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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


public abstract class Piece : MonoBehaviour
{
    private Board _board;
    // Start is called before the first frame update
    protected void Start()
    {
        _board = transform.parent.GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        movabletiles = GetMovableTilesCode();

        _originalPosition = transform.position;
        _mouseClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        _board.TintMovableTiles(this);   // check available tiles and tint its color
    }

    private void OnMouseDrag()
    {
        if (Camera.main is null) return;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Make Small Movement is not count as Drag
        if (!isDragging && Vector2.Distance(mousePosition, _mouseClickPosition) < 0.1f) return;

        
        // on Dragging
        isDragging = true;
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
            onDragEnd();
        }
        else
        {
            onClicked();
        }
        
        _board.ResetCheckTilesTint();
    }

    private void onDragEnd()
    {
        if (!_isInBoard)
        {
            ResetPosition();
            return;
        }
        
    }

    private void onClicked()
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

    public MovableTiles movabletiles;
    
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
            if (_board.IsPieceOnTile(currentY * 10 + (currentX - x)))
            {
                movableTiles.KillableTile.Add(currentY * 10 + (currentX - x));
                break;
            }
            
            movableTiles.MovableTile.Add(currentY * 10 + (currentX - x));
        }
        // right movement
        for (var x = 1; x < 8; x++)
        {
            // board border check
            if (currentX + x > 7) break;
            // blocking piece check
            if (_board.IsPieceOnTile(currentY * 10 + (currentX + x)))
            {
                movableTiles.KillableTile.Add(currentY * 10 + (currentX + x));
                break;
            }
            
            movableTiles.MovableTile.Add(currentY * 10 + (currentX + x));
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
            if (_board.IsPieceOnTile((currentY + y) * 10 + currentX))
            {
                movableTiles.KillableTile.Add((currentY + y) * 10 + currentX);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY + y) * 10 + currentX);
        }
        // down movement
        for (var y = 1; y < 8; y++)
        {
            // board border check
            if (currentY - y < 0) break;
            // blocking piece check
            if (_board.IsPieceOnTile((currentY - y) * 10 + currentX))
            {
                movableTiles.KillableTile.Add((currentY - y) * 10 + currentX);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY - y) * 10 + currentX);
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
            if (_board.IsPieceOnTile((currentY + i) * 10 + currentX - i))
            {
                movableTiles.KillableTile.Add((currentY + i) * 10 + currentX - i);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY + i) * 10 + currentX - i);
        }
        // left-down diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX - i < 0 || currentY - i < 0) break;
            // blocking piece check
            if (_board.IsPieceOnTile((currentY - i) * 10 + currentX - i))
            {
                movableTiles.KillableTile.Add((currentY - i) * 10 + currentX - i);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY - i) * 10 + currentX - i);
        }
        // right-up diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX + i > 7 || currentY + i > 7) break;
            // blocking piece check
            if (_board.IsPieceOnTile((currentY + i) * 10 + currentX + i))
            {
                movableTiles.KillableTile.Add((currentY + i) * 10 + currentX + i);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY + i) * 10 + currentX + i);
        }
        // right-down diagonal movement
        for (var i = 1; i < 8; i++)
        {
            // board border check
            if (currentX + i > 7 || currentY - i < 0) break;
            // blocking piece check
            if (_board.IsPieceOnTile((currentY - i) * 10 + currentX + i))
            {
                movableTiles.KillableTile.Add((currentY - i) * 10 + currentX + i);
                break;
            }
            
            movableTiles.MovableTile.Add((currentY - i) * 10 + currentX + i);
        }

        return movableTiles;
    }

    #endregion
}
