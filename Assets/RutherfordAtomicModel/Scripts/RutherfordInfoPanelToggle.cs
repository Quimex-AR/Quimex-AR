using UnityEngine;

public class RutherfordInfoPanelToggle : MonoBehaviour
{
    public GameObject infoPanel;

    public void ToggleInfo()
    {
        if (infoPanel == null)
        {
            Debug.LogWarning("Panel de información no asignado en RutherfordInfoPanelToggle.");
            return;
        }

        infoPanel.SetActive(!infoPanel.activeSelf);
    }
}
