using TMPro;
using UnityEngine;

[System.Serializable]
public class InputValidationRule
{
    public TMP_InputField inputField;
    public TextMeshProUGUI errorText;
    public string errorMessage;
    // public UnityEngine.UI.Image inputBackground;

    // // Sprite image fields
    // public Sprite errorSprite;
    // [HideInInspector] public Sprite defaultSprite;

    // // Tint color fields
    // public Color errorTint;
    // [HideInInspector] public Color defaultTint;
}
