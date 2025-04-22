using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomObjectSpawning : MonoBehaviour
{
    SceneTransitionManager sceneTransition;
    public Player player;
    [SerializeField]public GameObject[] ob;
    [SerializeField]public float damage = 10;
    [SerializeField] public int yPos = 2;
    [SerializeField] public int gapLo = 10;
    [SerializeField] public int gapHi = 15;

    [SerializeField] public int horizontalStartLo = 20;
    [SerializeField] public int horizontalStartHi = 25;
    [SerializeField] public int horizontalStartPos = 55;

    [SerializeField] public int verticalStartPos = 30;
    [SerializeField] public int verticalStartLo = 45;
    [SerializeField] public int verticalStartHi = 50;

    public Dialogue winDialogue;
    public Dialogue winDialogue2;
    public Dialogue winDialogue3;
    public Dialogue loseDialogue;

    Vector3 up = new Vector3(1, 0, 0);
    Vector3 down = new Vector3(-1,0,0);
    Vector3 left = new Vector3(0,0,-1);
    Vector3 right = new Vector3(0,0,1);

    int numTurns;
    int repeatRate;
    int speed;

    public bool playerAlive = true;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        int[] repeat_speed_turns = player.GetBoulderDifficulty();
        repeatRate = repeat_speed_turns[0];
        speed = repeat_speed_turns[1];
        numTurns = repeat_speed_turns[2];
        StartCoroutine(RepeatFunction());
    }

    private IEnumerator WaitForAllToBeDestroyed()
    {
        while (GameObject.FindGameObjectsWithTag("Projectile").Length > 0)
        {
            yield return new WaitForSeconds(0.2f); //wait before checking again
            Debug.Log("waiting");
        }
        Debug.Log("no more");
    }

    public void StopRepeating()
    {
        if (RepeatFunction() != null)
        {
            StopCoroutine(RepeatFunction());
            Debug.Log("RepeatFunction stopped!");
        }
        StartCoroutine(Lose(3f));
    }

    IEnumerator RepeatFunction()
    {
        int lastInt = 0;
        int intStreak = 0;
        while (numTurns > 0 && playerAlive)
        {
            int randInt = Random.Range(0, 8);
            if (randInt == lastInt && intStreak == 3 && randInt != 0)
            {
                randInt = 1;
                intStreak = 0;
            }
            else if (randInt == lastInt && intStreak == 3 && randInt == 0)
            {
                randInt = 1;
                intStreak = 0;
            }
            else if (randInt == lastInt && intStreak < 3)
            {
                intStreak++;
            }
            yield return new WaitForSeconds(repeatRate);
            //do random sequence of projectile spawning after waiting

            //option 1: spawn one a time a few times from alternating directions
            if (randInt == 0) 
            {
                int randDir = Random.Range(0, 2);
                if (randDir == 0)
                {
                    singleProjectile(up);
                    singleProjectile(up);
                    singleProjectile(up);
                    singleProjectile(down);
                    singleProjectile(down);
                    singleProjectile(down);
                    singleProjectile(left);
                    singleProjectile(left);
                    singleProjectile(left);
                    singleProjectile(right);
                    singleProjectile(right);
                    singleProjectile(right);
                }
            }
            //option 2: spawn horizontal wall
            else if (randInt == 1) 
            {
                int randDir = Random.Range(0, 2);
                if (randDir == 0)
                {
                    horizontalLinesPattern(up);
                }
                else
                {
                    horizontalLinesPattern(down);
                }
            }
            //option 3: spawn vertical wall
            else if (randInt == 2)
            {
                int randDir = Random.Range(0, 2);
                if (randDir == 0)
                {
                    verticalLinesPattern(left);
                }
                else
                {
                    verticalLinesPattern(right);
                }
            }
            //option 4: spawn horizontal and vertical wall 
            else if (randInt == 3 || randInt > 5)
            {
                int randDir = Random.Range(0, 4);
                if (randDir == 0)
                {
                    horizontalLinesPattern(up);
                    verticalLinesPattern(left);
                }
                else if(randDir == 1)
                {
                    horizontalLinesPattern(up);
                    verticalLinesPattern(right);
                }
                else if (randDir == 2)
                {
                    horizontalLinesPattern(down);
                    verticalLinesPattern(left);
                }
                else if (randDir == 3)
                {
                    horizontalLinesPattern(down);
                    verticalLinesPattern(right);
                }
            }
            //option 5: spawn two horizontal walls
            else if (randInt == 4) 
            {
                horizontalLinesPattern(down);
                horizontalLinesPattern(up);
            }
            //option 6: spawn two vertical walls
            else if (randInt == 5)
            {
                verticalLinesPattern(right);
                verticalLinesPattern(left);
            }
            numTurns--;
        }
        yield return new WaitForSeconds(6f);
        if(playerAlive)
        {
            StartCoroutine(Win(5f));
        }
        else
        {
            StartCoroutine(Lose(5f));
        }
    }

    IEnumerator Lose(float waitTime)
    {
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        //SoundManager.Instance.PlaySound("lose_sfx");
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(LoseDialogue());
    }

    private IEnumerator LoseDialogue()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        dm.isFishyDialogue = true;
        dm.StartDialogue(loseDialogue);
        yield return new WaitUntil(() => dm.dialogueActive == false);
        sceneTransition.SetPreviousScene();
        SceneManager.LoadScene("CaveBossArea");
    }

    IEnumerator Win(float waitTime)
    {
        player.AddBoulderWin();
        if (player.boulderGameWins == 3)
        {
            Debug.Log("giving player fish");
            InventoryManager.Instance.AddItem(16, 1);
        }
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        SoundManager.Instance.PlaySound("win_sfx");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("starting win dialogue");
        StartCoroutine(WinDialogue());
    }

    private IEnumerator WinDialogue()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        dm.isFishyDialogue = true;
        Debug.Log("starting win dialogue routine connect4 wins = " + player.connect4Wins);
        if (player.boulderGameWins == 1)
        {
            dm.StartDialogue(winDialogue);
        }
        else if (player.boulderGameWins == 2)
        {
            dm.StartDialogue(winDialogue2);
        }
        else if (player.boulderGameWins == 3)
        {
            dm.StartDialogue(winDialogue3);
        }
        yield return new WaitUntil(() => dm.dialogueActive == false);
        Debug.Log("loading cave boss area");
        sceneTransition.SetPreviousScene();
        SceneManager.LoadScene("CaveBossArea");
    }

    //does the actual spawning
    void spawnProjectiles(List<Vector3> spawnPositions, Vector3 dir)
    {
        foreach (Vector3 pos in spawnPositions)
        {
            int randOb = Random.Range(0, ob.Length);
            // Instantiate the projectile at the calculated position
            GameObject projectile = Instantiate(ob[randOb], pos, Quaternion.identity);

            // Set the projectile's velocity to move in the chosen direction
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = dir * speed;
            }
        }
    }

    void spawnOneProjectile(Vector3 spawnPosition, Vector3 dir)
    {
        int randOb = Random.Range(0, ob.Length);
        // Instantiate the projectile at the calculated position
        Quaternion rotation = Quaternion.LookRotation(dir);
        GameObject projectile = Instantiate(ob[randOb], spawnPosition, Quaternion.identity);

        // Set the projectile's velocity to move in the chosen direction
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * speed;
        }
    }

    void horizontalLinesPattern(Vector3 dir) {
        int xStart = horizontalStartPos;
        //which zpos the projectiles will start at 
        int zPos = Random.Range(horizontalStartLo, horizontalStartHi);
        //space between projectiles (in z direction)
        int gapSize = Random.Range(gapLo, gapHi);
        //determines if these move right to left or left to right
        int directionChoice = Random.Range(0,2);

        //Vector3 dir = new Vector3();
        //up
        //Vector3 direction1 = new Vector3(1, 0, 0);
        //down
        //Vector3 direction2 = new Vector3(-1, 0, 0);

        if (dir == up) {
            //dir = direction1;
            xStart = xStart * (-1);
        }

        List<Vector3> spawnPositions = new List<Vector3>();

        while (zPos > -25) {
            Vector3 pos = new Vector3(xStart, yPos, zPos);
            spawnPositions.Add(pos);
            zPos -= gapSize;
        }

        spawnProjectiles(spawnPositions, dir);

    }

    void verticalLinesPattern(Vector3 dir) {
        int zStart = verticalStartPos;
        //which zpos the projectiles will start at 
        int xPos = Random.Range(verticalStartLo, verticalStartHi);
        //space between projectiles (in z direction)
        int gapSize = Random.Range(gapLo, gapHi);
        //determines if these move right to left or left to right
        int directionChoice = Random.Range(0, 2);

        //Vector3 dir = new Vector3();
        //left to right
        // direction1 = new Vector3(0, 0, 1);
        //right to left
        //Vector3 direction2 = new Vector3(0, 0, -1);

        if (dir == right)
        {
            //dir = direction1;
            zStart = zStart * (-1);
        }

        List<Vector3> spawnPositions = new List<Vector3>();

        while (xPos > -50)
        {
            Vector3 pos = new Vector3(xPos, yPos, zStart);

            spawnPositions.Add(pos);
            xPos -= gapSize;
        }

        spawnProjectiles(spawnPositions, dir);
    }

    void singleProjectile(Vector3 dir) 
    {
        Vector3 pos = new Vector3(0, yPos, 30);
        if(dir == up)
        {
            int xStart = (-1) * horizontalStartPos;
            int zPos = Random.Range(-25,26);

            pos = new Vector3(xStart, yPos, zPos);
        }
        else if(dir == down)
        {
            int xStart = horizontalStartPos;
            int zPos = Random.Range(-25,26);

            pos = new Vector3(xStart, yPos, zPos);
        }
        else if(dir == left)
        {
            int zStart = verticalStartPos;
            int xPos = Random.Range(-50, 51);

            pos = new Vector3(xPos, yPos, zStart);

        }
        else if(dir == right)
        {
            int zStart = (-1) * verticalStartPos;
            int xPos = Random.Range(-50,51);

            pos = new Vector3(xPos, yPos, zStart);

        }
        //Decide where to put the projectile 
        spawnOneProjectile(pos, dir);
    }

}
