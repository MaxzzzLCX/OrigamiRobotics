using UnityEngine;
using EzySlice;

public class TestSlice : MonoBehaviour
{
    public Material crossSectionMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Define the slicing plane
            Vector3 planeNormal = Vector3.up;
            Vector3 planePosition = transform.position;

            // Perform the slice
            SlicedHull slicedObject = gameObject.Slice(planePosition, planeNormal, crossSectionMaterial);

            if (slicedObject != null)
            {
                // Create upper and lower hulls
                GameObject upperHull = slicedObject.CreateUpperHull(gameObject, crossSectionMaterial);
                GameObject lowerHull = slicedObject.CreateLowerHull(gameObject, crossSectionMaterial);

                // Position the new objects
                upperHull.transform.position = transform.position;
                lowerHull.transform.position = transform.position;

                // Destroy the original object
                Destroy(gameObject);
            }
        }
    }
}
