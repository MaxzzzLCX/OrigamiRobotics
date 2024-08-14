using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ModelSelectionManager : MonoBehaviour
{
    public ConfigurationManager configManager;
    private int modelIndex;
    private ConfigurationManager.Configuration json_config;
    private string modelSequence;

    // Defining the events that will be triggered according to the current index of model
    // These events will determine which origami model to show in the model selection page
    public UnityEvent ModelAEvent;
    public UnityEvent ModelBEvent;
    public UnityEvent ModelCEvent;
    public UnityEvent ModelDEvent;


    // Start is called before the first frame update
    void Start()
    {
        // Connecting with ConfigurationManager (reads config info from json files)
        // modelSequence here is a string of 8 letters e.g. "ABCDEFGH" that represents the order of the experiment

        /*
        configManager = GetComponent<ConfigurationManager>();
        modelIndex = configManager.modelIndex;
        json_config = configManager.config;
        modelSequence = json_config.experiment_sequence;
        Debug.Log("ModelSelectionManager: " + modelSequence);
        */
    }

    // Update is called once per frame

    void OnEnable()
    {
        /*
        if (configManager == null)
        {
            Debug.LogError("ConfigurationManager not found on the same GameObject.");
            return;
        }
        */
        modelIndex = configManager.modelIndex;
        json_config = configManager.config;
        /*
        if (json_config == null)
        {
            Debug.LogError("json_config is null. Ensure ConfigurationManager has successfully loaded the configuration.");
            return;
        }
        */
        modelSequence = json_config.experiment_sequence;
        /*
        if (string.IsNullOrEmpty(modelSequence))
        {
            Debug.LogError("modelSequence is null or empty.");
            return;
        }
        */

        Debug.Log("ModelSelectionManager: " + modelSequence);

        char current = modelSequence[modelIndex];

        switch (current)
        {
            case 'A':
                Debug.Log("Case of A. Invoke A Event");
                if (ModelAEvent != null)
                {
                    ModelAEvent.Invoke();
                }
                break;
            case 'B':
                Debug.Log("Case of B. Invoke B Event");
                if (ModelBEvent != null)
                {
                    ModelBEvent.Invoke();
                }
                break;
            case 'C':
                Debug.Log("Case of C. Invoke A Event");
                if (ModelCEvent != null)
                {
                    ModelCEvent.Invoke();
                }
                break;
            case 'D':
                Debug.Log("Case of D. Invoke D Event");
                if (ModelDEvent != null)
                {
                    ModelDEvent.Invoke();
                }
                break;
            case 'E':
                Debug.Log("Case of E. Invoke A Event");
                if (ModelAEvent != null)
                {
                    ModelAEvent.Invoke();
                }
                break;
            case 'F':
                Debug.Log("Case of F. Invoke B Event");
                if (ModelBEvent != null)
                {
                    ModelBEvent.Invoke();
                }
                break;
            case 'G':
                Debug.Log("Case of G. Invoke C Event");
                if (ModelCEvent != null)
                {
                    ModelCEvent.Invoke();
                }
                break;
            case 'H':
                Debug.Log("Case of H. Invoke D Event");
                if (ModelDEvent != null)
                {
                    ModelDEvent.Invoke();
                }
                break;
            default:
                Debug.Log("Default Case");
                break;
        }
    }
}
