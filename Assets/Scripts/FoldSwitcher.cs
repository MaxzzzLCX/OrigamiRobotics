using UnityEngine;

public class FoldSwitcher : MonoBehaviour
{
    public GameObject parentObject; // Assign the parent object here in the editor
    public GameObject nextArrow; // 3D object for the "Next" arrow
    public GameObject previousArrow; // 3D object for the "Previous" arrow

    private GameObject[] folds;
    public bool[] validated; // Keep record of which steps have passed validation, thus can move on. 
    public int[] failureCount; // Keep record of the number of times that the validation has failed on this step
    public int currentIndex = -1;

    public TimeLoggerManager timeLoggerManager;
    public ConfigurationManager configManager;

    void Start()
    {
        // Initialization and finding children that start with "FOLD_"
        int children = parentObject.transform.childCount;
        folds = new GameObject[children];
        validated = new bool[children];
        failureCount = new int[children];

        // Initialize the validated array depending on whether the current model is with or without DL
        Debug.Log($"DL is {configManager.DL}");

        if (configManager.DL == 1)
        {
            for (int i = 0; i < children; i++)
            {
                validated[i] = false;
                failureCount[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < children; i++)
            {
                validated[i] = true;
                failureCount[i] = 0;
            }
        }

        // DEBUG purpose
        Debug.Log("[VAL] Print validated array");
        for (int i = 0; i < children; i++)
        {
            Debug.Log($"{i}: {validated[i]}");
        }

        bool activeFoldFound = false;

        for (int i = 0; i < children; ++i)
        {
            GameObject child = parentObject.transform.GetChild(i).gameObject;
            if (child.name.StartsWith("FOLD_"))
            {
                folds[i] = child;

                // Check if this FOLD is active
                if (child.activeSelf && !activeFoldFound)
                {
                    currentIndex = i;
                    activeFoldFound = true;
                }
            }
        }

        // If no active FOLD was found, activate the first one
        if (!activeFoldFound)
        {
            currentIndex = 0;
            folds[currentIndex].SetActive(true);
        }

        UpdateArrows();
    }
    
    public void NextFold() //Everytime the next button is pressed, it triggers the time logging
    {
        folds[currentIndex].SetActive(false);
        timeLoggerManager.endOfStep(configManager.config.id, configManager.DL, configManager.modelID, currentIndex);

        currentIndex = (currentIndex + 1) % folds.Length;
        folds[currentIndex].SetActive(true);
        timeLoggerManager.startOfStep();

        UpdateArrows();
    }

    public void PreviousFold()
    {
        folds[currentIndex].SetActive(false);

        currentIndex = (currentIndex - 1 + folds.Length) % folds.Length;
        folds[currentIndex].SetActive(true);

        UpdateArrows();
    }

    private void UpdateArrows()
    {
        // Disable the "Previous" arrow when the first FOLD is active
        previousArrow.SetActive(currentIndex > 0);

        // Disable the "Next" arrow when the last FOLD is active
        // Only show next step button if the current step passes validation OR failing 3 times. 
        nextArrow.SetActive(currentIndex < folds.Length - 1 && (validated[currentIndex] || failureCount[currentIndex] > 2));
        Debug.Log($"Update arrow: validation status of current step {validated[currentIndex]}");
    }


    // New function added by Max
    // This function will start the origami from the beginning everytime it is selected.
    private void OnEnable()
    {
        // Debug.Log("Enable Origami");

    }

    public void Restart()
    {
        Debug.Log("[NEW] Origami Model Enabled - Reset to Start");

        if (folds != null && folds.Length > 0)
        {
            currentIndex = 0;
            Debug.Log("Current Index RESET to 0");
            RestartFoldingStageDisplay();
            ResetArrowValidationRecord();
            UpdateArrows();
        }
    }
    public void ResetIndex()
    {
        Debug.Log("[New] Reset Index");
        currentIndex = 0;
    }

    public void RestartFoldingStageDisplay()
    {
        Debug.Log("Restart Now");
        if (folds != null && folds.Length > 0)
        {
            for (int i = 0; i < folds.Length; ++i)
            {
                if (currentIndex == i)
                {
                    folds[i].gameObject.SetActive(true);
                }
                else
                {
                    folds[i].gameObject.SetActive(false);
                }
            }

        }
    }

    public void StartOfStep()
    {
        timeLoggerManager.startOfStep();
    }
    public void EndOfStep()
    {
        timeLoggerManager.endOfStep(configManager.config.id, configManager.DL, configManager.modelID, currentIndex);
    }

    public void UpdateArrowValidationRecord()
    {
        validated[currentIndex] = true;
        Debug.Log("[VAL] Validation passed, update validated array");
        Debug.Log($"{currentIndex}: {validated[currentIndex]}");
        UpdateArrows();
    }
    public void UpdateArrowValidationFailureCount()
    {
        failureCount[currentIndex] += 1;
        Debug.Log($"Failure. Now the failureCount is {failureCount[currentIndex]}");
        UpdateArrows();
    }

    public void ResetArrowValidationRecord()
    {
        int children = parentObject.transform.childCount;
        if (configManager.DL == 1)
        {
            for (int i = 0; i < children; i++)
            {
                validated[i] = false;
                failureCount[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < children; i++)
            {
                validated[i] = true;
                failureCount[i] = 0;
            }
        }

        // DEBUG purpose
        Debug.Log("[VAL] RESET validated array");
        for (int i = 0; i < children; i++)
        {
            Debug.Log($"{i}: {validated[i]}");
        }
    }

}
