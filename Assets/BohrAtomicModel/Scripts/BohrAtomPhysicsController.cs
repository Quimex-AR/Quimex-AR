using System.Collections;
using UnityEngine;

public class BohrAtomPhysicsController : MonoBehaviour
{
    public float gravityActivationDelay = 2f;
    public static bool isGameReady = false;

    void Start()
    {
        StartCoroutine(EnableGravityAfterDelay());
    }

    IEnumerator EnableGravityAfterDelay()
    {
        yield return new WaitForSeconds(gravityActivationDelay);

        Rigidbody2D[] electronBodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in electronBodies)
        {
            rb.gravityScale = 1f;
        }

        yield return new WaitForSeconds(1f);

        isGameReady = true;
    }
}
