using System.Collections; 
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public static FishBehavior Instance {get; private set;}

    //Initial State Settings
    public GameObject sphereVisual;
    public GameObject fishVisual;
    public float initialDuration = 5f;
    public float initialSpeed = 0.5f;
    public bool isActive = false;
    
    //Normal State Settings
    private float baseSpeed;
    private float directionChangeInterval;
    private float coneAngle;
    private float rotationSpeed = 500f;

    private ResizablePlane resizablePlane;
    private bool isInitialState;
    private float originalBaseSpeed;
    private float originalDirectionChangeInterval;
    private Vector3[] worldSpaceCorners;

    // Existing variables
    private Vector3 movementDirection;
    private float timeSinceLastChange;
    private float fixedY;
    private float currentSpeed;
    private float scaleMod;
    private static int difficulty;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else
        Destroy(gameObject);
    }

    void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        InitializeFish();
    }
    void InitializeFish()
    {
        fixedY = transform.position.y; // Store initial Y position

        // Find the ResizablePlane in the correct spot
        resizablePlane = transform.parent.GetComponentInChildren<ResizablePlane>();

        worldSpaceCorners = System.Array.ConvertAll(resizablePlane.corners, corner => resizablePlane.transform.parent.parent.TransformPoint(corner));

        if (resizablePlane == null)
        {
            Debug.LogError("No ResizablePlane found in the scene!");
        }
        
        scaleMod = Random.Range(1f, 1.5f);

        InitializeInitialState();
    }

    void Update()
    {
        // if(!isActive) return;
        // Debug.Log(transform.position);
        MoveFish();
        RotateFish();
        
    }

     void InitializeInitialState()
    {
        isInitialState = true;
        
        // Set visuals
        sphereVisual.SetActive(true);
        fishVisual.SetActive(false);
        
        // Position at plane center
        if (resizablePlane != null)
        {
            transform.position = resizablePlane.transform.TransformPoint(resizablePlane.centerPoint);
            
            transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
            Debug.Log("fish position: " + transform.position);
        }

        // Configure initial movement
        baseSpeed = initialSpeed;
        ChangeDirection();
        
        StartCoroutine(InitialStateTimer());
    }

    IEnumerator InitialStateTimer()
    {
        yield return new WaitForSeconds(initialDuration);
        EndInitialState();
    }

    void EndInitialState()
    {
        if (!isInitialState) return;
        
        isInitialState = false;
        
        // Restore visuals
        sphereVisual.SetActive(false);
        fishVisual.SetActive(true);

        gameObject.transform.localScale = new Vector3(scaleMod, scaleMod, scaleMod);
        
        // Restore movement parameters
        SetDifficulty();
        
        // Enable normal behavior
        ChangeDirection(); // Set initial proper direction
    }

    void SetDifficulty() {
        // Randomly select difficulty (1-3)
        difficulty = Random.Range(1, 4);

        // Get settings from FishingManager
        FishingManager.FishDifficultySettings settings = FishingManager.Instance.GetDifficultySettings(difficulty);

        // Apply difficulty settings
        coneAngle = settings.coneAngle;
        baseSpeed = settings.baseSpeed;
        directionChangeInterval = settings.directionChangeInterval;

        // UIManager.Instance._fishingUI.GetComponent<FishingUI>().SetDifficultyText(difficulty);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInitialState && other.CompareTag("Bobber"))
        {
            StopAllCoroutines();
            EndInitialState();
        }
    }

    public bool GetInitialState() 
    {
        return isInitialState;
    }

    void ChangeDirection()
    {
        // Randomize speed within �25% of base speed
        currentSpeed = baseSpeed * Random.Range(0.75f, 1.25f);

        // Get a new movement direction within the defined cone
        movementDirection = GetRandomDirectionWithinCone(transform.forward, coneAngle);
        movementDirection.y = 0f; // Prevent Y-axis movement
        movementDirection.Normalize();
    }

    void MoveFish()
    {
        // Move fish forward while keeping Y position constant
        transform.position += movementDirection * currentSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= directionChangeInterval)
        {
            ChangeDirection();
            timeSinceLastChange = 0f;
        }

        CheckBounds();
    }

    void RotateFish()
    {
        // Determine the target Y rotation based on movement direction
        float targetRotationY = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;

        // Gradually rotate towards the new direction using transform.Rotate()
        float rotationStep = rotationSpeed * Time.deltaTime;
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, targetRotationY, rotationStep);

        // Apply the rotation change using transform.Rotate()
        transform.rotation = Quaternion.Euler(0, newY, 0);
    }

    Vector3 GetRandomDirectionWithinCone(Vector3 forward, float angle)
    {
        // Generate a random rotation within the allowed angle range
        Quaternion randomRotation = Quaternion.Euler(
            0, // No X rotation (no pitching up/down)
            Random.Range(-angle / 2, angle / 2), // Random Y rotation within cone
            0  // No Z rotation (no rolling)
        );

        return randomRotation * forward;
    }

    void CheckBounds()
    {
        if (resizablePlane == null || resizablePlane.corners.Length < 3)
        {
            Debug.LogWarning("ResizablePlane not found or invalid.");
            return;
        }

        // Check if the fish is outside the plane's bounds
        if (!IsPointInsidePolygon(transform.position, worldSpaceCorners))
        {
            // Find the closest point on the plane's boundary
            Vector3 closestPoint = GetClosestPointOnPolygon(transform.position, worldSpaceCorners);

            // Calculate the reflection normal
            Vector3 normal = (transform.position - closestPoint).normalized;
            normal.y = 0; // Ensure no Y-axis movement

            // Reflect the movement direction
            movementDirection = Vector3.Reflect(movementDirection, normal);
            movementDirection.y = 0; // Prevent Y movement
            movementDirection.Normalize();

            // Apply a slight random adjustment within the cone
            Quaternion randomAdjustment = Quaternion.Euler(0, Random.Range(-coneAngle / 2, coneAngle / 2), 0);
            movementDirection = randomAdjustment * movementDirection;

            // Move the fish back inside the bounds
            transform.position = closestPoint;

            // Rotate the fish naturally after bouncing
            RotateFish();
            timeSinceLastChange = directionChangeInterval/2;
        }
    }

    bool IsPointInsidePolygon(Vector3 point, Vector3[] polygon)
    {
        // Raycasting algorithm to check if a point is inside a polygon
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
        // Find the closest point on the polygon's edges to the given point
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
            float projection = Vector3.Dot(pointDirection, edgeDirection);
            projection = Mathf.Clamp(projection, 0, edgeLength);

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

    public int GetDifficulty() {
        return difficulty;
    }
}