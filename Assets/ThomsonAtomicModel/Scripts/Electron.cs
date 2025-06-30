using UnityEngine;
using Lean.Touch;

public class Electron : MonoBehaviour
{
    public GameController gameController;

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
        Ray ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                gameController.OnElectronTapped(this.gameObject);
            }
        }
    }
}
