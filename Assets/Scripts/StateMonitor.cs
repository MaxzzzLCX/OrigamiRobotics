using UnityEngine;
using UnityEngine.Events;

public class StateMonitor : MonoBehaviour
{
    public StateManager stateManager; // Referencja do StateManager

    // UnityEvents
    public UnityEvent onState1;
    public UnityEvent onState2;

    void Start()
    {
        if (stateManager != null)
        {
            stateManager.OnStateChanged += HandleStateChange;
        }
    }

    void HandleStateChange(int newState)
    {
        switch (newState)
        {
            case 1:
                onState1.Invoke();
                break;
            case 2:
                onState2.Invoke();
                break;
                // Dla stanu 0 nie robimy nic
        }
    }

    void OnDestroy()
    {
        if (stateManager != null)
        {
            // Odpinanie listenera
            stateManager.OnStateChanged -= HandleStateChange;
        }
    }
}
