using UnityEngine;

public class GameplayHint : MonoBehaviour
{
    void Start()
    {
        EnableGameplayHint(); // Cambiado: ahora se muestra al iniciar
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