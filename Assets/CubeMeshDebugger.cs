using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMeshDebugger : MonoBehaviour
{
    private class EdgeComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] edge1, int[] edge2)
        {
            return (edge1[0] == edge2[0] && edge1[1] == edge2[1]) ||
                   (edge1[0] == edge2[1] && edge1[1] == edge2[0]);
        }

        public int GetHashCode(int[] edge)
        {
            return edge[0].GetHashCode() ^ edge[1].GetHashCode();
        }
    }

    void Start()
    {
        // Get the MeshFilter component attached to the cube
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("No MeshFilter found!");
            return;
        }

        // Get the mesh data
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Create a vertex-to-index map
        Dictionary<Vector3, int> vertexToIndexMap = new Dictionary<Vector3, int>();
        List<Vector3> uniqueVertices = new List<Vector3>();
        int[] updatedTriangles = new int[triangles.Length];

        // Create unique vertex map and update triangle indices
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            if (!vertexToIndexMap.ContainsKey(vertex))
            {
                int newIndex = uniqueVertices.Count;
                uniqueVertices.Add(vertex);
                vertexToIndexMap[vertex] = newIndex;
            }
        }

        // Update triangle indices to point to unique vertex indices
        for (int i = 0; i < triangles.Length; i++)
        {
            Vector3 vertex = vertices[triangles[i]];
            updatedTriangles[i] = vertexToIndexMap[vertex];
        }

        // Use a HashSet to avoid duplicate edges
        HashSet<int[]> uniqueEdges = new HashSet<int[]>(new EdgeComparer());

        // Construct edges based on updated triangle array and discard the longest side
        for (int i = 0; i < updatedTriangles.Length; i += 3)
        {
            int v1 = updatedTriangles[i];
            int v2 = updatedTriangles[i + 1];
            int v3 = updatedTriangles[i + 2];

            Vector3 p1 = uniqueVertices[v1];
            Vector3 p2 = uniqueVertices[v2];
            Vector3 p3 = uniqueVertices[v3];

            // Calculate the lengths of the edges
            float length1 = Vector3.Distance(p1, p2);
            float length2 = Vector3.Distance(p2, p3);
            float length3 = Vector3.Distance(p3, p1);

            // Identify the longest side
            float maxLength = Mathf.Max(length1, length2, length3);

            // Add only the two shortest edges to the HashSet
            if (maxLength != length1)
            {
                uniqueEdges.Add(new int[] { v1, v2 });
            }
            if (maxLength != length2)
            {
                uniqueEdges.Add(new int[] { v2, v3 });
            }
            if (maxLength != length3)
            {
                uniqueEdges.Add(new int[] { v3, v1 });
            }
        }

        // Print unique edges
        string uniqueEdgeList = "Unique Edges: ";
        foreach (int[] edge in uniqueEdges)
        {
            Vector3 p1 = uniqueVertices[edge[0]];
            Vector3 p2 = uniqueVertices[edge[1]];
            uniqueEdgeList += $"({p1.x}, {p1.y}, {p1.z}) to ({p2.x}, {p2.y}, {p2.z}), ";
        }
        Debug.Log(uniqueEdgeList);

        // Output the counts
        Debug.Log($"Vertex Count: {uniqueVertices.Count}");
        Debug.Log($"Triangle Count: {updatedTriangles.Length / 3}");
        Debug.Log($"Unique Edge Count: {uniqueEdges.Count}");
    }
}
