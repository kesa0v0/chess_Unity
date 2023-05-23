public class Rook : Piece
{
    protected override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(HorizontalMovement());
        movableTiles.Add(VerticalMovement());

        return movableTiles;
    }
}
