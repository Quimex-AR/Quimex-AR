using UnityEngine;

public class SelfRotator : MonoBehaviour
{
    [Tooltip("Object that will rotate. If none is assigned, this GameObject will rotate.")]
    public GameObject rotatorObject;

    [Tooltip("Rotation speed in degrees per second on each axis")]
    public Vector3 rotationSpeed = new(0, 50, 0);

    private bool isRotationLocked = false;

    void Update()
    {
        if (isRotationLocked) return;

        Transform target = rotatorObject != null ? rotatorObject.transform : transform;
        target.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    /// Call to stop the rotation
    /// </summary>
    public void LockRotation()
    {
        isRotationLocked = true;
    }

    /// <summary>
    /// Call to resume the rotation
    /// </summary>
    public void UnlockRotation()
    {
        isRotationLocked = false;
    }


    /// <summary>
    /// Toggles the rotation state between locked and unlocked.
    /// </summary>
    public void ToggleRotation()
    {
        isRotationLocked = !isRotationLocked;
    }
}
