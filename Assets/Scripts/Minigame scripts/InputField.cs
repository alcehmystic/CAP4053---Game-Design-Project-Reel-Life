using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public int col;
    public GameManager gm;

    void OnMouseDown() 
    {
        gm.SelectColumn(col);
    }
}
