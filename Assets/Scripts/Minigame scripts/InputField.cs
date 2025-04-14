using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public int col;
    public GameManager gm;

    void OnMouseDown() 
    {
        //if it is not player 1's turn, do not allow them to select a column
        if (!gm.isPlayer1Turn) 
        {
            return;
        }
        gm.SelectColumn(col);
    }
}
