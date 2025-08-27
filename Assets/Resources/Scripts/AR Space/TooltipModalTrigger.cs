using UnityEngine;
using Lean.Touch;

/// <summary>
/// Attach to each tooltip collider (box). Stores title + body and notifies TooltipManager on tap.
/// </summary>
[AddComponentMenu("AR/Tooltip Trigger")]
public class TooltipModalTrigger : MonoBehaviour
{
    [Header("Tooltip Content")]
    [Tooltip("Title shown in the modal")]
    public string title;

    [TextArea(3, 10)]
    [Tooltip("Body text shown in the modal")]
    public string body;

    [Header("Raycast Settings")]
    [Tooltip("LayerMask to use for raycast. Set to the layer(s) your tooltip colliders are on.")]
    public LayerMask raycastMask = ~0;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    private void HandleFingerTap(LeanFinger finger)
    {
        // Ignore taps that started over UI (so modal buttons don't re-open)
        if (finger.IsOverGui) return;

        var ray = finger.GetRay();
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask))
        {
            // Accept hits on this transform or any child
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                TooltipModalManager.Instance?.ShowTooltip(this);
            }
        }
    }
}
