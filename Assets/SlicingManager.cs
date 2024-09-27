using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.UI;
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

    public List<GameObject> allPaperSegments = new List<GameObject>();
    public List<GameObject> animatingPaperSegments = new List<GameObject>();
    public List<GameObject> paperSegmentToBeFold = new List<GameObject>();
    public GameObject animationPart;
    public int foldLayerIndex = 0;
    public float foldAxisZ;
    public int numOfPlanesToSlice = 0;

    public GameObject foldSelectionButton;
    public Vector3 FOLDP1;
    public Vector3 FOLDP2;
    public Vector3 FOLDAXIS;
    public Vector3 FOLDPOSITION;

    // These two are used to keep track of the folding steps implemented in this round. So that they can be reverted. 
    public List<GameObject> previousPaperBeforeFold = new List<GameObject>();
    public List<GameObject[]> previousPaperAfterFold = new List<GameObject[]>();

    // TODO: impose physical constraints between layers by considering the edges between paper segments
    // - Mapping between face to the edges
    // - When folding takes place, the relationship of faces and edges are updated

    public Dictionary<GameObject, Edge> PaperToConnectedPapers; //Mapping of each paper segment gameobject to all of its edges

    void Start()
    {
        allPaperSegments.Add(paper);

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

    public void ConditionalSelectCorner()
    {
        if (cornerNode.GetComponent<NodeManager>().foldingMode)
        {
            SelectCorner();
        }
    }

    public void ConditionalUnselectCorner()
    {
        if (cornerNode.GetComponent<NodeManager>().foldingMode)
        {
            UnselectCorner();
        }
    }

    public void SelectCorner()
    {
        isCornerSelected = true;
        initialPosition = cornerNode.transform.position;
        Debug.Log("Select");

        foldSelectionButton.SetActive(false);
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

        // Reset all information for the new fold
        List<GameObject> paperSegmentsCopy = new List<GameObject>(allPaperSegments);
        animatingPaperSegments = new List<GameObject>();
        paperSegmentToBeFold = new List<GameObject>();
        previousPaperAfterFold = new List<GameObject[]>();
        previousPaperBeforeFold = new List<GameObject>();
        

        // Keep a list of gameobject of the current gameobject.
        // [DONE] TODO: the folding direction is easy to determine, but need to dynamically determine the height of the folding point
        // - It should be the uppermost layer (minimum z) that determines the foldaxis

        
        foldAxisZ = 5; // reset foldAxisZ to a very large value
        numOfPlanesToSlice = 0;

        Vector3 foldAxis = keypoints[2] - keypoints[3]; //difference of foldp1 and foldp2
        Vector3 foldPoint = keypoints[4]; //midpoint


        foreach (GameObject paperSegment in paperSegmentsCopy)
        {  
            paperSegment.GetComponent<SlicingScript>().TestSegment(keypoints[2], keypoints[3], keypoints[4]);
            // In this procedure, all paper segments under the fold will be traversed. The minimum z value will be recorded and used in animation 
        }
        //paper.GetComponent<SlicingScript>().SegmentPaper(keypoints[2], keypoints[3], keypoints[4]);
        Debug.Log("Testing Segment Finish");


        // TODO: After traversing all planes and identifying those influenced by the fold, if there is zero planes, then maybe flip entire paper
        if (numOfPlanesToSlice == 0)
        {
            // Add all planes into the animation planes list, and while traverse find the minimum z
            foreach(GameObject paperSegment in paperSegmentsCopy)
            {
                animatingPaperSegments.Add(paperSegment);

                foreach(Vector3 vertex in paperSegment.GetComponent<MeshFilter>().mesh.vertices){
                    Vector3 global = paperSegment.transform.TransformPoint(vertex);
                    if (global.z < foldAxisZ)
                    {
                        foldAxisZ = global.z;
                    }
                }
            }
            
            foldPoint.z = foldAxisZ; //change z coordinate of midpoint to the minimum z of all vertices of all segments under fold line

            foreach (GameObject animatingPaperSegment in animatingPaperSegments)
            {
                animatingPaperSegment.GetComponent<PaperSegment>().FoldingAnimation(foldPoint, foldAxis);
            }
            Debug.Log("Flipping entire paper");

            animatingPaperSegments.Clear();
        }

        else // at least some layers to fold
        {
            foldAxis = keypoints[2] - keypoints[3]; //difference of foldp1 and foldp2
            foldPoint = keypoints[4]; //midpoint
            foldPoint.z = foldAxisZ; //change z coordinate of midpoint to the minimum z of all vertices of all segments under fold line

            FOLDP1 = keypoints[2];
            FOLDP2 = keypoints[3];
            FOLDAXIS = foldAxis;
            FOLDPOSITION = foldPoint;

            Debug.Log($"Animation around point {foldPoint} and direction {foldAxis}");
            Debug.Log($"Number of planes to animate is {animatingPaperSegments.Count}");

            // TODO: if have more than one layers, fold the first layer, then ask the user if you want to fold another layer? perhaps via a button. Now system can only fold everything together

            // sort the paperSegmentToBeFold by ordering of layers from top to bottom
            paperSegmentToBeFold.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));


            //// sort the animatingPaperSegments by their ordering of layers from top to bottom
            //animatingPaperSegments.Sort((a, b) => a.transform.position.z.CompareTo(b.transform.position.z));


            foldSelectionButton.SetActive(true);
            foldLayerIndex = 0;
            // Then use the button to control folds. When clicked once, fold one paper. 





            //foreach (GameObject animatingPaperSegment in animatingPaperSegments)
            //{
            //    animatingPaperSegment.GetComponent<PaperSegment>().FoldingAnimation(foldPoint, foldAxis);
            //}
            //Debug.Log("Animation Finish");

            //animatingPaperSegments.Clear();
        }

    }

    public void NextLayer()
    {
        // Triggered when the button is clicked to fold next layer

        if (foldLayerIndex < paperSegmentToBeFold.Count)
        {

            paperSegmentToBeFold[foldLayerIndex].GetComponent<SlicingScript>().SegmentPaper(FOLDP1, FOLDP2, FOLDPOSITION);
            Debug.Log($"[DEBUG] the animatingPaperSegment list is length {animatingPaperSegments.Count}");

            //animatingPaperSegments[foldLayerIndex].SetActive(true);
            //animatingPaperSegments[foldLayerIndex].GetComponent<PaperSegment>().FoldingAnimation(FOLDPOSITION, FOLDAXIS);
            animationPart.GetComponent<PaperSegment>().FoldingAnimation(FOLDPOSITION, FOLDAXIS);
            Debug.Log($"Folded the #{foldLayerIndex + 1} layer out of the {paperSegmentToBeFold.Count} layers");

            foldLayerIndex++;
        }
        else
        {
            Debug.Log("Out of range");
            foldSelectionButton.SetActive(false);
        }
    }

    public void PreviousLayer()
    {
        if (foldLayerIndex > 0)
        {
            // Play reverse animation
            //animatingPaperSegments[foldLayerIndex-1].GetComponent<PaperSegment>().FoldingAnimation(FOLDPOSITION, FOLDAXIS, 180f);
            // animatingPaperSegments[foldLayerIndex - 1].GetComponent<PaperSegment>().SuddenFold(FOLDPOSITION, FOLDAXIS);

            GameObject previousPaper = previousPaperBeforeFold[foldLayerIndex - 1];
            GameObject[] previousAfterFold = previousPaperAfterFold[foldLayerIndex - 1];
            GameObject previousLowerHull = previousAfterFold[0];
            GameObject previousUpperHull = previousAfterFold[1];

            previousPaper.SetActive(true);
            previousLowerHull.SetActive(false);
            previousUpperHull.SetActive(false);

            allPaperSegments.Add(previousPaperBeforeFold[foldLayerIndex - 1]);
            allPaperSegments.Remove(previousPaperAfterFold[foldLayerIndex - 1][0]);
            allPaperSegments.Remove(previousPaperAfterFold[foldLayerIndex - 1][1]);

            // Since this step is reverted, remove the record of this step
            previousPaperBeforeFold.Remove(previousPaper);
            previousPaperAfterFold.Remove(previousAfterFold);
            Destroy(previousLowerHull);
            Destroy(previousUpperHull);

            Debug.Log($"Length of prevPaperBeforeFold {previousPaperBeforeFold.Count}, previousPaperAfterFold {previousPaperAfterFold.Count}");

            foldLayerIndex--;
        }
        else
        {
            Debug.Log("No Previous Fold");
        }
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


    public class Edge
    {
        public GameObject paper1;
        public GameObject paper2;
        public Vector3 point1;
        public Vector3 point2;

        public Edge(GameObject paper1, GameObject paper2, Vector3 point1, Vector3 point2)
        {
            this.paper1 = paper1;
            this.paper2 = paper2;
            this.point1 = point1;
            this.point2 = point2;

        }

    }

}
