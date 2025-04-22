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
    public bool isWinCoroutineRunning = false;
    public bool isLoseCoroutineRunning = false;
    public bool isDrawCoroutineRunning = false;
    public bool draw = false;
    public int[,] board;
    public int[,] AIboard;
    public int boardHeight = 6;
    public int boardLength = 7;
    public int difficulty;

    public Dialogue winDialogue;
    public Dialogue winDialogue2;
    public Dialogue winDialogue3;
    public Dialogue loseDialogue;
    public Dialogue drawDialogue;

    void Start()
    {
        //start on player 2 turn
        player = FindObjectOfType<Player>();
        Debug.Log("connect4 wins " + player.connect4Wins);
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
            if (!isWinCoroutineRunning)
            {
                StartCoroutine(Win(3f));
            }
            return;
        }
        else if (player2Win)
        {
            Debug.Log("you lose");
            if (!isLoseCoroutineRunning)
            {
                StartCoroutine(Lose(3f));
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
        player.AddConnect4Win();
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        SoundManager.Instance.PlaySound("win_sfx");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("about to start win");
        StartCoroutine(WinDialogue());
    }

    private IEnumerator WinDialogue()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        Debug.Log("starting win dialogue routine connect4 wins = " + player.connect4Wins);
        if (player.connect4Wins == 1)
        {
            dm.StartDialogue(winDialogue);
        }
        else if (player.connect4Wins == 2)
        {
            dm.StartDialogue(winDialogue2);
        }
        else if(player.connect4Wins == 3)
        {
            dm.StartDialogue(winDialogue3);
        }
        yield return new WaitUntil(() => dm.dialogueActive == false);
        //display dialogue
        //wait for player to click to exit scene
        sceneTransition.SetPreviousScene();
        SceneManager.LoadScene("SnowBossArea");
    }

    private IEnumerator LoseDialogue()
    {
        DialogueManager.Instance.StartDialogue(loseDialogue);
        yield return new WaitUntil(() => DialogueManager.Instance.dialogueActive == false);
        //display dialogue
        //wait for player to click to exit scene
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
        StartCoroutine(LoseDialogue());
    }

    private IEnumerator Draw(float waitTime)
    {
        isDrawCoroutineRunning = true;
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(DrawDialogue());
    }

    private IEnumerator DrawDialogue()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        dm.StartDialogue(drawDialogue);
        yield return new WaitUntil(() => dm.dialogueActive == false);
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

    public bool ThreeInARow(int[,] board, int playerNumber)
    {
        //Horizontal check
        for (int r = 0; r < boardHeight; r++)
        {
            //only need to start at columns 0 through 2
            for (int c = 0; c <= 2; c++)
            {
                int count = 0;
                for (int i = 0; i <= 2; i++)
                {
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
                if (count == 3) return true;
            }
        }

        //Vertical check
        for (int c = 0; c < boardLength; c++)
        {
            //only need to start at rows 0 through 3
            for (int r = 0; r <= 3; r++)
            {
                int count = 0;
                for (int i = 0; i <= 2; i++)
                {
                    if (board[r + i, c] == playerNumber)
                    {
                        count++;
                    }
                    //if we miss one then just continue to the next c
                    else
                    {
                        break;
                    }
                }
                if (count == 3) return true;
            }
        }

        //Diag check \
        for (int r = 0; r <= 2; r++)
        {
            for (int c = 0; c <= 3; c++)
            {
                int count = 0;
                for (int i = 0; i < 3; i++)
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
                if (count == 3) return true;
            }
        }

        //Diag check /
        for (int r = 3; r < boardHeight; r++)
        {
            for (int c = 0; c <= 3; c++)
            {
                int count = 0;
                for (int i = 0; i < 3; i++)
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
                if (count == 3) return true;
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
        

        if (difficulty == 3)
        {
            if (blockCols.Count == 0 && winCols.Count == 0)
            {
                List<int> threeRowCols = new List<int>();
                //find ways to get three in a row
                for (int col = 0; col < boardLength; col++)
                {
                    if (!colIsFull(col))
                    {
                        int row = getAvailableRow(col);

                        //if player 1 can win, AI will place to block
                        AIboard[row, col] = 1;
                        if (ThreeInARow(AIboard, 1))
                        {
                            threeRowCols.Add(col);
                        }
                        //undo turn
                        AIboard[row, col] = 0;
                    }
                }
                if (threeRowCols.Count != 0)
                {
                    int randomCol = threeRowCols[Random.Range(0, threeRowCols.Count)];
                    Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                    UpdateBoard(randomCol, 2);
                    return;
                }
                else
                {
                    int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                    Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                    UpdateBoard(randomCol, 2);
                    return;
                }
            }
            //if you can win, always win
            if (winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            //if you can block and you couldnt win, always block
            else if (blockCols.Count != 0)
            {
                int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
        }
        else if (difficulty == 2)
        {
            int chance = Random.Range(0, 10);
            //if you cant win or block, go randomly
            if (blockCols.Count == 0 && winCols.Count == 0)
            {
                int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            //if you can win, always win
            else if (winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            //if we can block and the chance is over 4, block. Otherwise go randomly
            else if (blockCols.Count != 0)
            {
                if(chance >= 4)
                {
                    int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                    Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                    UpdateBoard(randomCol, 2);
                    return;
                }
                else
                {
                    int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                    Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                    UpdateBoard(randomCol, 2);
                    return;
                }
             
            }
        }
        else 
        {
            if (blockCols.Count == 0 && winCols.Count == 0)
            {
                int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            int chance = Random.Range(0, 3);
            if (chance == 0 && winCols.Count != 0)
            {
                int randomCol = winCols[Random.Range(0, winCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else if(chance == 1 && blockCols.Count != 0)
            {
                int randomCol = blockCols[Random.Range(0, blockCols.Count)];
                Instantiate(player2, spawners[randomCol].transform.position, Quaternion.identity);
                UpdateBoard(randomCol, 2);
                return;
            }
            else
            {
                int randomCol = emptyCols[Random.Range(0, emptyCols.Count)];
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

        int fullCount = 0;
        for (int c = 0; c < boardLength; c++)
        {
            if (colIsFull(c)) fullCount++;
            else break;
        }
        if(fullCount == 7)
        {
            StartCoroutine(Draw(3f));
        }

    }
}
