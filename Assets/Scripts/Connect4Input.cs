using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4Input : MonoBehaviour
{
    public int column;
    public GameManager gameManager;

    //on mouse click, execute this code
    private void OnMouseDown()
    {
        gameManager.selectColumn(column);
    }
}
