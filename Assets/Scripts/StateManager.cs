using UnityEngine;
using System; // Dla EventArgs

public class StateManager : MonoBehaviour
{
    private int currentState = 0;

    // Definiowanie zdarzenia dla zmiany stanu
    public event Action<int> OnStateChanged;

    public void ChangeState(int newState)
    {
        if (newState >= 0 && newState <= 2)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState); // Wywołanie zdarzenia
            Debug.Log("Stan zmieniony na: " + currentState);
        }
        else
        {
            Debug.LogError("Nieprawidłowy stan: " + newState);
        }
    }

    // Metoda do pobierania aktualnego stanu
    public int GetCurrentState()
    {
        return currentState;
    }
}
