using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.speed = 0;

    }

    public void ToggleAnimation()
    {
        if (animator.speed == 0)
        {
            animator.speed = 1;
        }
        else
        {
            animator.speed = 0;
        }
    }
}
