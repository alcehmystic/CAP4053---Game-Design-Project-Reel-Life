using UnityEngine;

public class BobberMovement : MonoBehaviour
{
    public ResizablePlane waterPlane;  // Assign your ResizablePlane in Inspector
    public Camera mainCamera;          // Assign Main Camera
    public float yOffset = 0.1f;       // Height above plane surface
    
    public bool isActive = false;

    private float waterY;              // Fixed Y position for the bobber

    void Start()
    {
        if (waterPlane == null)
        {
            Debug.LogError("ResizablePlane not assigned!");
            return;
        }

        // Set Y position based on plane's position
        waterY = waterPlane.transform.position.y + yOffset;
    }

    void Update()
    {
        // if (!isActive) return;
        FollowMouseOnPlane();
    }

    public void SetActiveState(bool state)
    {
        isActive = state;
        GetComponent<Collider>().enabled = state;
    }

    void FollowMouseOnPlane()
    {
        if (mainCamera == null || waterPlane == null) return;

        // Get mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, waterY, 0));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            
            // Convert to plane's local space
            Vector3 localPos = waterPlane.transform.InverseTransformPoint(targetPosition);
            
            // Ensure position stays within custom plane bounds
            if (!IsPointInsidePolygon(localPos, waterPlane.corners))
            {
                localPos = GetClosestPointOnPolygon(localPos, waterPlane.corners);
            }

            // Convert back to world space and maintain Y position
            targetPosition = waterPlane.transform.TransformPoint(localPos);
            targetPosition.y = waterY;

            // Smooth movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 20f);
        }
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
}