using UnityEngine;
using Lean.Touch;

public class TouchHandler : MonoBehaviour
{
    public GameManager gameManager;

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
        // 👉 Ignora toques si están sobre la UI (botones, paneles, etc.)
        if (finger.IsOverGui)
        {
            return;
        }

        // Realiza raycast desde la posición del toque
        Ray ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            gameManager.HandleTap(hit.point);
        }
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

    }
}
