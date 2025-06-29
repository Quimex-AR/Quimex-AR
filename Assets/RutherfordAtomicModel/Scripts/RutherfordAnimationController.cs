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
}
