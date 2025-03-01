using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    private Ray playerRay;
    // Start is called before the first frame update
    void Start()
    {
        playerRay = new Ray(transform.position, transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        playerRay = new Ray(transform.position, transform.forward);
        Debug.DrawRay(playerRay.origin, playerRay.direction * 3f, Color.red); // Debug ray
    }

    public Ray GetPlayerRay()
    {
        return playerRay;
    }
}
