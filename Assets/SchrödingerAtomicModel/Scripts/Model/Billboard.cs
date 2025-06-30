using UnityEngine;

public class BillboardFaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        // Dirección desde el cartel hasta la cámara
        Vector3 direction = transform.position - Camera.main.transform.position;

        // Asegura que el eje 'up' se mantenga correctamente
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
