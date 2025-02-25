using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    int maxVal = 4;
    public void setHealth(int health) {
        if (health < 0)
        {
            slider.value = 0;
        }
        if (health > 4)
        {
            slider.value = 4;
        }
        slider.value = health;
    }
    public void SetHealth(int health) {
        if (health < 0) {
            slider.value = 0;
        }
        if (health > 4) {
            slider.value = 4;
        }
        slider.value = health;
    }
}
