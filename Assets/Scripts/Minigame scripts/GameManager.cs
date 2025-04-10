using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject X;
    public GameObject[] spawners;
    public bool isPlayer1Turn;
    public int[,] board;
    public int boardHeight = 6;
    public int boardLength = 7;

    void Start()
    {
        //start on player 2 turn
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeIn();
        }
        isPlayer1Turn = false;
        board = new int[boardHeight, boardLength];
    }

    public bool colIsFull(int col) {
        if (board[0, col] != 0) 
        { 
            return true;
        }
        return false;
    }

    public void SelectColumn(int col) 
    {
        if (colIsFull(col))
        {
            Debug.Log("column is full!");
            SoundManager.Instance.PlaySound("error_sfx");
            GameObject x_ob = Instantiate(X, spawners[col].transform.position, Quaternion.identity);
            Destroy(x_ob, 1f);
        }
        else 
        {
            Debug.Log("column has room");
            TakeTurn(col);
        }
    }

    public void UpdateBoard(int col) 
    {
        int updateVal = 0;
        if (isPlayer1Turn)
        {
            updateVal = 1;
        }
        else 
        {
            updateVal = 2;
        }

        for (int i = 5; i >= 0; i--)
        {
            if (board[i, col] == 0)
            {
                board[i, col] = updateVal;
                Debug.Log("piece placed in row " + i + " col " + col + " with value " + updateVal);
                return;
            }
        }
    }

    public bool DidWin(int playerNumber)
    {
        return false;
    }

    void TakeTurn(int col)
    {
        if (isPlayer1Turn)
        {
            Instantiate(player1, spawners[col].transform.position, Quaternion.identity);
            UpdateBoard(col);
            isPlayer1Turn = false;
        }
        else 
        {
            Instantiate(player2, spawners[col].transform.position, Quaternion.identity);
            UpdateBoard(col);
            isPlayer1Turn = true;
        }

        
    }
}
