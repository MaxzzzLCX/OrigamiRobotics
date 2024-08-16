using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ValidateCheck : MonoBehaviour
{
    public ParentStateController parentStateController;
    public NeuralNetworkController neuralNetworkController;

    public GameObject objectToControl; // GameObject that is to be controlled
    public float delayInSeconds = 3f; // Delay in seconds

    public UnityEvent onMatch;
    public UnityEvent onMismatch;

    public void CheckParentState()
    {
        if (parentStateController == null || neuralNetworkController == null)
        {
            Debug.LogError("ParentStateController lub NeuralNetworkController nie zosta³ przypisany do ValidateCheck.");
            return;
        }

        // Start the coroutine that controls the process
        StartCoroutine(ControlObjectRoutine());
    }

    private IEnumerator ControlObjectRoutine()
    {
        if (objectToControl != null)
        {
            // Enable the GameObject
            objectToControl.SetActive(true);

            // Pause execution for the specified time
            yield return new WaitForSeconds(delayInSeconds);

            // Disable the GameObject
            objectToControl.SetActive(false);
        }

        // Continue with other actions after stopping the GameObject
        int currentState = parentStateController.ParentState + 1;
        int predictedState = neuralNetworkController.Validate();

        Debug.Log($"Current parent state: {currentState}, Predicted state: {predictedState}");

        neuralNetworkController.LogMessage($"{neuralNetworkController.lastModelId}, {currentState}, {predictedState}");

        if (currentState == predictedState)
        {
            Debug.Log("States match: invoking onMatch");
            onMatch.Invoke();
        }
        else
        {
            Debug.Log("States differ: invoking onMismatch");
            onMismatch.Invoke();
        }
    }
}
