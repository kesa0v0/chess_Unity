public class King : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        var pos = Board.GetPosFromVec2(transform.position);
        
        var movablePos = new[] {
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
            
        }
        
        // and kill enemy piece
        movableTiles.AddKillable(pos + 10);
        

        return movableTiles;
    }
}
