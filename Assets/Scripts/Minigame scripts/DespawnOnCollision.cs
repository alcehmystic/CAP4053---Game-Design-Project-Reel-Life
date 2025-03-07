using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnCollision : MonoBehaviour
{
    public HealthBar healthBar;

    private void Start()
    {
        healthBar = FindObjectOfType<HealthBar>();
    }
    void OnTriggerEnter(Collider collision){
       
        Debug.Log("Collided with: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);

        if (collision.gameObject.tag == "Player") {
            Destroy(gameObject);
            healthBar.takeDamage(1);
        }
        else if (collision.gameObject.tag == "Destroy") {
            Destroy(gameObject);
        }
    }
}
