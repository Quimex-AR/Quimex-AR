using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BohrARManager : MonoBehaviour
{
    public static BohrARManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void HandleTaps(int taps)
    {
        switch (taps)
        {
            case 1:
                ToastManager.Instance.ShowToast("Apagando Atomo", 0.25f);
                break;
            case 2:
                ToastManager.Instance.ShowToast("Partes del Atomo", 0.25f);
                break;
            case 3:
                ToastManager.Instance.ShowToast("Informacion acerca del atomo", 0.3f);
                break;
            default:
                if (taps >= 6)
                {
                    ToastManager.Instance.ShowToast("Iniciando Juego", 1f);
                    StartCoroutine(StartGame("Bohr Game"));
                }
                break;
        }

    }

    private IEnumerator StartGame(string sceneName)
    {
        yield return new WaitForSeconds(1.5f);


        if (FadeCanvas.Instance != null)
        {
            StartCoroutine(FadeCanvas.Instance.FadeToBlack());
        }

        LoadingScreenController.targetScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }
}
