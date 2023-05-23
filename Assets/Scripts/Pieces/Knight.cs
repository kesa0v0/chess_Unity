using System.Collections.Generic;
using System.Linq;

public class Knight : Piece
{
    protected override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        var pos = Board.GetPosFromVec2(transform.position);
        var knightMove = new List<int>
        {
            pos + 21, pos + 19, pos + 12, pos + 8, pos - 21, pos - 19, pos - 12, pos - 8
        };

        foreach (var p in knightMove.Where(p => p % 10 >= 0 && p % 10 <= 7 && p / 10 >= 0 && p / 10 <= 7))
        {
            if (!Board.GetPiece(p)) // if there is empty tile
            {
                movableTiles.AddMovable(p);
            }
            else if (Board.GetPiece(p) && Board.GetPiece(p).Team != Team) // if there is a enemy piece
            {
                movableTiles.AddKillable(p);
            }
        }

        return movableTiles;
    }
}
