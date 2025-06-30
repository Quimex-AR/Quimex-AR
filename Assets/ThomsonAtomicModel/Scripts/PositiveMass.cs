using UnityEngine;
using Lean.Touch;

public class PositiveMass : MonoBehaviour
{
    public GameController gameController;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleTap;
    }

    private void HandleTap(LeanFinger finger)
    {
        Ray ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                gameController.OnWrongTap();
            }
        }
    }
}
