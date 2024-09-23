using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SlicingManager : MonoBehaviour
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
    public float zOfPaperSurface;
    private Vector3[] keypoints;

    private Vector3[] originalVertices;
    private Vector3[] upperVertices;
    private Vector3[] lowerVertices;

    public Material crossSectionMaterial;

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
        zOfPaperSurface = paperVertices[0].z;
        foreach (Vector3 vertex in paperVertices)
        {
            if (vertex.z < zOfPaperSurface)
            {
                zOfPaperSurface = vertex.z; //minimum z
            }
        }
        //// Store the original vertices
        //originalVertices = meshFilter.mesh.vertices;
    }

    private void Update()
    {
        if (isCornerSelected)
        {
            finalPosition = cornerNode.transform.position;
            keypoints = TrackKeyPoints();
            //Vector3[] keypoints = TrackKeyPoints();
            Vector3 point1 = keypoints[2];
            Vector3 point2 = keypoints[3];
            //point1.z = point1.z - 0.3f;
            //point2.z = point2.z - 0.3f;
            DrawFoldLine(point1, point2);
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

        paper.GetComponent<SlicingScript>().SegmentPaper(keypoints[2], keypoints[3], keypoints[4]);
        Debug.Log("Segment Finish");

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


        //linepoint1.z = zOfPaperSurface;
        //linepoint2.z = zOfPaperSurface; // TODO: figure out how to use the paper surface z
        linepoint1.z = midpoint.z;
        linepoint2.z = midpoint.z;
        //Debug.Log($"change point1 to {linepoint1} and point 2 to {linepoint2}");

        return new Vector3[] { initialPosition, finalPosition, linepoint1, linepoint2, midpoint };



    }
    private void DrawFoldLine(Vector3 point1, Vector3 point2)
    {
        if (foldLineRenderer != null)
        {
            foldLineRenderer.SetPosition(0, point1);
            foldLineRenderer.SetPosition(1, point2);
        }
    }
    
    private void SegmentPaper(Vector3 point1, Vector3 point2, Vector3 midpoint)
    {
        Vector3 foldAxis = point1 - point2;
        Vector3 planeNormal = Vector3.Cross(foldAxis, new Vector3(0f, 0f, 1f)).normalized;
        Vector3 planePosition = midpoint; // A point on the fold axis

        SlicedHull slicedObject = paper.Slice(planePosition, planeNormal, crossSectionMaterial);

        if (slicedObject != null)
        {
            // Create upper and lower hulls
            GameObject upperHull = slicedObject.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHull = slicedObject.CreateLowerHull(gameObject, crossSectionMaterial);

            // Position the new objects
            upperHull.transform.position = transform.position;
            lowerHull.transform.position = transform.position;

            // Destroy the original object
            gameObject.SetActive(false);
            // Destroy(gameObject);
        }
        else
        {
            Debug.Log("Error Slicing");
        }



    }



}
