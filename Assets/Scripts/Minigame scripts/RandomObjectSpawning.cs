using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawning : MonoBehaviour
{
    [SerializeField]public GameObject[] ob;
    [SerializeField]public float speed = 10;
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



    Vector3 up = new Vector3(1, 0, 0);
    Vector3 down = new Vector3(-1,0,0);
    Vector3 left = new Vector3(0,0,-1);
    Vector3 right = new Vector3(0,0,1);

    public float repeatRate = 5f;
    public int difficulty = 1;
    public int numTurns = 10;

    // Update is called once per frame
    void Update()
    {
        //if space is pressed, spawn a random object at a random position (y will always be 3 to avoid collision with the ground)
        //if (Input.GetKeyDown(KeyCode.Space)) {
            
            //int randFunc = Random.Range(0, 2);
            //if (randFunc == 2) crossPattern();
            //else if (randFunc == 1) horizontalLinesPattern();
            //else if (randFunc == 0) verticalLinesPattern();
            

            //Vector3 randPosition = new Vector3(Random.Range(-30, 30), 3, Random.Range(-30,30));

            //GameObject newProjectile = Instantiate(ob, randPosition, Quaternion.identity);
             
            //Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
            //Vector3 randSpeed = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
            //rb.velocity = randSpeed;
            //rb.useGravity = false;
            //rb.velocity = direction.normalized * speed;
        //}
    }

    private void Start()
    {
        StartCoroutine(RepeatFunction());
    }

    IEnumerator RepeatFunction()
    {
        int lastInt = 0;
        int intStreak = 0;
        while (numTurns > 0)
        {
            int randInt = Random.Range(0, 6);
            if (randInt == lastInt && intStreak == 3 && randInt != 0)
            {
                randInt = 0;
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
                    singleProjectile(down);
                    singleProjectile(up);
                    singleProjectile(down);
                }
                else 
                {
                    singleProjectile(left);
                    singleProjectile(right);
                    singleProjectile(left);
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
            else if (randInt == 3)
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
