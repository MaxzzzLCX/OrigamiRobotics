using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public bool countDownDeactivate;
    public bool followCamera;
    public float duration = 5f;
    public GameObject attachTo;

    private Coroutine deactivationCoroutine;


    // Start is called before the first frame update
    void OnEnable()
    {
        if (countDownDeactivate)
        {
            if (deactivationCoroutine != null)
            {
                StopCoroutine(deactivationCoroutine); //preventing multiple coroutines in parallel
            }
            deactivationCoroutine = StartCoroutine(DeactivateAfterDuration()); // Start count-down; instructions only exist for a specific duration
        }
        else
        {
            if (attachTo != null)
            {
                gameObject.transform.position = attachTo.transform.position; // Instruction appears near the object it attaches to
            }
        }

        /*
        if (countDownDeactivate)
        {
            
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(DeactivateAfterDuration()); // Start count-down; instructions only exist for a specific duration
            }
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.transform.position = attachTo.transform.position; // Instruction appears near the object it attaches to
            }
        }
        */

    }

    IEnumerator DeactivateAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (followCamera)
        {
            if (Camera.main != null)
            {
                gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                gameObject.transform.rotation = Camera.main.transform.rotation;
            }
            else
            {
                Debug.LogWarning("Main camera not found.");
            }
        }
    }
}
