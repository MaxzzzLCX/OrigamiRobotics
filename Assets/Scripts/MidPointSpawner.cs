using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointSpawner : MonoBehaviour
{
    public GameObject objectToMove; // Obiekt do przeniesienia, ustawiany rêcznie w Unity
    public GameObject objectToActivate; // Obiekt do aktywacji, ustawiany rêcznie w Unity

    public PaperSheetLocalizer paperSheetLocalizer;
    public float delay = 2.0f; // OpóŸnienie przed pierwszym sprawdzeniem
    public float checkInterval = 5.0f; // Interwa³ czasu, co ile sekund ma sprawdzaæ dostêpnoœæ punktów

    private bool isMoved = false; // Flaga wskazuj¹ca, czy obiekt zosta³ ju¿ przeniesiony

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

            if (points != null && points.Count > 0)
            {
                // Wyznaczenie punktu œrodkowego
                Vector3 midPoint = CalculateMidPoint(points);

                // Przesuniêcie obiektu do wyliczonego œrodka
                objectToMove.transform.position = midPoint;

                // Aktywacja wybranego obiektu
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                }

                isMoved = true; // Ustaw flagê, ¿e obiekt zosta³ przeniesiony
            }
            else
            {
                // Czekaj okreœlony interwa³ czasu przed ponownym sprawdzeniem
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
        return sum / points.Count; // Œrednia z punktów
    }
}
