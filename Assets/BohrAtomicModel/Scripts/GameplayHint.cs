using UnityEngine;

public class GameplayHint : MonoBehaviour
{
    void Start()
    {
        DisableGameplayHint();
    }

    public void EnableGameplayHint()
    {
        gameObject.SetActive(true);
    }

    public void DisableGameplayHint()
    {
        gameObject.SetActive(false);
    }
}
