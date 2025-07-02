using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneLoader : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (FadeCanvas.Instance != null)
            {
                StartCoroutine(FadeCanvas.Instance.FadeToBlack());
            }

            LoadingScreenController.targetScene = sceneName;
            SceneManager.LoadScene("Loading Scene");
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena en el Inspector.");
        }
    }
}
