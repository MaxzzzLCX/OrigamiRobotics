using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerNew : MonoBehaviour
{

    public Animator fold1Animator;
    public Animator fold2Animator;
    public GameObject stage1;
    public GameObject stage2;

    private string startIndex;
    private string endIndex;
    private string nameAnim1;
    private string nameAnim2;

    private Vector3 initialPosition1;
    private Quaternion initialRotation1;
    private Vector3 initialScale1;
    private Vector3 initialPosition2;
    private Quaternion initialRotation2;
    private Vector3 initialScale2;

    void Start()
    {
        // Assign the Animators and GameObjects (drag and drop in Inspector or find them via script)
        //fold1Animator = fold1.GetComponent<Animator>();
        //fold2Animator = fold2.GetComponent<Animator>();

        stage1.SetActive(true);
        stage2.SetActive(false);  // Fold 2 starts inactive

        startIndex = RetrieveStageInfo(stage1);
        endIndex = RetrieveStageInfo(stage2);

        nameAnim1 = "FOLD" + startIndex;
        nameAnim2 = "FOLD" + endIndex;

    }

    public void PlayFirstAnimation()
    {
        Debug.Log("**Fold first animation");
        //fold1Animator.SetBool("OnDemonstration", true);
        fold1Animator.Play(nameAnim1, 0, 0f);
    }
    public void TransitionToNextStage()
    {
        stage1.SetActive(false);
        stage2.SetActive(true);
        Debug.Log("**Transition to next");
        //fold2Animator.SetBool("OnDemonstration", true);
        fold2Animator.Play(nameAnim2, 0, 0f);
    }

    public void ResetAnimation()
    {
        // The few lines below tries to revert the first gameobject to initial state whenever hover exits
        // Before rebind, enter the folding state.
        // If rebinding in idle state will not revert object to initial unfolded state


        stage1.SetActive(true);
        fold1Animator.Play(nameAnim1, 0, 0f);

        StartCoroutine(DelayRebindWithDeactivate());

       
        //fold1Animator.Rebind();
        //fold1Animator.Update(0); // Forces the animator to apply the new state immediately

        // stage 2 doesn't have to be reset because it will be hided
        // the next time hover is on, it will play the animation, and second animation will automatically be played from beginning when first animation finishes
        fold2Animator.Rebind();
        fold2Animator.Update(0);
        stage1.SetActive(true);
        stage2.SetActive(false);
        
    }

    private IEnumerator DelayRebindWithDeactivate()
    {
        // Wait one frame
        yield return null;

        // Forcefully reset fold1 by temporarily deactivating it
        stage1.SetActive(false); // Disable stage1 to reset its state
        yield return null;  // Wait one more frame to ensure the deactivation happens

        // Re-enable and reset the animator
        stage1.SetActive(true);
        fold1Animator.Rebind();  // Reset the animator to its default state
        fold1Animator.Update(0);

        Debug.Log("Animator has been successfully reset.");
    }

    public string RetrieveStageInfo(GameObject stage)
    {
        // Get the name of the GameObject (e.g., "STAGE_1" or "STAGE_13")
        string gameObjectName = stage.name;

        // Split the string based on the underscore
        string[] splitName = gameObjectName.Split('_');

        // Check if the split result contains the part after the underscore
        if (splitName.Length > 1)
        {
            string stageNumber = splitName[1]; // This will be "1" or "13", etc.
            Debug.Log("Stage Number: " + stageNumber);
            return stageNumber;
        }
        return "";
    }

    /*
     * Code used on Sept 10th
    private void StoreInitialState()
    {
        initialPosition1 = stage1.transform.position;
        initialRotation1 = stage1.transform.rotation;
        initialScale1 = stage1.transform.localScale;
        initialPosition2 = stage2.transform.position;
        initialRotation2 = stage2.transform.rotation;
        initialScale2 = stage2.transform.localScale;
    }

    
    private void RevertInitialState()
    {
        stage1.transform.position = initialPosition1;
        stage1.transform.rotation = initialRotation1;
        stage1.transform.localScale = initialScale1;
        stage2.transform.position = initialPosition2;
        stage2.transform.rotation = initialRotation2;
        stage2.transform.localScale = initialScale2;
    }

    // Prase the name of the gameobjects - retrieves their index - used to find the corresponding name of the animations
    


    public void FoldTwoSteps()
    {
        Debug.Log($"FOLDING {nameAnim1}");

        fold1Animator.SetBool("OnDemonstration", true);
        fold1Animator.Play(nameAnim1, 0, 0f);

        // Wait for one frame to allow the animator to update to the correct state
        StartCoroutine(WaitForFold1ToFinish());
    }

    IEnumerator WaitForFold1ToFinish()
    {
        int animStateHash = Animator.StringToHash(nameAnim1);  // Get hash of the animation state

        // Wait until the animator transitions to the correct animation state
        while (!fold1Animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(animStateHash))
        {
            AnimatorStateInfo currentState = fold1Animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"Current state hash: {currentState.shortNameHash}, Expected: {animStateHash}");
            yield return null;  // Wait until we are in the correct animation state
        }

        // Now the animator should be in the correct state
        float animationLength = fold1Animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log($"wait for stage {nameAnim1} to finish: {animationLength}");

        // Start the transition coroutine after getting the correct animation length
        StartCoroutine(HandleFoldTransition(animationLength + 0.1f, nameAnim2));
    }

    IEnumerator HandleFoldTransition(float waitTime, string nameAnim2)
    {
        yield return new WaitForSeconds(waitTime); // Wait for Fold 1 to finish

        // Hide Fold 1, show Fold 2, and play its animation
        stage1.SetActive(false);
        stage2.SetActive(true);
        Debug.Log("Switched to next stage object");
        fold2Animator.SetBool("OnDemonstration", true);
        fold2Animator.Play(nameAnim2, 0, 0f);
    }

    public void ResetDemosntration()
    {
        Debug.Log("Reset folding on hover exit");
        fold1Animator.SetBool("OnDemonstration", false);
        fold2Animator.SetBool("OnDemonstration", false);
        fold1Animator.Rebind();
        fold1Animator.Update(0);  // Ensure the animator updates immediately
        fold2Animator.Rebind();
        fold2Animator.Update(0);  // Ensure the animator updates immediately

        RevertInitialState();
    }
    */

    /*
     public Animator animator;

    //private void Awake()
    //{
    //    animator = GetComponent<Animator>();
    //}

    public void StartDemonstration()
    {
        animator.SetBool("OnDemonstration", true);
    }
    public void EndDemonstration()
    {
        animator.SetBool("OnDemonstration", false);
    }
    */
}
