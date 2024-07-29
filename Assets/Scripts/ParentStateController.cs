using UnityEngine;

public class ParentStateController : MonoBehaviour
{
    private GameObject[] children;
    private int lastActiveChildIndex = -1; // Indeks ostatnio aktywnego dziecka

    // Zmienna do przechowywania niestandardowego stanu rodzica
    private int parentStateCustom = 0;

    // Publiczna w³aœciwoœæ do pobierania i ustawiania niestandardowego stanu rodzica
    public int ParentStateCustom
    {
        get { return parentStateCustom; }
        set
        {
            parentStateCustom = value;
            OnParentStateCustomChanged(); // Opcjonalnie: Mo¿esz wywo³aæ tutaj metodê, jeœli chcesz obs³u¿yæ zmianê.
        }
    }

    public int ParentState
    {
        get { return lastActiveChildIndex; }
    }

    private void Start()
    {
        InitializeChildrenArray();
    }

    private void Update()
    {
        CheckForActiveChild();
    }

    private void InitializeChildrenArray()
    {
        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
    }

    private void CheckForActiveChild()
    {
        int currentActiveChildIndex = -1;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].activeSelf)
            {
                currentActiveChildIndex = i;
                break; // Znaleziono aktywne dziecko, przerywamy pêtlê
            }
        }

        if (currentActiveChildIndex != lastActiveChildIndex)
        {
            lastActiveChildIndex = currentActiveChildIndex;
            UpdateParentState(currentActiveChildIndex);
        }
    }

    private void UpdateParentState(int newState)
    {
        Debug.Log("Stan rodzica zaktualizowany na: " + newState);
    }

    // Metoda wywo³ywana po zmianie ParentStateCustom
    private void OnParentStateCustomChanged()
    {
        Debug.Log("Niestandardowy stan rodzica zmieniony na: " + parentStateCustom);
        // Tutaj mo¿esz dodaæ dodatkow¹ logikê, która ma siê wykonaæ po zmianie wartoœci ParentStateCustom
    }
}
