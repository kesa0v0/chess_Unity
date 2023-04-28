using System;
using UnityEngine;

public class Pawn : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        // TileCode: YX (Y: Column, X: Row)
        
        var movableTiles = new MovableTiles();

        var pos = Board.GetPosFromVec2(transform.position);

        switch (Team)
        {
            case Team.White:
            {
                movableTiles.AddMovable(pos + 10);
                if (isFirstMove)
                    movableTiles.AddMovable(pos + 20);
                break;
            }
            case Team.Black:
            {
                movableTiles.AddMovable(pos - 10);
                if (isFirstMove)
                    movableTiles.AddMovable(pos - 20);
                break;
            }
            case Team.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return movableTiles;
    }

    public void EnPassant()
    {
        
    }
    
    public void Promotion()
    {
        
    }
}
