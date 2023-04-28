public class Queen : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(HorizontalMovement());
        movableTiles.Add(VerticalMovement());
        movableTiles.Add(DiagonalMovement());

        return movableTiles;
    }
}
