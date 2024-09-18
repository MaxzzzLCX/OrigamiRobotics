using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class FoldingManager : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isCornerSelected = false;
    public GameObject cornerNode;
    public Vector3 initialPosition;
    public Vector3 finalPosition;
    public LineRenderer foldLineRenderer;
    public float lineLength = 2.0f;


    public GameObject paper;  // Paper GameObject with a Mesh
    public MeshFilter meshFilter;  // Reference to the mesh filter
    public Vector3[] paperVertices;
    public int[] paperTriangles;

    private Vector3[] originalVertices;
    private Vector3[] upperVertices;
    private Vector3[] lowerVertices;

    public GameObject newObject;

    void Start()
    {
        // Ensure the LineRenderer is configured
        if (foldLineRenderer != null)
        {
            foldLineRenderer.positionCount = 2;  // We only need two points to draw a line
            foldLineRenderer.SetPosition(0, new Vector3(0, 0, 0)); // Start point
            foldLineRenderer.SetPosition(1, new Vector3(1, 0, 0)); // End point
        }

        //// Get the mesh filter of the paper
        meshFilter = paper.GetComponent<MeshFilter>();
        paperVertices = meshFilter.mesh.vertices;
        paperTriangles = meshFilter.mesh.triangles;
        //// Store the original vertices
        //originalVertices = meshFilter.mesh.vertices;
    }

    private void Update()
    {
        if (isCornerSelected)
        {
            finalPosition = cornerNode.transform.position;
            Vector3[] keypoints = TrackKeyPoints();
            DrawFoldLine(keypoints[2], keypoints[3]);
        }
    }

    public void SelectCorner()
    {
        isCornerSelected = true;
        initialPosition = cornerNode.transform.position;
        Debug.Log("Select");
    }

    public void UnselectCorner()
    {
        isCornerSelected = false;
        finalPosition = cornerNode.transform.position;
        //Vector3 finalPositionTemp = cornerNode.transform.position;
        //finalPosition = new Vector3(finalPosition.x, finalPosition.y, initialPosition.z);
        Debug.Log("Unselect");
        Vector3[] keypoints = TrackKeyPoints();
        DrawFoldLine(keypoints[2], keypoints[3]);
        Debug.Log("Finish Drawing Line)");


        //Take the vertices of the paper sheet
        // Get the mesh filter of the paper
        meshFilter = paper.GetComponent<MeshFilter>();
        // Store the original vertices
        Vector3[] vertices = meshFilter.mesh.vertices;
        Debug.Log(vertices.Length);

        foreach (Vector3 vertex in vertices)
        {
            Debug.Log(vertex);
        }



        CreateNewObjectFromCurrentObject(paperVertices, paperTriangles);
        Debug.Log("FINISH CREATING OBJECT");

        CalculateIntersections();
    }

    private Vector3[] TrackKeyPoints()
    {
        // Calculate the direction from the initial position to the release position
        Vector3 foldDirection = finalPosition - initialPosition;
        foldDirection.Normalize();  // Get the normalized direction vector

        // Find the fold axis, perpendicular to the plane of the paper and foldDirection
        Vector3 paperNormal = new Vector3(0, 0, 1);  // Assuming paper is on XY plane (normal points up along Z)
        Vector3 foldAxis = Vector3.Cross(paperNormal, foldDirection); // Cross product to get the fold axis
        foldAxis.Normalize();

        //Calculate two points to draw the fold-line
        Vector3 midpoint = (finalPosition + initialPosition) / 2;
        Vector3 linepoint1 = midpoint + foldAxis * lineLength;
        Vector3 linepoint2 = midpoint - foldAxis * lineLength;

        Debug.Log($"initial {initialPosition}, final {finalPosition}");
        Debug.Log($"midpoint {midpoint}, Frist point {linepoint1}, Second point {linepoint2}");

        linepoint1.z = midpoint.z;
        linepoint2.z = midpoint.z;
        Debug.Log($"change point1 to {linepoint1} and point 2 to {linepoint2}");

        return new Vector3[] { initialPosition, finalPosition, linepoint1, linepoint2 };



    }
    private void DrawFoldLine(Vector3 point1, Vector3 point2)
    {
        if (foldLineRenderer != null)
        {
            foldLineRenderer.SetPosition(0, point1);
            foldLineRenderer.SetPosition(1, point2);
        }
    }

    public void CreateNewObject()
    {
        // Creating new GameObject

        GameObject newPaper = new GameObject("NewPaper");

        // Add MeshFilter and MeshRenderer components (same as before)
        MeshFilter meshFilter = newPaper.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newPaper.AddComponent<MeshRenderer>();
        

        // Set a parent for the new GameObject
        newPaper.transform.SetParent(transform);

        




        float thickness = 0.0001f;
        // Create a new mesh
        Mesh mesh = new Mesh();

        // Define the 4-sided polygon base (in the XY plane)
        Vector3[] baseVertices = new Vector3[]
        {
            new Vector3(-1, -1, 0), // Bottom-left
            new Vector3(1, -1, 0),  // Bottom-right
            new Vector3(1, 1, 0),   // Top-right
            new Vector3(-1, 1, 0)   // Top-left
        };

        // Extrude the base vertices to create the top surface
        Vector3[] topVertices = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            topVertices[i] = baseVertices[i] + new Vector3(0, 0, thickness); // Move up by thickness
        }

        // Define the vertices for the entire mesh, including sides
        Vector3[] vertices = new Vector3[24]; // 4 (base) + 4 (top) + 16 (sides)

        // Base vertices
        baseVertices.CopyTo(vertices, 0); // Copy base vertices to indices 0-3

        // Top vertices
        topVertices.CopyTo(vertices, 4); // Copy top vertices to indices 4-7

        // Side vertices (duplicating the base and top vertices for each face)
        vertices[8] = baseVertices[0];   // Side 1 bottom-left
        vertices[9] = topVertices[0];    // Side 1 top-left
        vertices[10] = baseVertices[1];  // Side 1 bottom-right
        vertices[11] = topVertices[1];   // Side 1 top-right

        vertices[12] = baseVertices[1];  // Side 2 bottom-left
        vertices[13] = topVertices[1];   // Side 2 top-left
        vertices[14] = baseVertices[2];  // Side 2 bottom-right
        vertices[15] = topVertices[2];   // Side 2 top-right

        vertices[16] = baseVertices[2];  // Side 3 bottom-left
        vertices[17] = topVertices[2];   // Side 3 top-left
        vertices[18] = baseVertices[3];  // Side 3 bottom-right
        vertices[19] = topVertices[3];   // Side 3 top-right

        vertices[20] = baseVertices[3];  // Side 4 bottom-left
        vertices[21] = topVertices[3];   // Side 4 top-left
        vertices[22] = baseVertices[0];  // Side 4 bottom-right
        vertices[23] = topVertices[0];   // Side 4 top-right

        // Define the triangles (3 indices per triangle)
        int[] triangles = new int[]
        {
        // Base face
        0, 1, 2,
        0, 2, 3,

        // Top face (reverse winding order)
        7, 6, 5,
        7, 5, 4,

        // Side 1 face
        8, 9, 10,
        9, 11, 10,

        // Side 2 face
        12, 13, 14,
        13, 15, 14,

        // Side 3 face
        16, 17, 18,
        17, 19, 18,

        // Side 4 face
        20, 21, 22,
        21, 23, 22
        };

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals for correct lightingS
        mesh.RecalculateNormals();


        newPaper.GetComponent<MeshFilter>().mesh = mesh;

        // Optionally, you can also set its local position relative to the parent
        newPaper.transform.localPosition = new Vector3(0, 0, 0.5f); // Set the local position to (0, 0, 0) relative to the parent

        // Optionally, set local rotation or scale
        newPaper.transform.localRotation = Quaternion.identity; // Reset rotation
        newPaper.transform.localScale = new Vector3(0.15f, 0.15f, 0.0001f); // Set the scale to default (1, 1, 1)

        meshRenderer.material = new Material(Shader.Find("Standard")); // Assign a basic material for visibility


        Debug.Log("New Paper Created");

        /*
        // Assign the mesh to the MeshFilter component
        newObject.GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard")); // Assign a basic material for visibility
        newObject.transform.localScale = new Vector3 (0.15f, 0.15f, 0.0001f); // Set the scale to 1,1,1 if it's too small
        newObject.transform.localPosition = new Vector3 (2, 0, 0);  // Position the object at the origin

        */
    }

    public void CreateNewObjectFromCurrentObject(Vector3[] vertices, int[] triangles)
    {
        // Creating new GameObject

        GameObject newPaper = new GameObject("NewPaper");

        // Add MeshFilter and MeshRenderer components (same as before)
        MeshFilter meshFilter = newPaper.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newPaper.AddComponent<MeshRenderer>();


        // Set a parent for the new GameObject
        newPaper.transform.SetParent(transform);


        Mesh mesh = new Mesh();

        
        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals for correct lightingS
        mesh.RecalculateNormals();


        newPaper.GetComponent<MeshFilter>().mesh = mesh;

        // Optionally, you can also set its local position relative to the parent
        newPaper.transform.localPosition = paper.transform.localPosition + new Vector3(0.5f, 0, 0); // Set the local position to (0, 0, 0) relative to the parent

        // Optionally, set local rotation or scale
        newPaper.transform.localRotation = Quaternion.identity; // Reset rotation
        newPaper.transform.localScale = paper.transform.localScale; 

        meshRenderer.material = new Material(Shader.Find("Standard")); // Assign a basic material for visibility


        Debug.Log("New Paper Created");

        /*
        // Assign the mesh to the MeshFilter component
        newObject.GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard")); // Assign a basic material for visibility
        newObject.transform.localScale = new Vector3 (0.15f, 0.15f, 0.0001f); // Set the scale to 1,1,1 if it's too small
        newObject.transform.localPosition = new Vector3 (2, 0, 0);  // Position the object at the origin

        */
    }

    public void CalculateIntersection()
    {
        Vector3[] localVertices = paper.GetComponent<MeshFilter>().mesh.vertices;

        Vector3[] worldVertices = new Vector3[localVertices.Length];
        for (int i = 0; i < localVertices.Length; i++)
        {
            worldVertices[i] = paper.transform.TransformPoint(localVertices[i]);
            Debug.Log($"{i}: worldVertices[i]");
        }

        

    }
    // TODO: because this comment, everything is not yet tested and 100% complete. It was still under development
    private void CalculateIntersections()
    {
        // Get mesh and vertices
        Mesh mesh = paper.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Convert vertices to world space
        Vector3[] worldVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = paper.transform.TransformPoint(vertices[i]);
        }

        // Use a dictionary to store unique edges
        HashSet<Edge> edges = new HashSet<Edge>(new EdgeComparer());

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Each triangle is defined by three vertices (i, i+1, i+2)
            Vector3 p1 = worldVertices[triangles[i]];
            Vector3 p2 = worldVertices[triangles[i + 1]];
            Vector3 p3 = worldVertices[triangles[i + 2]];

            // Add edges (p1-p2), (p2-p3), (p3-p1)
            edges.Add(new Edge(p1, p2));
            edges.Add(new Edge(p2, p3));
            edges.Add(new Edge(p3, p1));
        }

        // Log the number of unique edges
        Debug.Log($"Triangles: {triangles.Length}");
        Debug.Log($"Vertices: {vertices.Length}");
        Debug.Log($"Unique edges: {edges.Count}");
        foreach (Edge edge in edges)
        {
            Debug.Log($"Edge from {edge.p1} to {edge.p2}");
        }
    }

    // Helper class to represent an edge
    class Edge
    {
        public Vector3 p1, p2;

        public Edge(Vector3 p1, Vector3 p2)
        {
            // Ensure the vertices are always stored in the same order (for comparison)
            if (p1.sqrMagnitude < p2.sqrMagnitude)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
            else
            {
                this.p1 = p2;
                this.p2 = p1;
            }
        }
    }

    // Custom comparer to ensure edges are treated as duplicates if they share the same vertices
    class EdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge edge1, Edge edge2)
        {
            // Consider edges equal if they have the same two vertices (in any order)
            return (Vector3.Distance(edge1.p1, edge2.p1) < 0.0001f && Vector3.Distance(edge1.p2, edge2.p2) < 0.0001f) ||
                   (Vector3.Distance(edge1.p1, edge2.p2) < 0.0001f && Vector3.Distance(edge1.p2, edge2.p1) < 0.0001f);
        }

        public int GetHashCode(Edge edge)
        {
            // Combine the hash codes of the two vertices to get a unique hash for the edge
            return edge.p1.GetHashCode() ^ edge.p2.GetHashCode();
        }
    }


    /*
    private void segmentPlane(Vector3[] keypoints)
    {
        Vector3 initialPos = keypoints[0];
        Vector3 finalPos = keypoints[1];
        Vector3 midpoint = (finalPos + initialPos) / 2;
        Vector3 foldStart = keypoints[2];
        Vector3 foldEnd = keypoints[3];
         
        // Calculate the fold plane (using the fold line)
        Plane foldPlane = new Plane(finalPos - initialPos, midpoint);

        // Split vertices into two arrays (above and below the fold line)
        SplitVertices(foldPlane);
        CreateFoldedMeshes();

        // Now we can animate the upperVertices around the foldAxis
    }
    private void SplitVertices(Plane foldPlane)
    {
        // Initialize arrays to hold vertices for each side
        upperVertices = new Vector3[originalVertices.Length];
        lowerVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            // Since originalVertices are in local space, no need to convert to world space
            Vector3 localVertex = originalVertices[i];

            // Determine which side of the fold plane the local vertex is on
            if (foldPlane.GetSide(localVertex))  // Checking the local vertex against the local fold plane
            {
                // Vertex is above the fold line
                upperVertices[i] = localVertex;
                lowerVertices[i] = Vector3.positiveInfinity; // Mark as not used
            }
            else
            {
                // Vertex is below the fold line
                lowerVertices[i] = localVertex;
                upperVertices[i] = Vector3.positiveInfinity; // Mark as not used
            }
        }
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

    */

}
