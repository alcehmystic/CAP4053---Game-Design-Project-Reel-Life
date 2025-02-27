using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawning : MonoBehaviour
{
    [SerializeField]public GameObject ob;
    [SerializeField]public float speed = 10;
    [SerializeField]public float damage = 10;
    [SerializeField] public int yPos = 2;
    [SerializeField] public int gapLo = 4;
    [SerializeField] public int gapHi = 9;

    [SerializeField] public int horizontalStartLo = 20;
    [SerializeField] public int horizontalStartHi = 25;
    [SerializeField] public int horizontalStartPos = 55;

    [SerializeField] public int verticalStartPos = 30;
    [SerializeField] public int verticalStartLo = 45;
    [SerializeField] public int verticalStartHi = 50;

    // Update is called once per frame
    void Update()
    {
        //if space is pressed, spawn a random object at a random position (y will always be 3 to avoid collision with the ground)
        if (Input.GetKeyDown(KeyCode.Space)) {
            
            int randFunc = Random.Range(0, 2);
            if (randFunc == 2) crossPattern();
            else if (randFunc == 1) horizontalLinesPattern();
            else if (randFunc == 0) verticalLinesPattern();
            

            //Vector3 randPosition = new Vector3(Random.Range(-30, 30), 3, Random.Range(-30,30));

            //GameObject newProjectile = Instantiate(ob, randPosition, Quaternion.identity);
             
            //Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
            //Vector3 randSpeed = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
            //rb.velocity = randSpeed;
            //rb.useGravity = false;
            //rb.velocity = direction.normalized * speed;
        }
    }

    //does the actual spawning
    void spawnProjectiles(List<Vector3> spawnPositions, Vector3 dir)
    {
        foreach (Vector3 pos in spawnPositions)
        {
            // Instantiate the projectile at the calculated position
            GameObject projectile = Instantiate(ob, pos, Quaternion.identity);

            // Set the projectile's velocity to move in the chosen direction
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = dir * speed; // Adjust 10f to your desired speed
            }
        }
    }

    void crossPattern() { 
    }

    void horizontalLinesPattern() {
        int xStart = horizontalStartPos;
        //which zpos the projectiles will start at 
        int zPos = Random.Range(horizontalStartLo, horizontalStartHi);
        //space between projectiles (in z direction)
        int gapSize = Random.Range(gapLo, gapHi);
        //determines if these move right to left or left to right
        int directionChoice = Random.Range(0,2);

        Vector3 dir = new Vector3();
        //left to right
        Vector3 direction1 = new Vector3(1, 0, 0);
        //right to left
        Vector3 direction2 = new Vector3(-1, 0, 0);

        if (directionChoice == 0) {
            dir = direction1;
            xStart = xStart * (-1);
        }
        else
        {
            dir = direction2;
        }

        List<Vector3> spawnPositions = new List<Vector3>();

        while (zPos > -25) {
            Vector3 pos = new Vector3(xStart, yPos, zPos);
            spawnPositions.Add(pos);
            zPos -= gapSize;
        }

        spawnProjectiles(spawnPositions, dir);

    }

    void verticalLinesPattern() {
        int zStart = verticalStartPos;
        //which zpos the projectiles will start at 
        int xPos = Random.Range(verticalStartLo, verticalStartHi);
        //space between projectiles (in z direction)
        int gapSize = Random.Range(gapLo, gapHi);
        //determines if these move right to left or left to right
        int directionChoice = Random.Range(0, 2);

        Vector3 dir = new Vector3();
        //left to right
        Vector3 direction1 = new Vector3(0, 0, 1);
        //right to left
        Vector3 direction2 = new Vector3(0, 0, -1);

        if (directionChoice == 0)
        {
            dir = direction1;
            zStart = zStart * (-1);
        }
        else
        {
            dir = direction2;
        }

        List<Vector3> spawnPositions = new List<Vector3>();

        while (xPos > -50)
        {
            print(xPos);
            Vector3 pos = new Vector3(xPos, yPos, zStart);

            spawnPositions.Add(pos);
            xPos -= gapSize;
        }

        spawnProjectiles(spawnPositions, dir);
    }

}
