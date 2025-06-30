using UnityEngine;
using Lean.Touch;
using TMPro;

public class ShowModalOnTap : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject modalPanel;                   // Panel del modal (Canvas UI)
    public TextMeshProUGUI modalText;               // Texto dentro del modal
    [TextArea] public string textToShow;            // Texto a mostrar (asignado desde el Inspector)

    [Header("UI Control")]
    public CanvasGroup mainUIGroup;                 // UI principal a desactivar mientras el modal está abierto

    private bool modalIsOpen = false;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleTap;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleTap;
    }

    private void HandleTap(LeanFinger finger)
    {
        if (modalIsOpen) return; // Evita múltiples toques si ya está abierto

        Ray ray = finger.GetRay();
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == this.transform && modalPanel != null)
            {
                OpenModal();
            }
        }
    }

    private void OpenModal()
    {
        modalPanel.SetActive(true);
        if (modalText != null)
        {
            modalText.text = textToShow;
        }

        if (mainUIGroup != null)
        {
            mainUIGroup.interactable = false;
            mainUIGroup.blocksRaycasts = false;
            mainUIGroup.alpha = 0.5f;
        }

        modalIsOpen = true;
    }

    public void CloseModal()
    {
        modalPanel.SetActive(false);

        if (mainUIGroup != null)
        {
            mainUIGroup.interactable = true;
            mainUIGroup.blocksRaycasts = true;
            mainUIGroup.alpha = 1f;
        }

        modalIsOpen = false;
    }
}
