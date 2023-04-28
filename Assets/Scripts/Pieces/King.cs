using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class King : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        var pos = Board.GetPosFromVec2(transform.position);
        
        var movablePos = new List<int>
        {
            pos + 10,
            pos - 10,
            pos + 1,
            pos - 1,
            pos + 11,
            pos - 11,
            pos + 9,
            pos - 9
        };
        var movablePosCopy = new List<int>(movablePos);

        foreach (var p in movablePosCopy)
        {
            if (p % 10 < 0 || p % 10 > 7 || p / 10 < 0 || p / 10 > 7) // if it is out of board, don't add
            {
                movablePos.Remove(p);
            }
            else if (Board.IsDangerZone(Board.GetTileFromPos(p), Team)) // if it is danger zone, don't add
            {
                movablePos.Remove(p);
            }
            else if (Board.GetPiece(p)) // if there is a enemy piece
            {
                if (Board.GetPiece(p).Team != Team)
                    movableTiles.AddKillable(p);
                else
                    movablePos.Remove(p);
            }
            else
            {
                movableTiles.AddMovable(p);
            }
        }

        // Log movablePos
        Debug.Log(movablePos.Aggregate("", (current, p) => current + (p + " ")));

        if (movablePos.Count == 0)
            Board.RaiseCheckMate();


        return movableTiles;
    }
}
