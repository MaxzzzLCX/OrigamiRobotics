using UnityEngine;
using System.Collections;

public class ObjectTimerDeactivator : MonoBehaviour
{
    // Czas w sekundach, po którym obiekt zostanie wy³¹czony
    public float deactivationDelay = 3f;

    void OnEnable()
    {
        // Rozpoczêcie odliczania do deaktywacji
        StartCoroutine(DeactivateAfterDelay());
    }

    IEnumerator DeactivateAfterDelay()
    {
        // Czekanie przez okreœlony czas
        yield return new WaitForSeconds(deactivationDelay);

        // Wy³¹czenie obiektu
        gameObject.SetActive(false);
    }
}
