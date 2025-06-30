using UnityEngine;
using Lean.Touch;
using UnityEngine.SceneManagement;

public class TapToChangeScene : MonoBehaviour
{
    public int tapThreshold = 3;              // Número de toques necesarios
    public string sceneToLoad = "Minijuego";  // Nombre exacto de la escena a cargar

    private int tapCount = 0;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += OnFingerTap;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerTap -= OnFingerTap;
    }

    private void OnFingerTap(LeanFinger finger)
    {
        // Raycast para saber si el toque fue sobre este objeto
        Ray ray = finger.GetRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == this.transform)
            {
                tapCount++;

                if (tapCount >= tapThreshold)
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}
