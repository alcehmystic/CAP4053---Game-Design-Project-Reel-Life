using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositions : MonoBehaviour
{   
    public GameObject fish;
    public GameObject bobber;
    public ResizablePlane resizablePlane;

    // Start is called before the first frame update
    void Start()
    {
        ResetItemPositions();
    }

    void OnEnable()
    {
        ResetItemPositions();
    }

    void ResetItemPositions()
    {
        if (fish != null && bobber != null && resizablePlane != null)
        {
            Vector3 center = resizablePlane.GetCenterPosition();
            fish.gameObject.transform.localPosition = center;
            bobber.gameObject.transform.localPosition = center + new Vector3(0, 0.3f, -2.5f);
        }
        else
        {
            Debug.LogWarning("One or more GameObjects are not assigned in the inspector.");
        }
    }
}
