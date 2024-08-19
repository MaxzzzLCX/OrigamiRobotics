using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValidateButtonAppearanceManager : MonoBehaviour
{
    public ConfigurationManager configManager;
    private int modelIndex;
    private int tutorialIndex;
    private ConfigurationManager.Configuration json_config;
    private string modelSequence;
    private string tutorialSequence;
    private string tutorialMode;

    private void OnEnable()
    {
        modelIndex = configManager.modelIndex;
        tutorialIndex = configManager.tutorialIndex;
        tutorialMode = configManager.config.tutorial_mode;
        // json_config = configManager.config;
        modelSequence = configManager.config.experiment_sequence;
        tutorialSequence = configManager.config.tutorial_sequence;
       
        if (tutorialMode == "0")
        {
            Debug.Log("NOW IS EXPERIMENT");
            char current = modelSequence[modelIndex];

            if (current != null)
            {
                if (current == 'E' || current == 'F' || current == 'G' || current == 'H')
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
        else
        {
            Debug.Log("NOW IS TUTORIAL");
            char current = tutorialSequence[tutorialIndex];

            if (current != null)
            {
                if (current == '0')
                {
                    Debug.Log("Current number is " + current + " , thus hide validation button");
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
}
