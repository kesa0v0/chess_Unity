public class Rook : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(HorizontalMovement());
        movableTiles.Add(VerticalMovement());

        return movableTiles;
    }
}
