using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class SlicingScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Material crossSectionMaterial;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SegmentPaper(Vector3 point1, Vector3 point2, Vector3 midpoint)
    {
        Vector3 foldAxis = point1 - point2;
        Vector3 planeNormal = Vector3.Cross(foldAxis, new Vector3(0f, 0f, 1f)).normalized;
        Vector3 planePosition = midpoint; // A point on the fold axis

        SlicedHull slicedObject = gameObject.Slice(planePosition, planeNormal, crossSectionMaterial);

        if (slicedObject != null)
        {
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

            


            float foldingAngle = -180f; // The angle to fold, adjust as needed
            float animationDuration = 5f; // Duration of the folding animation

            StartCoroutine(AnimateFold(lowerHull, midpoint, foldAxis, foldingAngle, animationDuration));

            // Destroy the original object
            //gameObject.SetActive(false);
            // Destroy(gameObject);



        }
        else
        {
            Debug.Log("Error Slicing");
        }
    }

 

    private IEnumerator AnimateFold(GameObject foldingPart, Vector3 axisPoint, Vector3 axisDirection, float totalAngle, float duration)
    {
        float elapsed = 0f;
        float currentAngle = 0f;

        while (elapsed < duration)
        {
            float deltaAngle = (Time.deltaTime / duration) * totalAngle;
            foldingPart.transform.RotateAround(axisPoint, axisDirection, deltaAngle);

            elapsed += Time.deltaTime;
            currentAngle += deltaAngle;
            yield return null;
        }

        // Correct any residual angle due to frame time inaccuracies
        float remainingAngle = totalAngle - currentAngle;
        if (Mathf.Abs(remainingAngle) > 0.01f)
        {
            foldingPart.transform.RotateAround(axisPoint, axisDirection, remainingAngle);
        }
    }

}
