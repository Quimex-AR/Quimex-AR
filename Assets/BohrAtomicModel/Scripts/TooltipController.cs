// DO NOT attach this script to non-tooltip objects!
// This script is ONLY meant to be placed on Tooltip prefab instances themselves.
// It will not auto-instantiate or manage other GameObjects. See TooltipManager for coordination.

using UnityEngine;

// [ExecuteAlways]
public class TooltipController : MonoBehaviour
{
    // [Header("Tooltip Settings")]

    /// <summary>
    /// Show this tooltip manually.
    /// </summary>
    public void ShowTooltip()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide this tooltip manually.
    /// </summary>
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggle this tooltip manually.
    /// </summary>
    public void ToggleTooltip()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    /// <summary>
    /// TooltipManager compatibility — shows tooltip at a custom position.
    /// </summary>
    public void ShowAt(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
    }

    /// <summary>
    /// TooltipManager compatibility — basic show call.
    /// </summary>
    public void ShowAt()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// TooltipManager compatibility — hide call.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
