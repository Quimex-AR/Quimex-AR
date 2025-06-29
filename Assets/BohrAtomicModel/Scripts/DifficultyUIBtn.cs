using UnityEngine;
using UnityEngine.UI;

public class DifficultyUIBtn : MonoBehaviour
{
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
}
