using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        // TileCode: YX (Y: Column, X: Row)
        // TODO: Implement Pawn's Movable Tiles

        // for test, return Queen's movable tiles
        
        var movableTiles = new MovableTiles();

        var position = transform.position;
        
        movableTiles.Add(HorizontalMovement());
        movableTiles.Add(VerticalMovement());
        movableTiles.Add(DiagonalMovement());

        return movableTiles;
    }
}
