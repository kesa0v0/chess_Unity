public class Knight : Piece
{
    public override MovableTiles GetMovableTilesCode()
    {
        var movableTiles = new MovableTiles();
        
        movableTiles.Add(HorizontalMovement());

        return movableTiles;
    }
}
