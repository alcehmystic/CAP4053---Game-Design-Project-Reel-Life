using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider collision){

        if (collision.gameObject.tag == "Player") {
            
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Destroy") {

            Destroy(gameObject);
        }
    }
}
