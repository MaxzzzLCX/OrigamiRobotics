using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class SlicingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public SlicingManager slicingManager;

    public Material crossSectionMaterial;


    void Start()
    {
        slicingManager = gameObject.transform.parent.GetComponent<SlicingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool IsUnderFold(Vector3 point1, Vector3 point2, Vector3 midpoint)
    {
        Vector3 foldAxis = point1 - point2;
        Vector3 planeNormal = Vector3.Cross(foldAxis, new Vector3(0f, 0f, 1f)).normalized;
        Vector3 planePosition = midpoint; // A point on the fold axis
        SlicedHull slicedObject = gameObject.Slice(planePosition, planeNormal, crossSectionMaterial);

        if (slicedObject != null )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SegmentPaper(Vector3 point1, Vector3 point2, Vector3 midpoint)
    {
        Vector3 foldAxis = point1 - point2;
        Vector3 planeNormal = Vector3.Cross(foldAxis, new Vector3(0f, 0f, 1f)).normalized;
        Vector3 planePosition = midpoint; // A point on the fold axis

        SlicedHull slicedObject = gameObject.Slice(planePosition, planeNormal, crossSectionMaterial);

        if (slicedObject != null)
        {
            // If under the fold axis, find the minimum z (the height of the axis)
            foreach (Vector3 vertex in gameObject.GetComponent<MeshFilter>().mesh.vertices)
            {
                Vector3 global = gameObject.transform.TransformPoint(vertex);
                if (global.z < slicingManager.foldAxisZ)
                {
                    slicingManager.foldAxisZ = global.z;
                    Debug.Log($"Fold Axis z updated to {global.z}");
                }
            }

            // slicingManager.numOfPlanesToSlice++; // record the number of planes influenced

            // Create upper and lower hulls
            GameObject upperHull = slicedObject.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHull = slicedObject.CreateLowerHull(gameObject, crossSectionMaterial);

            // Position the new objects
            upperHull.transform.SetParent(transform.parent);
            lowerHull.transform.SetParent(transform.parent);
            upperHull.transform.position = transform.position;
            lowerHull.transform.position = transform.position;
            upperHull.transform.localScale = transform.localScale;
            lowerHull.transform.localScale = transform.localScale;

            upperHull.AddComponent<PaperSegment>();
            lowerHull.AddComponent<PaperSegment>();
            upperHull.AddComponent<SlicingScript>();
            lowerHull.AddComponent<SlicingScript>();

            

            // slicingManager.animatingPaperSegments.Add(lowerHull); //Put into the list of all parts that animations will be played on
            slicingManager.animationPart = lowerHull;


            //// Optionally, start the animation
            //lowerHull.GetComponent<PaperSegment>().FoldingAnimation(midpoint, foldAxis);

            slicingManager.allPaperSegments.Add(upperHull);
            slicingManager.allPaperSegments.Add(lowerHull);
            slicingManager.allPaperSegments.Remove(gameObject);

            // Keeping a record of the folding steps implemented. This can be reverted using the back button
            slicingManager.previousPaperBeforeFold.Add(gameObject);
            slicingManager.previousPaperAfterFold.Add(new GameObject[] { lowerHull, upperHull});
            


            // Destroy the original object
            gameObject.SetActive(false);
            // Destroy(gameObject);

            


        }
        else
        {
            Debug.Log("Error Slicing");
        }
    }

    public void TestSegment(Vector3 point1, Vector3 point2, Vector3 midpoint) //This function traverses all paper segments and calculate the number of paper that will be cut. However doesn't actually segment the paper
    {
        Vector3 foldAxis = point1 - point2;
        Vector3 planeNormal = Vector3.Cross(foldAxis, new Vector3(0f, 0f, 1f)).normalized;
        Vector3 planePosition = midpoint; // A point on the fold axis

        SlicedHull slicedObject = gameObject.Slice(planePosition, planeNormal, crossSectionMaterial);

        if (slicedObject != null)
        {
            // If under the fold axis, find the minimum z (the height of the axis)
            foreach (Vector3 vertex in gameObject.GetComponent<MeshFilter>().mesh.vertices)
            {
                Vector3 global = gameObject.transform.TransformPoint(vertex);
                if (global.z < slicingManager.foldAxisZ)
                {
                    slicingManager.foldAxisZ = global.z;
                    Debug.Log($"Fold Axis z updated to {global.z}");
                }
            }

            slicingManager.numOfPlanesToSlice++; // record the number of planes influenced
            
            slicingManager.paperSegmentToBeFold.Add(gameObject);


            /*
            // Create upper and lower hulls
            GameObject upperHull = slicedObject.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHull = slicedObject.CreateLowerHull(gameObject, crossSectionMaterial);

            // Position the new objects
            upperHull.transform.SetParent(transform.parent);
            lowerHull.transform.SetParent(transform.parent);
            upperHull.transform.position = transform.position;
            lowerHull.transform.position = transform.position;
            upperHull.transform.localScale = transform.localScale;
            lowerHull.transform.localScale = transform.localScale;

            upperHull.AddComponent<PaperSegment>();
            lowerHull.AddComponent<PaperSegment>();
            upperHull.AddComponent<SlicingScript>();
            lowerHull.AddComponent<SlicingScript>();



            slicingManager.animatingPaperSegments.Add(lowerHull); //Put into the list of all parts that animations will be played on


            //// Optionally, start the animation
            //lowerHull.GetComponent<PaperSegment>().FoldingAnimation(midpoint, foldAxis);

            slicingManager.allPaperSegments.Add(upperHull);
            slicingManager.allPaperSegments.Add(lowerHull);
            slicingManager.allPaperSegments.Remove(gameObject);


            // Destroy the original object
            gameObject.SetActive(false);
            // Destroy(gameObject);


            */

        }
        else
        {
            Debug.Log("Error Slicing");
        }
    }
}
