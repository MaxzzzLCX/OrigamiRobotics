using UnityEngine;
using System.Collections;

public class ObjectTimerDeactivator : MonoBehaviour
{
    // Czas w sekundach, po kt�rym obiekt zostanie wy��czony
    public float deactivationDelay = 3f;
    public bool followCamera = false;

    void OnEnable()
    {
        // Rozpocz�cie odliczania do deaktywacji
        StartCoroutine(DeactivateAfterDelay());
    }

    IEnumerator DeactivateAfterDelay()
    {
        // Czekanie przez okre�lony czas
        yield return new WaitForSeconds(deactivationDelay);

        // Wy��czenie obiektu
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
