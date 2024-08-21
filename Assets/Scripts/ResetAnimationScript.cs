using UnityEngine;

public class ResetAnimationScript : MonoBehaviour
{
    // Public method to reset animations
    public void ResetAnimations()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ResetAndToggleAnimationOnGameObject(child.gameObject);
                ResetAnimationsRecursive(child);
            }
        }
    }

    // Private method to reset animations and toggle state on a specific GameObject
    private void ResetAndToggleAnimationOnGameObject(GameObject obj)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        Animation animation = obj.GetComponent<Animation>();
        if (animation != null)
        {
            animation.Stop();
            animation.Play();
        }

        // Checks if the object has an assigned AnimationController script and invokes ToggleAnimation
        AnimationController animationController = obj.GetComponent<AnimationController>();
        if (animationController != null)
        {
            animationController.ToggleAnimation();
        }
    }

    // Private recursive method to traverse and reset animations
    private void ResetAnimationsRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ResetAndToggleAnimationOnGameObject(child.gameObject);
            ResetAnimationsRecursive(child);
        }
    }
}
