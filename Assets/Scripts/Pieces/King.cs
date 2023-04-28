using System.Collections.Generic;
using NUnit.Framework;

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
        var checkList = new List<int>
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

        foreach (var p in movablePos)
        {
            if (Board.IsDangerZone(Board.GetTileFromPos(p), Team)) // if it is danger zone, don't add
            {
                checkList.Remove(p);
            }
            else if (Board.GetPiece(p)) // if there is a enemy piece
            {
                if (Board.GetPiece(p).Team != Team)
                    movableTiles.AddKillable(p);
                else
                    checkList.Remove(p);
            }
            else
            {
                movableTiles.AddMovable(p);
            }
        }

        if (checkList.Count == 0)
            Board.RaiseCheckMate();


        return movableTiles;
    }
}
