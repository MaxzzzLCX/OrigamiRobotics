using UnityEngine;

public class PaperFoldingManager : MonoBehaviour
{
    public GameObject paper;  // Paper GameObject with a Mesh
    public Vector3 foldLineStart;
    public Vector3 foldLineEnd;
    public Vector3 foldAxis;
    public MeshFilter meshFilter;  // Reference to the mesh filter

    private Vector3[] originalVertices;
    private Vector3[] upperVertices;
    private Vector3[] lowerVertices;

    void Start()
    {
        // Get the mesh filter of the paper
        meshFilter = paper.GetComponent<MeshFilter>();
        // Store the original vertices
        originalVertices = meshFilter.mesh.vertices;

        // Apply fold after calculating fold line and axis
        ApplyFold();
    }

    private void ApplyFold()
    {
        // Calculate the fold plane (using the fold line)
        Plane foldPlane = new Plane(foldLineEnd - foldLineStart, foldLineStart);

        // Split vertices into two arrays (above and below the fold line)
        SplitVertices(foldPlane);

        // Now we can animate the upperVertices around the foldAxis
    }

    private void SplitVertices(Plane foldPlane)
    {
        // Initialize arrays to hold vertices for each side
        upperVertices = new Vector3[originalVertices.Length];
        lowerVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            // Convert local vertex positions to world space
            Vector3 worldVertex = paper.transform.TransformPoint(originalVertices[i]);

            // Determine which side of the fold the vertex is on
            if (foldPlane.GetSide(worldVertex))
            {
                // Above the fold line
                upperVertices[i] = originalVertices[i];
                lowerVertices[i] = Vector3.zero; // Mark it as not existing
            }
            else
            {
                // Below the fold line
                lowerVertices[i] = originalVertices[i];
                upperVertices[i] = Vector3.zero; // Mark it as not existing
            }
        }

        // After segmenting the vertices, you can create two separate meshes
        CreateFoldedMeshes();
    }

    private void CreateFoldedMeshes()
    {
        // Create mesh for the upper part
        Mesh upperMesh = new Mesh();
        upperMesh.vertices = upperVertices;
        upperMesh.triangles = meshFilter.mesh.triangles; // Use original triangles for now
        upperMesh.RecalculateNormals();

        // Create mesh for the lower part
        Mesh lowerMesh = new Mesh();
        lowerMesh.vertices = lowerVertices;
        lowerMesh.triangles = meshFilter.mesh.triangles; // Use original triangles for now
        lowerMesh.RecalculateNormals();

        // For now, just visualize the meshes in two separate game objects (for debugging)
        GameObject upperPart = new GameObject("Upper Part");
        upperPart.AddComponent<MeshFilter>().mesh = upperMesh;
        upperPart.AddComponent<MeshRenderer>().material = paper.GetComponent<MeshRenderer>().material;

        GameObject lowerPart = new GameObject("Lower Part");
        lowerPart.AddComponent<MeshFilter>().mesh = lowerMesh;
        lowerPart.AddComponent<MeshRenderer>().material = paper.GetComponent<MeshRenderer>().material;

        // You can then animate the upper part rotating around the fold axis
    }
}
