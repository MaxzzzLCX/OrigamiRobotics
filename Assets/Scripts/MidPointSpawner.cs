using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointSpawner : MonoBehaviour
{
    public GameObject objectToMove; // Obiekt do przeniesienia, ustawiany r�cznie w Unity
    public GameObject objectToActivate; // Obiekt do aktywacji, ustawiany r�cznie w Unity

    public PaperSheetLocalizer paperSheetLocalizer;
    public float delay = 2.0f; // Op�nienie przed pierwszym sprawdzeniem
    public float checkInterval = 5.0f; // Interwa� czasu, co ile sekund ma sprawdza� dost�pno�� punkt�w

    private bool isMoved = false; // Flaga wskazuj�ca, czy obiekt zosta� ju� przeniesiony

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
                // Wyznaczenie punktu �rodkowego
                Vector3 midPoint = CalculateMidPoint(points);

                // Przesuni�cie obiektu do wyliczonego �rodka
                objectToMove.transform.position = midPoint;

                // Aktywacja wybranego obiektu
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                }

                isMoved = true; // Ustaw flag�, �e obiekt zosta� przeniesiony
            }
            else
            {
                // Czekaj okre�lony interwa� czasu przed ponownym sprawdzeniem
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
        return sum / points.Count; // �rednia z punkt�w
    }
}
