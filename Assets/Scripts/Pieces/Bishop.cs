public class Bishop : Piece
{
    protected override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(DiagonalMovement());

        return movableTiles;
    }
}
