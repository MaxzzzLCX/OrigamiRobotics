using UnityEngine;

public class FoldSwitcher : MonoBehaviour
{
    public GameObject parentObject; // Przypisz tutaj obiekt nadrzêdny w edytorze
    public GameObject nextArrow; // Obiekt 3D dla strza³ki "Dalej"
    public GameObject previousArrow; // Obiekt 3D dla strza³ki "Cofnij"

    private GameObject[] folds;
    private int currentIndex = -1;

    void Start()
    {
        // Inicjalizacja i znalezienie dzieci zaczynaj¹cych siê od "FOLD_"
        int children = parentObject.transform.childCount;
        folds = new GameObject[children];
        bool activeFoldFound = false;

        for (int i = 0; i < children; ++i)
        {
            GameObject child = parentObject.transform.GetChild(i).gameObject;
            if (child.name.StartsWith("FOLD_"))
            {
                folds[i] = child;

                // SprawdŸ, czy ten FOLD jest aktywny
                if (child.activeSelf && !activeFoldFound)
                {
                    currentIndex = i;
                    activeFoldFound = true;
                }
            }
        }

        // Jeœli nie znaleziono aktywnego FOLDa, aktywuj pierwszy
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
        // Wy³¹cz strza³kê "Cofnij" gdy pierwszy FOLD jest aktywny
        previousArrow.SetActive(currentIndex > 0);

        // Wy³¹cz strza³kê "Dalej" gdy ostatni FOLD jest aktywny
        nextArrow.SetActive(currentIndex < folds.Length - 1);
    }
}
