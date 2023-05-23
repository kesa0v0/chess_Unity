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
            if (Board.GetKindOfTile(this, p) is TileKind.Movable) // if there is empty tile
            {
                movableTiles.AddMovable(p);
            }
            else if (Board.GetKindOfTile(this, p) is TileKind.Killable or TileKind.EnPassant) // if there is a enemy piece
            {
                movableTiles.AddKillable(p);
            }
        }

        return movableTiles;
    }
}
