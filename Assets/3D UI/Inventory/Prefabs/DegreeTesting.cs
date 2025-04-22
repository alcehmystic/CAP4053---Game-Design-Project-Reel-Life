using UnityEngine;

public class DegreeTesting : MonoBehaviour
{
    void Start()
    {
        // Rotate the object 18 degrees around its local Z-axis
        transform.Rotate(0, 18, 0, Space.Self);
        
        // Get the new rotation in Euler angles
        Vector3 newRotation = transform.rotation.eulerAngles;
        
        // Print the new rotation
        Debug.Log("New Rotation after 18 degrees: " + newRotation);
    }
}