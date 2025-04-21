using UnityEngine;
using System.Collections;

public class OrganicFishBehavior : MonoBehaviour
{
    [Header("Initial State Settings")]
    public GameObject sphereVisual;
    public GameObject fishVisual;
    public float initialDuration = 5f;
    public float initialSpeed = 0.5f;

    [Header("Normal State Parameters")]
    [SerializeField] private float baseSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float speedVariation = 0.3f;
    [SerializeField] private float wanderStrength = 1f;
    [SerializeField] private float edgeAvoidDistance = 2f;
    [SerializeField] private float edgeSteerWeight = 3f;

    private ResizablePlane resizablePlane;
    private Vector3[] worldSpaceCorners;
    private float currentSpeed;
    private Vector3 smoothDirection;
    private float fixedY;
    private float speedOffset;
    public bool isInitialState = true;
    private Coroutine initialStateRoutine;

    private float[,] stateValues;

    void Awake()
    {

        //2D Array to hold Initial State / Normal State values
        stateValues = new float[2, 5] 
        {
            {0.5f, 0.2f, 0.5f, 2f, 5f},
            {baseSpeed, speedVariation, wanderStrength, edgeAvoidDistance, edgeSteerWeight}
        };

        
    }

    void OnEnable()
    {
        isInitialState = true;
        InitializeReferences();
        fixedY = transform.position.y;
        InitializeInitialState();
    }

    void InitializeReferences()
    {
        resizablePlane = transform.parent.GetComponentInChildren<ResizablePlane>();
        if (resizablePlane != null)
        {
            worldSpaceCorners = System.Array.ConvertAll(resizablePlane.corners, 
                corner => resizablePlane.transform.parent.parent.TransformPoint(corner));
        }
    }

    public void SetFishValues(int[] values)
    {
        stateValues[1, 0] = values[0];
        stateValues[1, 1] = values[1];
        stateValues[1, 2] = values[2];
        stateValues[1, 3] = values[3];
        stateValues[1, 4] = values[4];
    }

    void InitializeInitialState()
    {
        sphereVisual.SetActive(true);
        fishVisual.SetActive(false);

        //initial state values
        baseSpeed = stateValues[0,0];
        speedVariation = stateValues[0,1];
        wanderStrength = stateValues[0,2];
        edgeAvoidDistance = stateValues[0,3];
        edgeSteerWeight = stateValues[0,4];


        transform.position = resizablePlane.transform.TransformPoint(resizablePlane.centerPoint);
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
        initialStateRoutine = StartCoroutine(InitialStateCountdown());
    }

    IEnumerator InitialStateCountdown()
    {
        yield return new WaitForSeconds(initialDuration);
        EndInitialState();
    }

    void EndInitialState()
    {
        if (!isInitialState) return;
        
        isInitialState = false;
        sphereVisual.SetActive(false);
        fishVisual.SetActive(true);

        //normal state values
        baseSpeed = stateValues[1,0];
        speedVariation = stateValues[1,1];
        wanderStrength = stateValues[1,2];
        edgeAvoidDistance = stateValues[1,3];
        edgeSteerWeight = stateValues[1,4];

        speedOffset = Random.Range(-speedVariation, speedVariation);
        
        if(initialStateRoutine != null) 
            StopCoroutine(initialStateRoutine);
    }

    void Update()
    {
        // if(isInitialState)
        // {
        //     UpdateInitialMovement();
        // }
        // else
        // {
        //     UpdateNormalMovement();
        //     ApplyOrganicSpeedVariation();
        //     MaintainBounds();
        // }

        ApplyOrganicSpeedVariation();
        UpdateNormalMovement();
        MaintainBounds();
    }

    void UpdateInitialMovement()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
        CheckInitialBounds();
    }

    void CheckInitialBounds()
    {
        if(!IsPointInsidePolygon(transform.position, worldSpaceCorners))
        {
            Vector3 newDir = (resizablePlane.transform.TransformPoint(resizablePlane.centerPoint) - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    void UpdateNormalMovement()
    {
        Vector3 desiredDirection = CalculateSteeringDirection();
        smoothDirection = Vector3.Lerp(smoothDirection, desiredDirection, rotationSpeed * Time.deltaTime);
        smoothDirection.Normalize();

        transform.position += smoothDirection * (currentSpeed + speedOffset) * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
        
        RotateTowardsDirection();
    }

    Vector3 CalculateSteeringDirection()
    {
        // 1. Generate base wander direction (circular distribution)
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float wanderX = randomDir.x;
        float wanderZ = randomDir.y;

        // 2. Apply Perlin-based smooth variation
        float perlinX = Mathf.PerlinNoise(Time.time * 0.3f, 0) - 0.5f;
        float perlinZ = Mathf.PerlinNoise(Time.time * 0.3f + 1000, 100) - 0.5f;
        
        // 3. Blend random and Perlin directions
        Vector3 wanderTarget = smoothDirection + new Vector3(
            (wanderX + perlinX * 0.3f),
            0,
            (wanderZ + perlinZ * 0.3f)
        ).normalized * wanderStrength;

        // 4. Apply edge avoidance
        Vector3 edgeAvoid = CalculateEdgeAvoidance();
        
        return (wanderTarget + edgeAvoid.normalized * edgeSteerWeight).normalized;
    }

Vector3 CalculateEdgeAvoidance()
{
    Vector3 steerAway = Vector3.zero;
    int edgeHits = 0;

    // Sample 8 directions around the fish for balanced avoidance
    for (int i = 0; i < 8; i++)
    {
        float angle = i * Mathf.PI * 0.25f; // 45-degree increments
        Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        Vector3 predictedPos = transform.position + dir * edgeAvoidDistance;

        if (!IsPointInsidePolygon(predictedPos, worldSpaceCorners))
        {
            Vector3 closestPoint = GetClosestPointOnPolygon(predictedPos, worldSpaceCorners);
            steerAway += (closestPoint - predictedPos).normalized;
            edgeHits++;
        }
    }

    // Average the avoidance vectors if multiple edges detected
    return edgeHits > 0 ? steerAway / edgeHits : Vector3.zero;
}

    void RotateTowardsDirection()
    {
        if (smoothDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(smoothDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void ApplyOrganicSpeedVariation()
    {
        currentSpeed = baseSpeed + Mathf.Sin(Time.time) * speedVariation;
    }

    void MaintainBounds()
    {

        if (!IsPointInsidePolygon(transform.position, worldSpaceCorners))
        {
            Vector3 safePosition = GetClosestPointOnPolygon(transform.position, worldSpaceCorners);
            
            // NEW: Calculate a direction back to safety while considering current momentum
            Vector3 correctionDirection = (safePosition - transform.position).normalized;
            float momentumInfluence = Vector3.Dot(smoothDirection, correctionDirection);
            
            // Blend between pure correction and momentum-preserving move
            Vector3 blendedDirection = (correctionDirection * 0.7f + smoothDirection * 0.3f).normalized;
            
            // Apply correction with momentum influence
            transform.position += blendedDirection * (currentSpeed + speedOffset) * Time.deltaTime;
            
            // Force ensure we're inside bounds
            if (!IsPointInsidePolygon(transform.position, worldSpaceCorners))
            {
                transform.position = safePosition;
            }
            
            // NEW: Immediately update smoothDirection to prevent oscillation
            smoothDirection = blendedDirection;

            Debug.DrawLine(transform.position, safePosition, Color.red, 0.5f);
            Debug.DrawRay(transform.position, blendedDirection * 2f, Color.green, 0.5f);
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if(isInitialState && other.CompareTag("Bobber"))
        {
            EndInitialState();
        }
    }

    // Original Polygon Functions
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
}