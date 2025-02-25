using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public GameObject[] spawnPositions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectColumn(int col) {
        takeTurn(col);
    }

    void takeTurn(int col) {
        Instantiate(player1, spawnPositions[col].transform.position, Quaternion.Euler(90,0,0));
    }
}
