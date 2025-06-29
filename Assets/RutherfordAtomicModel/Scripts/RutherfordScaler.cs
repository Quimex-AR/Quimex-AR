using UnityEngine;

public class RutherfordScaler : MonoBehaviour
{
    private bool isLarge = false;

    public float scaleMultiplier = 1.5f;

    public void ToggleScale()
    {
        if (isLarge)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one * scaleMultiplier;
        }

        isLarge = !isLarge;
    }
}
