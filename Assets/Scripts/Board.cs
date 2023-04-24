using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tile;
    private readonly List<List<Tile>> _tiles = new();

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize Chess Tiles ( 8 x 8 )
        for (var i = 0; i < 8; i++) // vertical
        {
            _tiles.Add(new List<Tile>());
            
            for (var j = 0; j < 8; j++) // horizontal
            {
                var temp = Instantiate(tile, this.transform);
                temp.transform.transform.position = new Vector3(j, i, 5);
                _tiles[i].Add(temp.GetComponent<Tile>());
                temp.GetComponent<SpriteRenderer>().color = (j % 2 + i) % 2 == 1 ? Color.gray : new Color(0.8f, 0.8f, 0.8f);
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
}
