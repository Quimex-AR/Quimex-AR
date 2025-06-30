using Lean.Touch;
using UnityEngine;

public class RotationTouchHandler : MonoBehaviour
{
    private AutoRotate autoRotate;

    void Start()
    {
        autoRotate = GetComponent<AutoRotate>();
    }

    void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        if (autoRotate != null)
            autoRotate.isTouching = true;
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        if (autoRotate != null)
            autoRotate.isTouching = false;
    }
}
