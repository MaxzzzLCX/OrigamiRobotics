using UnityEngine;
using UnityEngine.Events;

public class ObjectAppearanceEvent : MonoBehaviour
{
    // Event wywo³ywany, gdy obiekt siê pojawi
    public UnityEvent onAppear;

    // Event wywo³ywany, gdy obiekt zniknie
    public UnityEvent onDisappear;

    // Event wywo³ywany z opóŸnieniem, gdy obiekt siê pojawi
    public UnityEvent onAppearDelay;

    // Event wywo³ywany z opóŸnieniem, gdy obiekt zniknie
    public UnityEvent onDisappearDelay;

    // OpóŸnienie w sekundach
    public float delayInSeconds = 2f;

    void OnEnable()
    {
        // Natychmiastowe wywo³anie eventu, gdy obiekt siê pojawi
        onAppear.Invoke();

        // OpóŸnione wywo³anie eventu, gdy obiekt siê pojawi
        Invoke("InvokeOnAppearDelay", delayInSeconds);
    }

    void OnDisable()
    {
        // Natychmiastowe wywo³anie eventu, gdy obiekt zniknie
        onDisappear.Invoke();

        // OpóŸnione wywo³anie eventu, gdy obiekt zniknie
        Invoke("InvokeOnDisappearDelay", delayInSeconds);
    }

    void InvokeOnAppearDelay()
    {
        onAppearDelay.Invoke();
    }

    void InvokeOnDisappearDelay()
    {
        onDisappearDelay.Invoke();
    }
}
