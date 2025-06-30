using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 10f;
    public bool isTouching = false;

    void Update()
    {
        if (!isTouching)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }

    public void PauseRotation()
    {
        isTouching = true;
    }

    public void ContinueRotation()
    {
        isTouching = false;
    }
}
