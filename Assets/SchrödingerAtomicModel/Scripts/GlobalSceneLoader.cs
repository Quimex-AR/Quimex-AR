using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneLoader : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            LoadingScreenController.targetScene = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena en el Inspector.");
        }
    }
}