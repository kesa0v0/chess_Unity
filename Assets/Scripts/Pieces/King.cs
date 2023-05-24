using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    protected override MovableTiles GetMovableTilesCode()
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
                movablePos.Remove(p);
            else if (Board.IsDangerZone(Board.GetTileFromPos(p), Team)) // if it is danger zone, don't add
                movablePos.Remove(p);
            else if (Board.GetKindOfTile(this, p) is TileKind.Killable or TileKind.EnPassant) // if there is a enemy piece
                movableTiles.AddKillable(p);
            else if (Board.GetKindOfTile(this, p) is TileKind.Obstacle)
                movablePos.Remove(p);
            else
                movableTiles.AddMovable(p);
        }
        
        // Castling Check
        var unmovedRook = Board.GetPieces<Rook>(Team).FindAll(piece => piece.isFirstMove);
        if (isFirstMove && unmovedRook.Count > 0)
        {
            foreach (var piece in unmovedRook)
            {
                // Check if there is no piece between king and rook
                var isNoPieceBetween = true;
                var rookPos = Board.GetPosFromVec2(piece.transform.position);
                var diff = rookPos - pos;
                var diffAbs = diff > 0 ? diff : -diff;
                var diffSign = diff > 0 ? 1 : -1;
                
                for (var i = 1; i < diffAbs; i++)
                {
                    var tile = Board.GetTileFromPos(pos + i * diffSign);
                    if (tile.pieceOnTile != null)
                    {
                        isNoPieceBetween = false;
                        break;
                    }
                }
                
                // Check if there is no danger zone between king and rook
                var isNoDangerZoneBetween = true;
                for (var i = 0; i < 3; i++)
                {
                    var tile = Board.GetTileFromPos(pos + i * diffSign);
                    if (Board.IsDangerZone(tile, Team))
                    {
                        isNoDangerZoneBetween = false;
                        break;
                    }
                }

                if (isNoPieceBetween && isNoDangerZoneBetween)
                {
                    if (diffSign < 0)
                        movableTiles.QueenSideCastling = Team == Team.White ? 02 : 72;
                    else
                        movableTiles.KingSideCastling = Team == Team.White ? 06 : 76;
                }
            }
        }

        return movableTiles;
    }
}
