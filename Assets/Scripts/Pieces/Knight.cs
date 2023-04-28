using System.Collections.Generic;
using System.Linq;

public class Knight : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        var pos = Board.GetPosFromVec2(transform.position);
        var knightMove = new List<int>
        {
            pos + 21, pos + 19, pos + 12, pos + 8, pos - 21, pos - 19, pos - 12, pos - 8
        };
        
        foreach (var move in knightMove.Where(move => !Board.GetPiece(move) || Board.GetPiece(move).Team != Team))
        {
            if (Board.GetPiece(move) && Board.GetPiece(move).Team != Team)
            {
                movableTiles.AddKillable(move);
                continue;
            }
            movableTiles.AddMovable(move);
        }

        return movableTiles;
    }
}
