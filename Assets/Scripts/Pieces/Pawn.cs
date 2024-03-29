using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Pawn : Piece
{
    public bool isEnPassantTarget;
    
    protected override MovableTiles GetMovableTilesCode()
    {
        // TileCode: YX (Y: Column, X: Row)
        
        var movableTiles = new MovableTiles();

        var pos = Board.GetPosFromVec2(transform.position);
        
        if (isEnPassantTarget && Board.EnPassantPawns.ContainsKey(pos + (Team == Team.White ? -10 : 10)))
        {
            isEnPassantTarget = false;
            Board.EnPassantPawns.Remove(pos + (Team == Team.White ? -10 : 10));
        }

        switch (Team)
        {
            case Team.White:
            {
                if (Board.GetKindOfTile(this, pos + 11) is TileKind.Killable or TileKind.EnPassant)
                    movableTiles.AddKillable(pos + 11);
                if (Board.GetKindOfTile(this, pos + 9) is TileKind.Killable or TileKind.EnPassant)
                    movableTiles.AddKillable(pos + 9);
                
                if (Board.GetKindOfTile(this, pos + 10) is TileKind.Movable)
                    movableTiles.AddMovable(pos + 10);
                if (isFirstMove && !Board.GetPiece(pos + 10) && !Board.GetPiece(pos + 20))
                {
                    movableTiles.EnPassantMoveTile = pos + 20;
                    movableTiles.EnPassantTile = pos + 10;
                }
                break;
            }
            case Team.Black:
            {
                if (Board.GetKindOfTile(this, pos - 11) is TileKind.Killable or TileKind.EnPassant)
                    movableTiles.AddKillable(pos - 11);
                if (Board.GetKindOfTile(this, pos - 9) is TileKind.Killable or TileKind.EnPassant)
                    movableTiles.AddKillable(pos - 9);
                
                if (Board.GetKindOfTile(this, pos - 10) is TileKind.Movable)
                    movableTiles.AddMovable(pos - 10);
                if (isFirstMove && !Board.GetPiece(pos - 10) && !Board.GetPiece(pos - 20))
                {
                    movableTiles.EnPassantMoveTile = pos - 20;
                    movableTiles.EnPassantTile = pos - 10;
                }
                break;
            }
            case Team.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return movableTiles;
    }

    public void CheckPromotion()
    {
        if (Team == Team.White && Board.GetPosFromVec2(transform.position) / 10 == 7 ||
            Team == Team.Black && Board.GetPosFromVec2(transform.position) / 10 == 0)
        {
            Board.Promotion(this);
        }
    }

    protected override void OnDragEnd()
    {
        base.OnDragEnd();
        var currentPos = Board.GetPosFromCursor();
        
        // Check is Pawn's Double Move tile
        if (movabletiles.EnPassantMoveTile != currentPos) return;
        Board.TempEnPassantPawns.Add(movabletiles.EnPassantTile, this);
        Board.MovePiece(this, Board.GetTileFromPos(currentPos));
        isEnPassantTarget = true;
        Board.TurnOver();
    }
}
