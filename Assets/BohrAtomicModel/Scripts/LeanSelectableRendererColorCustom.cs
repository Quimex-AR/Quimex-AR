using Lean.Common;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LeanSelectableRendererColorCustom : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material selectedMaterial;

    private Renderer cachedRenderer;

    void Awake()
    {

        cachedRenderer = GetComponent<Renderer>();
        SetDefaultMaterial();
    }

    /// <summary>
    /// Call this to apply the default material.
    /// </summary>
    public void UpdateDefaultColor()
    {
        SetDefaultMaterial();
    }

    /// <summary>
    /// Call this to apply the selected material.
    /// </summary>
    public void UpdateSelectedColor()
    {
        SetSelectedMaterial();
    }

    private void SetDefaultMaterial()
    {
        if (cachedRenderer != null && defaultMaterial != null)
        {
            cachedRenderer.material = defaultMaterial;
        }
    }

    private void SetSelectedMaterial()
    {
        if (cachedRenderer != null && selectedMaterial != null)
        {
            cachedRenderer.material = selectedMaterial;
        }
    }
}
