using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region MouseActions
    [Header("Mouse Actions")]
    
    [SerializeField] private bool isDragging = false;
    private Vector2 _originalPosition;
    
    private void OnMouseDown()
    {
        Debug.Log("Mousedown");
        if (isDragging) return;
        
        isDragging = true;
        _originalPosition = transform.position;
        
        transform.parent.GetComponent<Board>().CheckAvailableTiles();   // check available tiles and tint its color
    }

    private void OnMouseDrag()
    {
        if (Camera.main is null) return;


        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePosition.x, mousePosition.y);
    }
    
    private void OnMouseUp()
    {
        Debug.Log("mouseUp");
        isDragging = false;
        transform.position = _originalPosition;
        
        transform.parent.GetComponent<Board>().ResetCheckTiles();   // reset color of available tiles
    }
    #endregion
}
