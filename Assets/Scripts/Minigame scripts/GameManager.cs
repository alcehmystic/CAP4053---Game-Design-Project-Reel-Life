using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    SceneTransitionManager sceneTransition;
    public Player player;
    public GameObject player1;
    public GameObject player2;
    public bool player1Win;
    public bool player2Win;
    public GameObject X;
    public GameObject[] spawners;
    public bool isPlayer1Turn;
    public bool isActionWaiting = false;
    private bool isWinCoroutineRunning = false;
    private bool isLoseCoroutineRunning = false;
    public int[,] board;
    public int[,] AIboard;
    public int boardHeight = 6;
    public int boardLength = 7;
    public int difficulty;

    void Start()
    {
        //start on player 2 turn
        player = FindObjectOfType<Player>();
        difficulty = player.GetConnect4Difficulty();
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeIn();
        }
        isPlayer1Turn = false;
        board = new int[boardHeight, boardLength];
        AIboard = new int[boardHeight, boardLength];
        player1Win = false;
        player2Win = false;
    }

    private void Update()
    {
        if (player1Win)
        {
            Debug.Log("you win");
            player.AddConnect4Win();
            if (!isWinCoroutineRunning)
            {
                StartCoroutine(Win(5f));
            }
            return;
        }
        else if (player2Win)
        {
            Debug.Log("you lose");
            if (!isLoseCoroutineRunning)
            {
                StartCoroutine(Lose(5f));
            }
            return;
        }
        if (!isPlayer1Turn && !isActionWaiting)
        {
            StartCoroutine(WaitBeforeTakingTurn(1f));
        }
    }
    private IEnumerator WaitBeforeTakingTurn(float waitTime)
    {
        isActionWaiting = true;
        yield return new WaitForSeconds(waitTime); 
        TakeTurn(0); 
        isActionWaiting = false; 
    }

    private IEnumerator Win(float waitTime)
    {
        isWinCoroutineRunning = true;
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        SoundManager.Instance.PlaySound("win_sfx");
        yield return new WaitForSeconds(waitTime);
        //put the player back in the snow scene
        sceneTransition.SetPreviousScene();
        SceneManager.LoadScene("SnowBossArea");
    }

    private IEnumerator Lose(float waitTime)
    {
        isLoseCoroutineRunning = true;
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        //SoundManager.Instance.PlaySound("lose_sfx");
        yield return new WaitForSeconds(waitTime);
        //put the player back in the snow scene
        sceneTransition.SetPreviousScene();
        SceneManager.LoadScene("SnowBossArea");
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

    public void UpdateBoard(int col, int updateVal) 
    {
        for (int i = 5; i >= 0; i--)
        {
            if (board[i, col] == 0)
            {
                board[i, col] = updateVal;
                AIboard[i, col] = updateVal;
                return;
            }
        }
    }

    public int getAvailableRow(int col)
    {
        for (int i = 5; i >= 0; i--)
        {
            if (board[i, col] == 0)
            {
                return i;
            }
        }
        return -1;
    }
    public bool DidWin(int[,] board, int playerNumber)
    {
        //Horizontal check
        for (int r = 0; r < boardHeight; r++) 
        {
            //only need to start at columns 0 through 3
            for (int c = 0; c <=3; c++) 
            {
                int count = 0;
                for (int i = 0; i <= 3; i++) {
                    if (board[r, c + i] == playerNumber)
                    {
                        count++;
                    }
                    //if we miss one then just continue to the next c
                    else
                    {
                        break;
                    }
                }
                if (count == 4) return true;
            }
        }

        //Vertical check
        for (int c = 0; c < boardLength; c++)
        {
            //only need to start at rows 0 through 2
            for (int r = 0; r <= 2; r++)
            {
                int count = 0;
                for (int i = 0; i <= 3; i++)
                {
                    if (board[r+i,c] == playerNumber)
                    {
                        count++;
                    }
                    //if we miss one then just continue to the next c
                    else
                    {
                        break;
                    }
                }
                if (count == 4) return true;
            }
        }

        //Diag check \
        for (int r = 0; r <= 2; r++)
        {
            for (int c = 0; c <= 3; c++)
            {
                int count = 0;
                for (int i = 0; i <= 3; i++)
                {
                    if (board[r + i, c + i] == playerNumber)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count == 4) return true;
            }
        }

        //Diag check /
        for (int r = 3; r < boardHeight; r++)
        {
            for (int c = 0; c <= 3; c++)
            {
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (board[r - i, c + i] == playerNumber)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count == 4) return true;
            }
        }

        return false;
    }

    public void AITurn(int difficulty) 
    {
        int randomInt = Random.Range(0, 10);

        List<int> blockCols = new List<int>();
        List<int> winCols = new List<int>();
        List<int> emptyCols = new List<int>();

        for (int col = 0; col < boardLength; col++)
        {
            if (!colIsFull(col))
            {
                int row = getAvailableRow(col);

                //if player 1 can win, AI will place to block
                AIboard[row, col] = 1;
                if (DidWin(AIboard, 1))
                {
                    blockCols.Add(col);
                    //undo turn
                    AIboard[row, col] = 0;
                    continue;
                }
                //undo turn
                AIboard[row, col] = 0;
                

                AIboard[row, col] = 2;
                //if AI will win, place piece there
                if (DidWin(AIboard, 2))
                {
                    winCols.Add(col);
                    //undo turn
                    AIboard[row, col] = 0;
                    continue;
                }
                //undo turn
                AIboard[row, col] = 0;

                emptyCols.Add(col);

            }
        }

        Debug.Log("size of block cols: " + blockCols.Count);
        Debug.Log("size of win cols: " + winCols.Count);
        Debug.Log("size of empty cols: " + emptyCols.Count);

        if (blockCols.Count == 0 && winCols.Count == 0)
        { 
            int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
            Debug.Log("selected column " + randomCol);
            Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
            UpdateBoard(randomCol, 2);
            return;
        }

        if (difficulty == 3)
        {
            if (winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else if (blockCols.Count != 0)
            {
                int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
        }
        else if (difficulty == 2)
        {
            int chance = Random.Range(0, 2);
            if (chance == 0 && winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else if (chance == 1 && blockCols.Count != 0)
            {
                int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else 
            {
                Debug.Log("!!!!!!!!empty cols is: " + emptyCols.Count);
                int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
        }
        else 
        {
            int chance = Random.Range(0, 3);
            if (chance == 0 && winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else if(chance == 1 && blockCols.Count != 0)
            {
                int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else
            {
                int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                Debug.Log("selected column " + randomCol);
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
        }


        return;


    }
    void TakeTurn(int col)
    {
        if (isPlayer1Turn)
        {
            Instantiate(player1, spawners[col].transform.position, Quaternion.identity);
            UpdateBoard(col, 1);
            isPlayer1Turn = false;
            if (DidWin(board,1)) player1Win = true;
        }
        else 
        {
            AITurn(difficulty);
            isPlayer1Turn = true;
            if (DidWin(board,2)) player2Win = true;
        }

        
    }
}
