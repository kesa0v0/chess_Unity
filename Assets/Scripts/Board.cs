using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    
    [SerializeField] private Tile tile;
    private List<List<Tile>> tiles = new List<List<Tile>>();

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var temp = Instantiate(tile, this.transform);
            temp.transform.transform.position = new Vector2(i, j);
            if ((j % 2 + i) % 2 == 1)
                temp.GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
