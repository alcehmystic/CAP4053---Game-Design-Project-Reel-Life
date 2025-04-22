using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    // public UnityEngine.UI.Slider slider;
    [SerializeField] public GameObject player;
    public RandomObjectSpawning spawner;

    public GameObject health25;
    public GameObject health50;
    public GameObject health75;
    public GameObject health100;
    public int health;

    private void Awake()
    {
        // if (slider == null)
        // {
        //     // slider = GetComponentInChildren<UnityEngine.UI.Slider>(); // Automatically finds the slider
        // }
    }

    private void Start()
    {
        health = 4;
        SetHealth(health);
        spawner = FindObjectOfType<RandomObjectSpawning>();
    }

    public void takeDamage(int damage) {
        health -= damage;
        SoundManager.Instance.PlaySound("Hit_sfx");
        SetHealth(health);
    }

    public void SetHealth(int val)
    {
        // if (val >= 0) {
        //     slider.value = val;

        //     // if (slider.value == 0) {
        //         spawner.playerAlive = false;
        //         // Debug.Log("Player has died");
        //         spawner.StopRepeating();
        //     // }
        // }

        switch (val) 
        {
            case 0:
                health100.SetActive(false);
                health75.SetActive(false);
                health50.SetActive(false);
                health25.SetActive(false);

                spawner.playerAlive = false;
                spawner.StopRepeating();
                break;
            case 1:
                health100.SetActive(false);
                health75.SetActive(false);
                health50.SetActive(false);
                health25.SetActive(true);
                break;
            case 2:
                health100.SetActive(false);
                health75.SetActive(false);
                health50.SetActive(true);
                health25.SetActive(true);
                break;
            case 3:
                health100.SetActive(false);
                health75.SetActive(true);
                health50.SetActive(true);
                health25.SetActive(true);
                break;
            case 4:
                health100.SetActive(true);
                health75.SetActive(true);
                health50.SetActive(true);
                health25.SetActive(true);
                break;
        }
    }
}
