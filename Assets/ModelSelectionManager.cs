using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ModelSelectionManager : MonoBehaviour
{
    public ConfigurationManager configManager;
    private int modelIndex;
    private int tutorialIndex;
    private ConfigurationManager.Configuration json_config;
    private string tutorialMode;
    private string modelSequence;
    private string tutorialSequence;
    private int chosenIndex;
    private string chosenSequence;

    // Defining the events that will be triggered according to the current index of model
    // These events will determine which origami model to show in the model selection page
    public UnityEvent ModelAEvent;
    public UnityEvent ModelBEvent;
    public UnityEvent ModelCEvent;
    public UnityEvent ModelDEvent;
    public UnityEvent TutorialEvent;


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

    private void OnEnable()
    {
       
        modelIndex = configManager.modelIndex;
        tutorialIndex = configManager.tutorialIndex;
        // json_config = configManager.config;
        tutorialMode = configManager.config.tutorial_mode; // 1 means in tutorial mode; 0 means in experiment mode
        modelSequence = configManager.config.experiment_sequence;
        tutorialSequence = configManager.config.tutorial_sequence;

        chosenSequence = configManager.chosenSequence;
        chosenIndex = configManager.chosenIndex;


        // Debug.Log("ModelSelectionManager: " + modelSequence);

        char current = chosenSequence[chosenIndex];
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
                Debug.Log("Case of C. Invoke C Event");
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
            case 'N':
                Debug.Log($"Tutorial {chosenIndex + 1}");
                TutorialEvent.Invoke();
                break;
            case 'Y':
                Debug.Log($"Tutorial {chosenIndex + 1}");
                TutorialEvent.Invoke();
                break;
            default:
                Debug.Log("Default Case");
                break;
        }



        /*
         * [OLD]
        if (tutorialMode == "0")
        {
            // start of actual experiment, consisting of 8 models
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
                    Debug.Log("Case of C. Invoke C Event");
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
        else
        {
            char current = tutorialSequence[tutorialIndex];
            Debug.Log($"Tutorial {tutorialIndex + 1}");
            TutorialEvent.Invoke();
        }
        */

    }
}
