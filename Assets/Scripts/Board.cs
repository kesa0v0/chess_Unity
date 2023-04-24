using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Team currentTurn;
    
    [SerializeField] private Tile tile;
    private List<List<Tile>> _tiles = new();

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
                temp.transform.transform.position = new Vector2(j, i);
                _tiles[i].Add(temp.GetComponent<Tile>());
                if ((j % 2 + i) % 2 == 1)
                    temp.GetComponent<SpriteRenderer>().color = Color.gray;
                else
                    temp.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
            }
        }
        
        // Set Turns who moves first
        currentTurn = Team.White;
    }
    
    public void TurnOver()
    {
        if (currentTurn == Team.White)
        {
            currentTurn = Team.Black;
        }
        else if (currentTurn == Team.Black)
        {
            currentTurn = Team.White;
        }
        else
        {
            Debug.LogError("Turn is Unknown State.");
        }
    }
}
