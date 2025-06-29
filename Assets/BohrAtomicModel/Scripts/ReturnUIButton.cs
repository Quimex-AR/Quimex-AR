using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnUIButton : MonoBehaviour
{
    public string returnToScene = "Quimex AR";
    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();

        EnableButton();
    }

    public void EnableButton()
    {
        btn.interactable = true;
    }

    public void DisableButton()
    {
        btn.interactable = false;
    }

    public void ShowButton()
    {
        gameObject.SetActive(true);
    }

    public void HideButton()
    {
        gameObject.SetActive(false);
    }

    public void OnRestartClicked()
    {
        btn.interactable = false;

        if (FadeCanvas.Instance != null)
        {
            StartCoroutine(FadeCanvas.Instance.FadeToBlack());
        }

        LoadingScreenController.targetScene = returnToScene;
        SceneManager.LoadScene("Loading Scene");
    }
}
