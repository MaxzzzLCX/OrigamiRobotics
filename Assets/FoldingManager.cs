using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FoldingManager : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isCornerSelected = false;
    public GameObject cornerNode;
    public Vector3 initialPosition;
    public Vector3 finalPosition;
    public LineRenderer foldLineRenderer;
    public float lineLength = 5.0f;


    public GameObject paper;  // Paper GameObject with a Mesh
    public MeshFilter meshFilter;  // Reference to the mesh filter
    public Vector3[] paperVertices;
    public int[] paperTriangles;

    private Vector3[] originalVertices;
    private Vector3[] upperVertices;
    private Vector3[] lowerVertices;

    public GameObject newObject;

    public HashSet<int[]> uniqueEdges;
    public List<Vector3> uniqueVertices;
    public Vector3[] foldlinePoints;

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
            foldlinePoints = TrackKeyPoints();
            //Vector3[] keypoints = TrackKeyPoints();
            DrawFoldLine(foldlinePoints[2], foldlinePoints[3]);
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

        ExtractEdgeOfOriginalPaper();
        List<Vector3> intersections = CalculateIntersectionPoints();

        List<Vector3> groupA = new List<Vector3>();
        List<Vector3> groupB = new List<Vector3>();
        SegmentPaperByFold(uniqueVertices,keypoints[2], keypoints[3], out groupA, out groupB);

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

        //Debug.Log($"initial {initialPosition}, final {finalPosition}");
        //Debug.Log($"midpoint {midpoint}, Frist point {linepoint1}, Second point {linepoint2}");

        linepoint1.z = midpoint.z;
        linepoint2.z = midpoint.z;
        //Debug.Log($"change point1 to {linepoint1} and point 2 to {linepoint2}");

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

   
    private void ExtractEdgeOfOriginalPaper()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Create a vertex-to-index map
        Dictionary<Vector3, int> vertexToIndexMap = new Dictionary<Vector3, int>();
        uniqueVertices = new List<Vector3>();
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
        uniqueEdges = new HashSet<int[]>(new EdgeComparer());

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

    // Find the intersection points. Group the points into two new sets of vertices
    // Form two new game objects based on these vertices and triangles

    // Finds the intersection point

   


    private List<Vector3> CalculateIntersectionPoints()
    {
        // Function that determines whether two segments intersect
        bool FindLineIntersection2D(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
        {
            // Initialize the output intersection point
            intersection = Vector3.zero;

            // Convert the points to 2D (XY plane)
            Vector2 A = new Vector2(p1.x, p1.y);
            Vector2 B = new Vector2(p2.x, p2.y);
            Vector2 C = new Vector2(p3.x, p3.y);
            Vector2 D = new Vector2(p4.x, p4.y);

            // Calculate the denominator
            float denominator = (B.x - A.x) * (D.y - C.y) - (B.y - A.y) * (D.x - C.x);

            // Check if lines are parallel (denominator is zero)
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                return false; // Lines are parallel or coincident
            }

            // Calculate the numerators for the line intersection equations
            float tNumerator = (C.x - A.x) * (D.y - C.y) - (C.y - A.y) * (D.x - C.x);
            float uNumerator = (C.x - A.x) * (B.y - A.y) - (C.y - A.y) * (B.x - A.x);

            // Calculate the intersection points t and u
            float t = tNumerator / denominator;
            float u = uNumerator / denominator;

            // If t and u are between 0 and 1, the intersection is within both line segments
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                // Calculate the intersection point
                intersection = new Vector3(A.x + t * (B.x - A.x), A.y + t * (B.y - A.y), p1.z); // Keep z-coordinate constant
                return true; // Lines intersect
            }

            return false; // No valid intersection within the line segments
        }

        List<Vector3> intersections = new List<Vector3>();
        Debug.Log($"Number of unique edges is {uniqueEdges.Count}");
        int i = 1;

        foreach (int[] paperEdge in uniqueEdges)
        {
            //Vector3 edgeVertex1 = uniqueVertices[paperEdge[0]];
            //Vector3 edgeVertex2 = uniqueVertices[paperEdge[1]];
            Vector3 edgeVertex1 = paper.transform.TransformPoint(uniqueVertices[paperEdge[0]]);
            Vector3 edgeVertex2 = paper.transform.TransformPoint(uniqueVertices[paperEdge[1]]);
            Vector3 foldPoint1 = foldlinePoints[2];
            Vector3 foldPoint2 = foldlinePoints[3];


            // Debug.Log($"EDGE {i}: ({edgeVertex1.x},{edgeVertex1.y}) to ({edgeVertex2.x},{edgeVertex2.y}); FOLDAXIS x,y coordinates: ({foldPoint1.x},{foldPoint1.y}) to ({foldPoint2.x},{foldPoint2.y})");

            // Only have to check intersection when the edge is a horizontal edge
            // Any vertical edge will not be cut by the fold

            if (edgeVertex1.z == edgeVertex2.z) {
                
                foldPoint1.z = edgeVertex1.z;
                foldPoint2.z = edgeVertex1.z;

                Vector3 intersectionPoint1;

                if (FindLineIntersection2D(edgeVertex1, edgeVertex2, foldPoint1, foldPoint2, out intersectionPoint1))
                {
                    intersections.Add(intersectionPoint1);
                    Debug.Log($"intersection {i} at {intersectionPoint1}");
                }
            }
            i++;
        }
        return intersections;
    }
    
    private void SegmentPaperByFold(List<Vector3> originalVertices, Vector3 foldP1, Vector3 foldP2, out List<Vector3> groupA, out List<Vector3> groupB)
    {
        //(1) Group the vertices (including new intersection vertices) into two subgroups
        //(2) For each of these subgroups, take in the group of vertices and create a game object

        void SplitVertices(List<Vector3> originalVertices, Vector3 foldP1, Vector3 foldP2, out List<Vector3> groupA, out List<Vector3> groupB)
        {
            groupA = new List<Vector3>();
            groupB = new List<Vector3>();

            Vector2 foldAxis = new Vector2(foldP2.x - foldP1.x, foldP2.y - foldP1.y);

            foreach (Vector3 vertex in originalVertices)
            {
                Vector2 vertexVector = new Vector2(vertex.x - foldP1.x, vertex.y - foldP1.y);
                float crossProduct = foldAxis.x * vertexVector.y - foldAxis.y * vertexVector.x;

                if (Mathf.Abs(crossProduct) < 1e-6f)
                {
                    // Vertex lies on the fold axis
                    // Decide how to handle this case
                    Debug.Log("Too Small");
                }
                else if (crossProduct > 0)
                {
                    groupA.Add(vertex);
                    Debug.Log($"A: {vertex}");
                }
                else
                {
                    groupB.Add(vertex);
                    Debug.Log($"B: {vertex}");
                }
            }
        }
        
        void GenerateNewPaper(List<List<Vector3>> newVertices)
        {
            foreach(List<Vector3> verticesgroups in newVertices)
            {
                
            }
        }
        
        void NewGameObject(List<Vector3> vertices)
        {

        }

        SplitVertices(originalVertices, foldP1, foldP2, out groupA, out groupB);
        
        
    }
    


    // Custom comparer to ensure edges are treated as duplicates if they share the same vertices
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
    /*
    private void TestIntersection()
    {
        List<Vector3> intersections = new List<Vector3>();
        
        int i = 1;

        // (0.05167454,0.5048455) to (0.05167454,1.455155); FOLDAXIS x,y coordinates: (0.3168258,0.4046873) to (-0.9143568,1.980818)
        List<Vector3[]> testCases = new List<Vector3[]>();
        Vector3[] somecase = new Vector3[] { new Vector3(0.05167454f, 0.5048455f, 1.0f), new Vector3(0.05167454f, 1.455155f, 1.0f), new Vector3(0.3168258f, 0.4046873f, 1.0f), new Vector3(-0.9143568f, 1.980818f, 1.0f) };
        testCases.Add(somecase);

        foreach (Vector3[] acase in testCases)
        {
            //Vector3 edgeVertex1 = uniqueVertices[paperEdge[0]];
            //Vector3 edgeVertex2 = uniqueVertices[paperEdge[1]];
            Vector3 edgeVertex1 = acase[0];
            Vector3 edgeVertex2 = acase[1];
            Vector3 foldPoint1 = acase[2];
            Vector3 foldPoint2 = acase[3];


            Debug.Log($"EDGE {i}: ({edgeVertex1.x},{edgeVertex1.y}) to ({edgeVertex2.x},{edgeVertex2.y}); FOLDAXIS x,y coordinates: ({foldPoint1.x},{foldPoint1.y}) to ({foldPoint2.x},{foldPoint2.y})");

            // Only have to check intersection when the edge is a horizontal edge
            // Any vertical edge will not be cut by the fold

            if (edgeVertex1.z == edgeVertex2.z)
            {

                foldPoint1.z = edgeVertex1.z;
                foldPoint2.z = edgeVertex1.z;

                Vector3 intersectionPoint1;

                if (FindLineIntersection2D(edgeVertex1, edgeVertex2, foldPoint1, foldPoint2, out intersectionPoint1))
                {
                    intersections.Add(intersectionPoint1);
                    Debug.Log($"intersection {i} at {intersectionPoint1}");
                }
            }

            i++;


        }
    }
    */


}
