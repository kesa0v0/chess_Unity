public class Bishop : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(DiagonalMovement());

        return movableTiles;
    }
}
