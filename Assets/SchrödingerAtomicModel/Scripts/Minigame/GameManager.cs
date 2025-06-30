using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Transform cloudCenter;
    public float cloudRadius = 1.5f;
    public GameObject electronPrefab;
    public Collider cloudCollider;
    public Collider electronCollider;

    private GameObject currentElectron;
    private Vector3 electronPosition;

    public UIManager uiManager;

    void Start()
    {
        SpawnElectron();
    }

    public void SpawnElectron()
    {
        // Genera nueva posición aleatoria dentro de la nube
        electronPosition = cloudCenter.position + Random.insideUnitSphere * cloudRadius;
        currentElectron = Instantiate(electronPrefab, electronPosition, Quaternion.identity);
        currentElectron.SetActive(false);

        Debug.Log("Nuevo electrón en: " + electronPosition);
    }

    public void HandleTap(Vector3 tapPosition)
    {
        // Desactiva colisión para evitar taps dobles
        DisableCloudCollider();

        float distance = Vector3.Distance(tapPosition, electronPosition);
        float maxDistance = cloudRadius;

        float accuracy = Mathf.Clamp01(1f - (distance / maxDistance));
        float percentage = accuracy * 100f;

        currentElectron.SetActive(true); // Revelar electrón
        uiManager.ShowResult($"Precisión: {percentage:F1}%");
        Debug.Log($"Tap real: {tapPosition} | Electron: {electronPosition} | Distance: {Vector3.Distance(tapPosition, electronPosition)}");

    }

    public void RestartGame()
    {
        // Destruye electrón anterior
        if (currentElectron != null)
        {
            Destroy(currentElectron);
        }

        uiManager.HideResult();
        SpawnElectron();

        // Espera para evitar tap fantasma
        StartCoroutine(EnableColliderAfterDelay());
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // evita detección accidental
        EnableCloudCollider();
    }

    public void EnableCloudCollider()
    {
        if (cloudCollider != null)
            cloudCollider.enabled = true;
        if (electronCollider != null)
            electronCollider.enabled = true;
    }

    public void DisableCloudCollider()
    {
        if (cloudCollider != null)
            cloudCollider.enabled = false;
        if (electronCollider != null)
            electronCollider.enabled = false;
    }
}
