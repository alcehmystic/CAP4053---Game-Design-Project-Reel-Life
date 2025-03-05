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

}
