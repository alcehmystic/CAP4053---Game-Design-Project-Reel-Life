using UnityEngine;

public class ProjectileRoller : MonoBehaviour
{
    public float rollSpeed = 360f;

    void Update()
    {
        //Rotate around the axis perpendicular to its velocity (simulate rolling)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.velocity != Vector3.zero)
        {
            Vector3 rollAxis = Vector3.Cross(rb.velocity.normalized, Vector3.up);
            transform.Rotate(rollAxis, rollSpeed * Time.deltaTime, Space.World);
        }
    }
}
