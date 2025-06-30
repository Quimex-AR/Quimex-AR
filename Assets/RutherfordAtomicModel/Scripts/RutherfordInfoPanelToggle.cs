using System;
using UnityEngine;

public class RutherfordInfoPanelToggle : MonoBehaviour
{
    public GameObject infoPanel;

    public void ToggleInfo()
    {
        if (infoPanel == null)
        {
            Debug.LogWarning("Panel de informaci�n no asignado en RutherfordInfoPanelToggle.");
            return;
        }

        infoPanel.SetActive(!infoPanel.activeSelf);
    }

    public void ShowInfo()
    {
        if (infoPanel == null) return;

        infoPanel.SetActive(true);
    }


    public void HideInfo()
    {
        if (infoPanel == null) return;

        infoPanel.SetActive(false);
    }

    public void HideByTapInfo(int taps)
    {
        if (infoPanel == null) return;

        if (taps != 3)
        {
            infoPanel.SetActive(false);
        }
    }
}
