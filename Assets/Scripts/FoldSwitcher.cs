using UnityEngine;

public class FoldSwitcher : MonoBehaviour
{
    public GameObject parentObject; // Przypisz tutaj obiekt nadrz�dny w edytorze
    public GameObject nextArrow; // Obiekt 3D dla strza�ki "Dalej"
    public GameObject previousArrow; // Obiekt 3D dla strza�ki "Cofnij"

    private GameObject[] folds;
    private int currentIndex = -1;

    void Start()
    {
        // Inicjalizacja i znalezienie dzieci zaczynaj�cych si� od "FOLD_"
        int children = parentObject.transform.childCount;
        folds = new GameObject[children];
        bool activeFoldFound = false;

        for (int i = 0; i < children; ++i)
        {
            GameObject child = parentObject.transform.GetChild(i).gameObject;
            if (child.name.StartsWith("FOLD_"))
            {
                folds[i] = child;

                // Sprawd�, czy ten FOLD jest aktywny
                if (child.activeSelf && !activeFoldFound)
                {
                    currentIndex = i;
                    activeFoldFound = true;
                }
            }
        }

        // Je�li nie znaleziono aktywnego FOLDa, aktywuj pierwszy
        if (!activeFoldFound)
        {
            currentIndex = 0;
            folds[currentIndex].SetActive(true);
        }

        UpdateArrows();
    }

    public void NextFold()
    {
        folds[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % folds.Length;
        folds[currentIndex].SetActive(true);

        UpdateArrows();
    }

    public void PreviousFold()
    {
        folds[currentIndex].SetActive(false);

        currentIndex = (currentIndex - 1 + folds.Length) % folds.Length;
        folds[currentIndex].SetActive(true);

        UpdateArrows();
    }

    private void UpdateArrows()
    {
        // Wy��cz strza�k� "Cofnij" gdy pierwszy FOLD jest aktywny
        previousArrow.SetActive(currentIndex > 0);

        // Wy��cz strza�k� "Dalej" gdy ostatni FOLD jest aktywny
        nextArrow.SetActive(currentIndex < folds.Length - 1);
    }
}
