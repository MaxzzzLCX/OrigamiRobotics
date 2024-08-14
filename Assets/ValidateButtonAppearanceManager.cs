using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValidateButtonAppearanceManager : MonoBehaviour
{
    public ConfigurationManager configManager;
    private int modelIndex;
    private ConfigurationManager.Configuration json_config;
    private string modelSequence;

    private void OnEnable()
    {
        modelIndex = configManager.modelIndex;
        json_config = configManager.config;
        modelSequence = json_config.experiment_sequence;
        Debug.Log("ModelSelectionManager: " + modelSequence);

        char current = modelSequence[modelIndex];

        if (current != null)
        {
            if (current ==  'E' || current == 'F' || current == 'G' || current == 'H')
            {
                Debug.Log("Current letter is " + current + " , thus hide validation button");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Current letter is " + current + " , thus show validation button");
            }

        }
        else
        {
            Debug.Log("No Valid Character");
        }
    }
}
