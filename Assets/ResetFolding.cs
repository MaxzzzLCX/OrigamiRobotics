using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ResetFolding : MonoBehaviour
{
    public GameObject FoldingParentObject;
    private GameObject[] folds;
    private int currentIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        // Initialization and finding children that start with "FOLD_"
        int children = FoldingParentObject.transform.childCount;
        folds = new GameObject[children];
        bool activeFoldFound = false;
        currentIndex = 0;

        for (int i = 0; i < children; ++i)
        {
            GameObject child = FoldingParentObject.transform.GetChild(i).gameObject;
            if (child.name.StartsWith("FOLD_"))
            {
                folds[i] = child;
            }
        }

        // If no active FOLD was found, activate the first one
        /*
        if (!activeFoldFound)
        {
            currentIndex = 0;
            folds[currentIndex].SetActive(true);
        }
        */

    }

    // Update is called once per frame
    private void OnEnable()
    {
        Debug.Log("Origami Model Enabled - Reset to Start");

        if (folds != null && folds.Length > 0)
        {
            RestartFolding();
        }
    }

    private void RestartFolding()
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
}
