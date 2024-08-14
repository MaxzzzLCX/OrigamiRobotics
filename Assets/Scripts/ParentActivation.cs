using UnityEngine;

public class ParentActivation : MonoBehaviour
{
    // Function to activate Parent1 and appropriately manipulate the children
    public void ActivateParentAndFirstChild()
    {
        // Activate itself (Parent1)
        gameObject.SetActive(true);

        // Check if there are any children
        if (transform.childCount > 0)
        {
            // Activate the first child and deactivate the others
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(i == 0); // Activates only the first child
            }
        }
    }
}
