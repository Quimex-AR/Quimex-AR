using TMPro;
using UnityEngine;

[System.Serializable]
public class InputValidationRule
{
    public TMP_InputField inputField;
    public TextMeshProUGUI errorText;
    public string errorMessage;
    public UnityEngine.UI.Image inputBackground;
    public Sprite errorSprite;
    [HideInInspector] public Sprite defaultSprite;
}
