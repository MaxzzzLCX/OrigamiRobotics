using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointSpawner : MonoBehaviour
{
    public GameObject objectToMove; // Object to move, set manually in Unity
    public GameObject objectToActivate; // Object to activate, set manually in Unity
    public GameObject instructionsToActivate; //ADD: the confirmation message to be triggered

    public PaperSheetLocalizer paperSheetLocalizer;
    public float delay = 2.0f; // Delay before the first check
    public float checkInterval = 5.0f; // Time interval, how often to check for points availability

    private bool isMoved = false; // Flag indicating whether the object has already been moved
    void Start()
    {
        StartCoroutine(CheckAndMove());
    }

    IEnumerator CheckAndMove()
    {
        yield return new WaitForSeconds(delay);

        while (!isMoved)
        {
            List<Vector3> points = paperSheetLocalizer.DetectPapersheet();
            // DetectPapersheet() will return a set of points if a papersheet is detected. 
            // If not, it will return null

            if (points != null && points.Count > 0)
            {
                // Calculate the midpoint
                Vector3 midPoint = CalculateMidPoint(points);

                // Move the object to the calculated midpoint
                // Move the entire UI (objectToMove is the game object Set_Point, which includes all the UI components)
                objectToMove.transform.position = midPoint;

                // Activate the selected object
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                    Debug.Log("**Paper Located");
                    if (instructionsToActivate != null)
                    {
                        instructionsToActivate.SetActive(true);
                        Debug.Log("**Confirmation Message Shown");
                    }
                    else
                    {
                        Debug.Log("**Confirmation Message not triggered");
                    }
                }

                isMoved = true; // Set the flag that the object has been moved
            }
            else
            {
                // Wait for the specified time interval before checking again
                yield return new WaitForSeconds(checkInterval);
            }
        }
    }

    Vector3 CalculateMidPoint(List<Vector3> points)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 point in points)
        {
            sum += point;
        }
        return sum / points.Count; // Average of the points
    }
}
