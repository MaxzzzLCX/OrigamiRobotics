using UnityEngine;

public class StateChanger : MonoBehaviour
{
    public StateManager stateManager; // Referencja do StateManager

    // Funkcja do zmiany stanu na 1
    public void SetStateTo1()
    {
        if (stateManager != null)
        {
            stateManager.ChangeState(1);
            Debug.Log("Stan zmieniony na 1");
        }
        else
        {
            Debug.LogError("StateManager nie jest przypisany!");
        }
    }

    // Funkcja do zmiany stanu na 2
    public void SetStateTo2()
    {
        if (stateManager != null)
        {
            stateManager.ChangeState(2);
            Debug.Log("Stan zmieniony na 2");
        }
        else
        {
            Debug.LogError("StateManager nie jest przypisany!");
        }
    }
}
