using UnityEngine;

public class ParentStateController : MonoBehaviour
{
    private GameObject[] children;
    private int lastActiveChildIndex = -1; // Index of the last active child

    // Variable to store the custom state of the parent
    private int parentStateCustom = 0;

    // Public property to get and set the custom state of the parent
    public int ParentStateCustom
    {
        get { return parentStateCustom; }
        set
        {
            parentStateCustom = value;
            OnParentStateCustomChanged(); // Optionally: You can call a method here if you want to handle the change.
        }
    }

    public int ParentState
    {
        get { return lastActiveChildIndex; }
    }

    private void Start()
    {
        InitializeChildrenArray();
    }

    private void Update()
    {
        CheckForActiveChild();
    }

    private void InitializeChildrenArray()
    {
        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
    }

    private void CheckForActiveChild()
    {
        int currentActiveChildIndex = -1;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].activeSelf)
            {
                currentActiveChildIndex = i;
                break; // Active child found, break the loop
            }
        }

        if (currentActiveChildIndex != lastActiveChildIndex)
        {
            lastActiveChildIndex = currentActiveChildIndex;
            UpdateParentState(currentActiveChildIndex);
        }
    }

    private void UpdateParentState(int newState)
    {
        Debug.Log("Parent state updated to: " + newState);
    }

    // Method called after changing ParentStateCustom
    private void OnParentStateCustomChanged()
    {
        Debug.Log("Custom parent state changed to: " + parentStateCustom);
        // Here you can add additional logic that should be executed after changing the value of ParentStateCustom
    }
}
