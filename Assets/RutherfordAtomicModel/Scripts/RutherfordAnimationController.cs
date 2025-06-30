using UnityEngine;

public class RutherfordAnimationController : MonoBehaviour
{
    public Animator animator;

    private bool isPaused = false;

    public void ToggleAnimationPause()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator no asignado en RutherfordAnimationController.");
            return;
        }

        isPaused = !isPaused;
        animator.speed = isPaused ? 0f : 1f;
    }

    public void PauseAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator no asignado en RutherfordAnimationController.");
            return;
        }

        animator.speed = 0f;
    }

    public void PlayAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator no asignado en RutherfordAnimationController.");
            return;
        }

        animator.speed = 1f;
    }
}
