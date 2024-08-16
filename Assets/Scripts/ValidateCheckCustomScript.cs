using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ValidateCheckCustomScript : MonoBehaviour
{
    public ParentStateController parentStateController;
    public NeuralNetworkController neuralNetworkController;

    public GameObject objectToControl; // GameObject, który ma byæ kontrolowany
    public float delayInSeconds = 3f; // OpóŸnienie w sekundach

    public UnityEvent onMatch;
    public UnityEvent onMismatch;

    public void CheckParentStateCustom()
    {
        if (parentStateController == null || neuralNetworkController == null)
        {
            Debug.LogError("ParentStateController lub NeuralNetworkController nie zosta³ przypisany do ValidateCheck.");
            return;
        }

        // Rozpocznij coroutine, która kontroluje proces
        StartCoroutine(ControlObjectRoutine());
    }

    private IEnumerator ControlObjectRoutine()
    {
        if (objectToControl != null)
        {
            // W³¹cz GameObject
            objectToControl.SetActive(true);

            // Zatrzymaj wykonanie na okreœlony czas
            yield return new WaitForSeconds(delayInSeconds);

            // Wy³¹cz GameObject
            objectToControl.SetActive(false);
        }

        // Kontynuacja z pozosta³ymi czynnoœciami po zatrzymaniu GameObject
        int currentState = parentStateController.ParentStateCustom; // Zmienione na ParentStateCustom
        int predictedState = neuralNetworkController.Validate();

        Debug.Log($"Aktualny niestandardowy stan rodzica: {currentState}, Predykowany stan: {predictedState}");

        neuralNetworkController.LogMessage($"{neuralNetworkController.lastModelId}, {currentState}, {predictedState}");

        if (currentState == predictedState)
        {
            Debug.Log("Stany s¹ zgodne: wywo³anie onMatch");
            onMatch.Invoke();
        }
        else
        {
            Debug.Log("Stany siê ró¿ni¹: wywo³anie onMismatch");
            onMismatch.Invoke();
        }
    }
}
