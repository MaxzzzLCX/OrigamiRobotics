using UnityEngine;

public class ResetAnimationScript : MonoBehaviour
{
    // Publiczna metoda do resetowania animacji
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

    // Prywatna metoda do resetowania animacji i prze³¹czania stanu na konkretnym GameObject
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

        // Sprawdza, czy obiekt ma przypisany skrypt AnimationController i wywo³uje ToggleAnimation
        AnimationController animationController = obj.GetComponent<AnimationController>();
        if (animationController != null)
        {
            animationController.ToggleAnimation();
        }
    }

    // Prywatna metoda rekurencyjna do przeszukiwania i resetowania animacji
    private void ResetAnimationsRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ResetAndToggleAnimationOnGameObject(child.gameObject);
            ResetAnimationsRecursive(child);
        }
    }
}
