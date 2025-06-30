using UnityEngine;
using TMPro;
// 🔁 Cambia esto: solo usa el sistema UI clásico, no necesitas UnityEngine.UIElements;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject resultPanel;
    public GameObject instructionsPanel;

    public TextMeshProUGUI resultText;
    public GameManager gameManager;

    // 🔧 NUEVO: Referencia al grupo de UI principal que se debe desactivar
    public CanvasGroup mainUIGroup;

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
        gameManager.DisableCloudCollider();

        // 🔧 Desactiva botones del panel principal
        mainUIGroup.interactable = false;
        mainUIGroup.blocksRaycasts = false;
        mainUIGroup.alpha = 0.5f; // opcional: para que se vea desactivado
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
        gameManager.RestartGame();

        // 🔧 Reactiva botones del panel principal
        mainUIGroup.interactable = true;
        mainUIGroup.blocksRaycasts = true;
        mainUIGroup.alpha = 1f;
    }

    public void ShowResult(string text)
    {
        resultText.text = text;
        resultPanel.SetActive(true);

        mainUIGroup.interactable = false;
        mainUIGroup.blocksRaycasts = false;
        mainUIGroup.alpha = 0.5f; // opcional: para que se vea desactivado
    }

    public void HideResult()
    {
        resultPanel.SetActive(false);

        mainUIGroup.interactable = true;
        mainUIGroup.blocksRaycasts = true;
        mainUIGroup.alpha = 1f;
    }

    public void OnRetryPressed()
    {
        gameManager.DisableCloudCollider();
        gameManager.RestartGame();
    }
}
