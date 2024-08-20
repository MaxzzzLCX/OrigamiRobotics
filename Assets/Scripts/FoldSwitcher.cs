using UnityEngine;

public class FoldSwitcher : MonoBehaviour
{
    public GameObject parentObject; // Assign the parent object here in the editor
    public GameObject nextArrow; // 3D object for the "Next" arrow
    public GameObject previousArrow; // 3D object for the "Previous" arrow

    private GameObject[] folds;
    private int currentIndex = -1;

    public TimeLoggerManager timeLoggerManager;
    public ConfigurationManager configManager;

    void Start()
    {
        // Initialization and finding children that start with "FOLD_"
        int children = parentObject.transform.childCount;
        folds = new GameObject[children];
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
        nextArrow.SetActive(currentIndex < folds.Length - 1);
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
}
