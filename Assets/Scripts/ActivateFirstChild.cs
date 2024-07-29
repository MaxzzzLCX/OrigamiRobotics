using UnityEngine;

public class ActivateFirstChild : MonoBehaviour
{
    public GameObject parentObject; // Przypisz tutaj obiekt rodzica w inspektorze Unity

    // Metoda do wywo³ania eventu, np. przez przycisk
    public void ActivateOnlyFirstChild()
    {
        if (parentObject == null) return; // Zabezpieczenie na wypadek, gdyby rodzic nie by³ przypisany

        // Aktywujemy pierwsze dziecko i dezaktywujemy pozosta³e
        bool firstChildActivated = false;
        foreach (Transform child in parentObject.transform)
        {
            if (!firstChildActivated)
            {
                child.gameObject.SetActive(true);
                firstChildActivated = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
