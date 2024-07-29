using UnityEngine;

public class ParentActivation : MonoBehaviour
{
    // Funkcja do aktywacji Rodzic1 i odpowiedniej manipulacji dzie�mi
    public void ActivateParentAndFirstChild()
    {
        // Aktywacja samego siebie (Rodzic1)
        gameObject.SetActive(true);

        // Sprawdzanie czy s� jakie� dzieci
        if (transform.childCount > 0)
        {
            // Aktywacja pierwszego dziecka i deaktywacja pozosta�ych
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(i == 0); // Aktywuje tylko pierwsze dziecko
            }
        }
    }
}
