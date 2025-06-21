using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Tooltip("Manually managed list of tooltips that will be shown")]
    public List<TooltipController> tooltipControllers = new();


    [Tooltip("Whether all tooltips should be visible at the start")]
    public bool showTooltipsAtStart = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Apply initial tooltip visibility state
        if (showTooltipsAtStart)
        {
            ShowAllTooltips();
        }
        else
        {
            HideAllTooltips();
        }
    }

    /// <summary>
    /// Show all registered tooltips around the selected object.
    /// </summary>
    public void ShowTooltipsFor(GameObject selectedObject)
    {
        Vector3 basePosition = selectedObject.transform.position;
        float angleStep = 360f / tooltipControllers.Count;
        float radius = 0.1f;

        for (int i = 0; i < tooltipControllers.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0.2f, Mathf.Sin(angle)) * radius;
            tooltipControllers[i].ShowAt(basePosition + offset);
        }
    }

    /// <summary>
    /// Show all registered tooltips.
    /// </summary>
    public void ShowAllTooltips()
    {
        foreach (var tooltip in tooltipControllers)
        {
            tooltip.ShowAt();
        }
    }

    /// <summary>
    /// Hide all registered tooltips.
    /// </summary>
    public void HideAllTooltips()
    {
        foreach (var tooltip in tooltipControllers)
        {
            tooltip.Hide();
        }
    }
}
