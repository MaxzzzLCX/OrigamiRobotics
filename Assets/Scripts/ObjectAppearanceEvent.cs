using UnityEngine;
using UnityEngine.Events;

public class ObjectAppearanceEvent : MonoBehaviour
{
    // Event wywo�ywany, gdy obiekt si� pojawi
    public UnityEvent onAppear;

    // Event wywo�ywany, gdy obiekt zniknie
    public UnityEvent onDisappear;

    // Event wywo�ywany z op�nieniem, gdy obiekt si� pojawi
    public UnityEvent onAppearDelay;

    // Event wywo�ywany z op�nieniem, gdy obiekt zniknie
    public UnityEvent onDisappearDelay;

    // Op�nienie w sekundach
    public float delayInSeconds = 2f;

    void OnEnable()
    {
        // Natychmiastowe wywo�anie eventu, gdy obiekt si� pojawi
        onAppear.Invoke();

        // Op�nione wywo�anie eventu, gdy obiekt si� pojawi
        Invoke("InvokeOnAppearDelay", delayInSeconds);
    }

    void OnDisable()
    {
        // Natychmiastowe wywo�anie eventu, gdy obiekt zniknie
        onDisappear.Invoke();

        // Op�nione wywo�anie eventu, gdy obiekt zniknie
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
