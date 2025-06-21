using System.Collections;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages actions triggered by a specific number of taps on the selectable object.
/// Intended to be hooked into LeanTouch's On Count event.
/// </summary>
public class TapManager : MonoBehaviour
{
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    public LeanSelectable selectableObject;

    [Header("Tap Settings")]
    [Tooltip("Time window (in seconds) to wait for more taps before triggering the event.")]
    [SerializeField] private float tapDelay = LeanTouch.CurrentTapThreshold * 1.25f;

    [Header("Specific Tap Events")]
    [SerializeField] private UnityEvent onSingleTap;
    [SerializeField] private UnityEvent onDoubleTap;
    [SerializeField] private UnityEvent onTripleTap;

    [Header("General Tap Event")]
    [Tooltip("This will be called with the tap count as a parameter.")]
    [SerializeField] private IntEvent onTap;

    private int _pendingTabCount = 0;
    private Coroutine _tabRoutine;

    /// <summary>
    /// Called by Lean Touch's On Count event when the object is tapped.
    /// </summary>
    /// <param name="count">The number of consecutive taps detected.</param>
    public void HandleTapCount(int count)
    {
        _pendingTabCount = count;

        if (_tabRoutine != null)
        {
            StopCoroutine(_tabRoutine);
        }

        _tabRoutine = StartCoroutine(WaitThenProcessTap());
    }

    /// <summary>
    /// Waits for a specified delay before processing the tap count.
    /// This ensures that consecutive taps within the delay window are grouped together.
    /// After the delay, it invokes the appropriate Unity events based on the tap count.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    public IEnumerator WaitThenProcessTap()
    {
        yield return new WaitForSeconds(tapDelay);

        if (selectableObject != null && selectableObject.IsSelected)
        {
            Debug.Log($"Tap count on {gameObject.name}: {_pendingTabCount}");


            switch (_pendingTabCount)
            {
                case 1: onSingleTap?.Invoke(); break;
                case 2: onDoubleTap?.Invoke(); break;
                case 3: onTripleTap?.Invoke(); break;
            }

            onTap?.Invoke(_pendingTabCount);

            _tabRoutine = null;
            _pendingTabCount = 0;
        }
    }

    /// <summary>
    /// Allows scripts to subscribe to the general tap event at runtime. CODE-BASED
    /// </summary>
    public void AddListener(UnityAction<int> callback) => onTap.AddListener(callback);
    public void RemoveListener(UnityAction<int> callback) => onTap.RemoveListener(callback);
}
