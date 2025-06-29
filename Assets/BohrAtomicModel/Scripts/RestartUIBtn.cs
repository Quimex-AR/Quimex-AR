using UnityEngine;
using UnityEngine.UI;

public class RestartUIBtn : MonoBehaviour
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
}
