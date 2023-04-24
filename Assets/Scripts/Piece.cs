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
    }

    private void OnMouseDrag()
    {
        Debug.Log("mouseDrag");
        if (Camera.main is null) return;


        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePosition.x, mousePosition.y);
    }
    
    private void OnMouseUp()
    {
        Debug.Log("mouseUp");
        isDragging = false;
        transform.position = _originalPosition;
    }
    #endregion
}
