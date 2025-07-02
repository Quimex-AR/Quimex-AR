using UnityEngine;

public class QuimexHIW : MonoBehaviour
{
    public GameObject objectHIW;
    public bool startActive = false;

    void Start()
    {
        if (objectHIW != null && !startActive)
        {
            objectHIW.SetActive(false);
        }
    }

    public void ShowHIW()
    {
        objectHIW.SetActive(true);
    }

    public void HideHIW()
    {
        objectHIW.SetActive(false);
    }
}
