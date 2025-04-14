using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    [SerializeField] public GameObject player;
    public RandomObjectSpawning spawner;
    public int health;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<UnityEngine.UI.Slider>(); // Automatically finds the slider
        }
    }

    private void Start()
    {
        health = 4;
        SetHealth(health);
        spawner = FindObjectOfType<RandomObjectSpawning>();
    }

    public void takeDamage(int damage) {
        health -= damage;
        SetHealth(health);
    }

    public void SetHealth(int val)
    {
        if (val >= 0) {
            slider.value = val;

            if (slider.value == 0) {
                spawner.playerAlive = false;
                Debug.Log("Player has died");
                Destroy(player);
            }
        }
    }
}
