using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class ValidateCheckCustomScript : MonoBehaviour
{
    public ParentStateController parentStateController;
    public NeuralNetworkController neuralNetworkController;

    public GameObject objectToControl; // GameObject, kt�ry ma by� kontrolowany
    public float delayInSeconds = 3f; // Op�nienie w sekundach

    public UnityEvent onMatch;
    public UnityEvent onMismatch;

    public FoldSwitcher foldSwitcher;

    public DateTime timeStartOfValidation;
    public DateTime timeEndOfValidation;
    public TimeSpan validationDuration;

    public void CheckParentStateCustom()
    {
        if (parentStateController == null || neuralNetworkController == null)
        {
            Debug.LogError("ParentStateController lub NeuralNetworkController nie zosta� przypisany do ValidateCheck.");
            return;
        }

        // Rozpocznij coroutine, kt�ra kontroluje proces
        timeStartOfValidation = DateTime.Now;
        StartCoroutine(ControlObjectRoutine());
    }

    private IEnumerator ControlObjectRoutine()
    {
        if (objectToControl != null)
        {
            // W��cz GameObject
            objectToControl.SetActive(true);

            // Zatrzymaj wykonanie na okre�lony czas
            yield return new WaitForSeconds(delayInSeconds);

            // Wy��cz GameObject
            objectToControl.SetActive(false);
        }

        // Kontynuacja z pozosta�ymi czynno�ciami po zatrzymaniu GameObject
        int currentState = parentStateController.ParentStateCustom; // Zmienione na ParentStateCustom
        int predictedState = neuralNetworkController.Validate();

        timeEndOfValidation = DateTime.Now;
        validationDuration = timeEndOfValidation - timeStartOfValidation;
        int numFailure = foldSwitcher.failureCount[foldSwitcher.currentIndex];
        Debug.Log($"Current parent state: {currentState}, Predicted state: {predictedState}, Number of Failure: {numFailure}");

        neuralNetworkController.LogMessage($"{neuralNetworkController.lastModelId}, {currentState}, {predictedState}, {numFailure}, {validationDuration}");


        if (currentState == predictedState)
        {
            Debug.Log("Stany s� zgodne: wywo�anie onMatch");
            onMatch.Invoke();
        }
        else
        {
            Debug.Log("Stany si� r�ni�: wywo�anie onMismatch");
            onMismatch.Invoke();
        }
    }
}
