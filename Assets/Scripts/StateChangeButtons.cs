using UnityEngine;
using UnityEngine.UI;

public class StateChangeButtons : MonoBehaviour
{
    public Button state1Button; // Przycisk do zmiany na stan 1
    public Button state2Button; // Przycisk do zmiany na stan 2
    public StateManager stateManager; // Referencja do StateManager

    void Start()
    {
        // Dodanie listenerów do przycisków
        state1Button.onClick.AddListener(() => ChangeState(1));
        state2Button.onClick.AddListener(() => ChangeState(2));
    }

    void ChangeState(int newState)
    {
        if (stateManager != null)
        {
            stateManager.ChangeState(newState);
        }
        else
        {
            Debug.LogError("StateManager nie jest przypisany!");
        }
    }
}
