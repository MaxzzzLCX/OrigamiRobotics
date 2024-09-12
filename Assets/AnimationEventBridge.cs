using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventBridge : MonoBehaviour
{
    public UnityEvent onAnimationEventTransition;
    
    // Start is called before the first frame update
    public void AnimationTransitionEvent()
    {
        Debug.Log("Current animation finished. Received end event.");
        onAnimationEventTransition.Invoke();
    }
}
