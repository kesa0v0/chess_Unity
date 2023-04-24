using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private Board _board;
    // Start is called before the first frame update
    void Start()
    {
        _board = transform.parent.GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region MouseActions
    [Header("Mouse Actions")]
    
    [SerializeField] private bool isDragging = false;
    private Vector2 _originalPosition;
    private Vector2 _mouseClickPosition;
    
    private Vector2Int _draggingPosition;
    
    private void OnMouseDown()
    {
        if (Camera.main is null) return;

        Debug.Log("Mousedown");
        if (isDragging) return;
        isDragging = false;

        _originalPosition = transform.position;
        _mouseClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        _board.CheckAvailableTiles();   // check available tiles and tint its color
    }

    private void OnMouseDrag()
    {
        if (Camera.main is null) return;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Make Small Movement is not count as Drag
        if (!isDragging && Vector2.Distance(mousePosition, _mouseClickPosition) < 0.1f) return;

        
        // Dragging
        isDragging = true;
        transform.position = mousePosition;
        
        _draggingPosition.x = Mathf.RoundToInt(mousePosition.x);
        _draggingPosition.y = Mathf.RoundToInt(mousePosition.y);
        
        // if x or y is out of range then ignore
        if (_draggingPosition.x < 0 || _draggingPosition.x > 7 || _draggingPosition.y < 0 || _draggingPosition.y > 7) return;

        _board.TintCursorOnTile(_draggingPosition.x, _draggingPosition.y);
    }
    
    private void OnMouseUp()
    {
        Debug.Log("mouseUp");
        isDragging = false;
        transform.position = _originalPosition;
        
        _board.ResetCheckTiles();   // reset color of available tiles
    }
    #endregion
}
