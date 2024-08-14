using UnityEngine;
using System.Collections;

public class ObjectTimerDeactivator : MonoBehaviour
{
    // Czas w sekundach, po którym obiekt zostanie wy³¹czony
    public float deactivationDelay = 3f;
    public bool followCamera = false;

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

    private void Update()
    {
        if (followCamera)
        {
            gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1f;
        }
    }
}
