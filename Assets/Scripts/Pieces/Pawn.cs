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
                if (Board.GetPiece(pos + 11) && Board.GetPiece(pos + 11).Team != Team)
                    movableTiles.AddKillable(pos + 11);
                if (Board.GetPiece(pos + 9) && Board.GetPiece(pos + 9).Team != Team)
                    movableTiles.AddKillable(pos + 9);
                
                if (!Board.GetPiece(pos + 10))
                    movableTiles.AddMovable(pos + 10);
                if (isFirstMove && !Board.GetPiece(pos + 20))
                    movableTiles.AddMovable(pos + 20);
                break;
            }
            case Team.Black:
            {
                if (Board.GetPiece(pos - 11) && Board.GetPiece(pos - 11).Team != Team)
                    movableTiles.AddKillable(pos - 11);
                if (Board.GetPiece(pos - 9) && Board.GetPiece(pos - 9).Team != Team)
                    movableTiles.AddKillable(pos - 9);
                
                if (!Board.GetPiece(pos - 10))
                    movableTiles.AddMovable(pos - 10);
                if (isFirstMove && !Board.GetPiece(pos - 20))
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
