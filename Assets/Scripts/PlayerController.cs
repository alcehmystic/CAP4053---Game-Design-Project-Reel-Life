using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //mvmtSpeed can be edited in editor now
    [SerializeField]
    private float mvmtSpeed;

    private GameObject player;

    public int maxHealth = 4;
    public int currentHealth;

    public HealthBar healthBar;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }
       
    void Update()
    {
        handleMvmtInput();
    }

    void handleMvmtInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 mvmt = new Vector3(horizontal, 0, vertical);
        transform.Translate(mvmt * mvmtSpeed * Time.deltaTime, Space.World);
    }
    void takeDamage(int damage) {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth == 0) {
            Destroy(player);
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            takeDamage(1);
        }
    }
}
