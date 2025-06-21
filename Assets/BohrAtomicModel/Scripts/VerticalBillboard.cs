using UnityEngine;

[ExecuteInEditMode]
public class VerticalBillboard : MonoBehaviour
{
    [Tooltip("The target Transform that the GameObject will face. \nNone/null = MainCamera.")]
    public Transform target;

    void Start()
    {
        if (target == null)
        {
            if (Camera.main != null)
            {
                target = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning($"VerticalBillboard: No MainCamera found in the scene.");
            }
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target, Vector3.up);
        }
    }
}
