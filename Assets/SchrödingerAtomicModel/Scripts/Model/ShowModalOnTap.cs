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

    [Header("Additional Object")]
    public GameObject objectToHide;                 // Objeto a ocultar/mostrar al abrir/cerrar el modal

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
        if (modalIsOpen) return;

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

        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
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

        if (objectToHide != null)
        {
            objectToHide.SetActive(true);
        }

        modalIsOpen = false;
    }
}
