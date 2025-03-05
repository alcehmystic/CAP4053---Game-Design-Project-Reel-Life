using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedGlobalRotation : MonoBehaviour
{
    public Vector3 fixedRotation; // Set this to the desired global rotation in the Inspector
    
    void Start() {
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}