using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tile;
    private readonly Dictionary<int, Tile> _tiles = new();

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize Chess Tiles ( 8 x 8 )
        for (var i = 0; i < 8; i++) // vertical
        {
            for (var j = 0; j < 8; j++) // horizontal
            {
                var temp = Instantiate(tile, this.transform);
                temp.transform.transform.position = new Vector3(j, i, 5);
                _tiles.Add(i * 10 + j, temp.GetComponent<Tile>()); // 00 ~ 77, ABCDEFGH = 12345678
                
                temp.GetComponent<Tile>() // set tile color
                    .TileColor = (j % 2 + i) % 2 == 1
                    ? Color.gray
                    : new Color(0.8f,
                        0.8f,
                        0.8f);
            }
        }
        
        // Set Turns who moves first
        currentTurn = Team.White;
    }
    
    public void TurnOver()
    {
        switch (currentTurn)
        {
            case Team.White:
                currentTurn = Team.Black;
                break;
            case Team.Black:
                currentTurn = Team.White;
                break;
            case Team.Unknown:
            default:
                Debug.LogError("Turn is Unknown State.");
                break;
        }
    }
    
    public void CheckAvailableTiles()
    {
        foreach (var tilePair in _tiles)
        {
            if (tilePair.Value.IsPlaceAvailable())
                tilePair.Value.SetAvailable();
        }
    }
    
    public void ResetCheckTiles()
    {
        foreach (var tilePair in _tiles)
        {
            tilePair.Value.SetUnavailable();
        }
    }
}
