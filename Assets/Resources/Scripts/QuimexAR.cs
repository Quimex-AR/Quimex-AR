using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuimexAR : MonoBehaviour
{
    void Start()
    {
        if (FadeCanvas.Instance != null)
        {
            StartCoroutine(FadeCanvas.Instance.FadeAndHide());
        }
    }

    public void BohrGame(int taps)
    {
        if (taps >= 6)
        {
            ToastManager.Instance.ShowToast("Iniciando Juego", 1f);
            StartCoroutine(StartBohrGame());
        }
    }

    private IEnumerator StartBohrGame()
    {
        yield return new WaitForSeconds(1.5f);


        if (FadeCanvas.Instance != null)
        {
            StartCoroutine(FadeCanvas.Instance.FadeToBlack());
        }

        LoadingScreenController.targetScene = "Bohr Game";
        SceneManager.LoadScene("Loading Scene");

    }
}
