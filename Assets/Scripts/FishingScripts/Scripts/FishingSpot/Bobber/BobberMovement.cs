using UnityEngine;

public class BobberMovement : MonoBehaviour
{
    public ResizablePlane waterPlane;  // Assign your ResizablePlane in Inspector
    public Camera mainCamera;          // Assign Main Camera
    public float yOffset = 0.1f;       // Height above plane surface

    public bool isActive = false;

    private float waterY;              // Base Y position
    public float bobAmplitude = 0.05f; // Max vertical bob amount
    public float bobFrequency = 2f;    // Speed of bobbing

    public float horizontalBobAmplitude = 0.02f;
    public float horizontalBobFrequency = 1.5f;

    private Vector3 targetPosition;
    private Vector3 bobOffset;

    private bool isTouchingFish;

    void Start()
    {
        if (waterPlane == null)
        {
            Debug.LogError("ResizablePlane not assigned!");
            return;
        }

        // Set Y position based on plane's position
        waterY = waterPlane.transform.position.y + yOffset;

        isTouchingFish = false;
    }

    void Update()
    {
        // if (!isActive) return;
        FollowMouseOnPlane();
        ApplyBobbingEffect();
    }

    public void SetActiveState(bool state)
    {
        isActive = state;
        GetComponent<Collider>().enabled = state;
    }

    void FollowMouseOnPlane()
    {
        if (mainCamera == null || waterPlane == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, waterY, 0));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 localPos = waterPlane.transform.InverseTransformPoint(hitPoint);

            if (!IsPointInsidePolygon(localPos, waterPlane.corners))
            {
                localPos = GetClosestPointOnPolygon(localPos, waterPlane.corners);
            }

            targetPosition = waterPlane.transform.TransformPoint(localPos);
        }
    }

    void ApplyBobbingEffect()
    {
        float time = Time.time;

        // Vertical and slight horizontal bobbing
        float verticalOffset = Mathf.Sin(time * bobFrequency) * bobAmplitude;
        float xOffset = Mathf.Sin(time * horizontalBobFrequency) * horizontalBobAmplitude;
        float zOffset = Mathf.Cos(time * horizontalBobFrequency) * horizontalBobAmplitude;

        bobOffset = new Vector3(xOffset, verticalOffset, zOffset);

        Vector3 finalPosition = targetPosition + bobOffset;
        finalPosition.y = waterY + verticalOffset;

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * 10f);
    }

    bool IsPointInsidePolygon(Vector3 point, Vector3[] polygon)
    {
        bool inside = false;
        int j = polygon.Length - 1;

        for (int i = 0; i < polygon.Length; i++)
        {
            if ((polygon[i].z > point.z) != (polygon[j].z > point.z) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
            {
                inside = !inside;
            }
            j = i;
        }
        return inside;
    }

    Vector3 GetClosestPointOnPolygon(Vector3 point, Vector3[] polygon)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < polygon.Length; i++)
        {
            int j = (i + 1) % polygon.Length;
            Vector3 edgeStart = polygon[i];
            Vector3 edgeEnd = polygon[j];

            Vector3 edgeDirection = edgeEnd - edgeStart;
            float edgeLength = edgeDirection.magnitude;
            edgeDirection.Normalize();

            Vector3 pointDirection = point - edgeStart;
            float projection = Mathf.Clamp(Vector3.Dot(pointDirection, edgeDirection), 0, edgeLength);
            Vector3 closestEdgePoint = edgeStart + edgeDirection * projection;

            float distance = Vector3.Distance(point, closestEdgePoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = closestEdgePoint;
            }
        }
        return closestPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fish"))
            isTouchingFish = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Fish"))
            isTouchingFish = false;
    }

    public bool isColliding()
    {
        return isTouchingFish;
    }
}
