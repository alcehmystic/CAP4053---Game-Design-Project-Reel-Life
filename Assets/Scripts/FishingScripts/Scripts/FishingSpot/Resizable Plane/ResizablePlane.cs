using UnityEngine;

[ExecuteInEditMode]
public class ResizablePlane : MonoBehaviour
{
    public Vector3[] corners = new Vector3[] // Default to a rectangle
    {
        new Vector3(-5, 0, -5),
        new Vector3(5, 0, -5),
        new Vector3(5, 0, 5),
        new Vector3(-5, 0, 5)
    };

    public Vector3 centerPoint;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void OnValidate()
    {
        UpdatePlane();
    }

    private void Start()
    {

        UpdatePlane();
    }

    public void UpdatePlane()
    {
        if (corners.Length < 3)
        {
            Debug.LogWarning("A plane needs at least 3 corners.");
            return;
        }

        Mesh mesh = new Mesh();

        // Generate vertices and triangles for the mesh
        mesh.vertices = corners;

        // Generate triangles using a simple triangulation method
        int[] triangles = new int[(corners.Length - 2) * 3];
        for (int i = 0; i < corners.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.triangles = triangles;

        // Generate UVs
        Vector2[] uvs = new Vector2[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            uvs[i] = new Vector2(corners[i].x, corners[i].z);
        }
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        centerPoint = CalculateCenter();

    }

    public Vector3 CalculateCenter()
    {
        // Method 2: True centroid calculation (mathematically precise)
        float area = 0;
        Vector3 centroid = Vector3.zero;

        for (int i = 0; i < corners.Length; i++)
        {
            int j = (i + 1) % corners.Length;
            float cross = corners[i].x * corners[j].z - corners[j].x * corners[i].z;
            area += cross;
            centroid.x += (corners[i].x + corners[j].x) * cross;
            centroid.z += (corners[i].z + corners[j].z) * cross;
        }

        area *= 0.5f;
        centroid.x /= (6 * area);
        centroid.z /= (6 * area);
        centroid.y = corners[0].y; // Use the plane's fixed Y

        return centroid;
    }

    public void AddCorner(Vector3 newCorner)
    {
        System.Array.Resize(ref corners, corners.Length + 1);
        corners[corners.Length - 1] = newCorner;
        UpdatePlane();
    }

    public void RemoveCorner(int index)
    {
        if (corners.Length > 3 && index >= 0 && index < corners.Length)
        {
            for (int i = index; i < corners.Length - 1; i++)
            {
                corners[i] = corners[i + 1];
            }
            System.Array.Resize(ref corners, corners.Length - 1);
            UpdatePlane();
        }
    }

    public Vector3 GetCenterPosition()
    {
        return CalculateCenter();
    }
}