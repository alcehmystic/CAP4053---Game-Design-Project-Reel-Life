using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingUI : MonoBehaviour
{
    public Image progressBar;
    public Image progressBarFill;
    public TMP_Text fishingText;
    public TMP_Text difficultyText;
    // Start is called before the first frame update
    
    public Image GetProgressBar() {
        return progressBar;
    }

    public Image GetProgressBarFill() {
        return progressBarFill;
    }

    public TMP_Text GetProgressText() {
        return fishingText;
    }

    public TMP_Text GetDifficultyText() {
        return difficultyText;
    }

    public void SetDifficultyText(int diff) {
        switch (diff) {
            case 1:
                difficultyText.text = "Difficulty: Easy";
                break;
            case 2:
                difficultyText.text = "Difficulty: Medium";
                break;
            case 3:
                difficultyText.text = "Difficulty: Hard";
                break;
            default:
                difficultyText.text = "Difficulty: Unknown";
                break;
        }
        
    }

}
