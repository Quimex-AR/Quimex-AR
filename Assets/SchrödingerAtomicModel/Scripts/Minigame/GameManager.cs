using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Transform cloudCenter;
    public float cloudRadius = 1.5f;

    public GameObject electronPrefab;
    public GameObject tapMarkerPrefab; // 🔧 NUEVO: esfera para marcar el tap del jugador

    public Collider cloudCollider;
    public Collider electronCollider;

    private GameObject currentElectron;
    private GameObject currentTapMarker; // 🔧 NUEVO

    private Vector3 electronPosition;

    public UIManager uiManager;

    void Start()
    {
        SpawnElectron();
    }

    public void SpawnElectron()
    {
        electronPosition = cloudCenter.position + Random.insideUnitSphere * cloudRadius;
        currentElectron = Instantiate(electronPrefab, electronPosition, Quaternion.identity);
        currentElectron.SetActive(false);

        Debug.Log("Nuevo electrón en: " + electronPosition);
    }

    public void HandleTap(Vector3 tapPosition)
    {
        DisableCloudCollider();

        // 🔧 NUEVO: Mostrar marcador donde tocó el jugador
        if (tapMarkerPrefab != null)
        {
            // Si ya hay uno, lo destruye antes
            if (currentTapMarker != null)
                Destroy(currentTapMarker);

            currentTapMarker = Instantiate(tapMarkerPrefab, tapPosition, Quaternion.identity);
        }

        float distance = Vector3.Distance(tapPosition, electronPosition);
        float maxDistance = cloudRadius;
        float accuracy = Mathf.Clamp01(1f - (distance / maxDistance));
        float percentage = accuracy * 100f;

        currentElectron.SetActive(true); // Mostrar electrón
        HideAllVisuals();
        uiManager.ShowResult($"Precisión: {percentage:F1}%");

        Debug.Log($"Tap real: {tapPosition} | Electron: {electronPosition} | Distancia: {distance}");
    }

    public void RestartGame()
    {
        if (currentElectron != null)
            Destroy(currentElectron);

        // 🔧 NUEVO: elimina marcador anterior si existe
        if (currentTapMarker != null)
            Destroy(currentTapMarker);

        if (orbitalObject != null)
            orbitalObject.SetActive(true);

        uiManager.HideResult();
        SpawnElectron();
        StartCoroutine(EnableColliderAfterDelay());
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);
        EnableCloudCollider();
    }

    public void EnableCloudCollider()
    {
        if (cloudCollider != null) cloudCollider.enabled = true;
        if (electronCollider != null) electronCollider.enabled = true;
    }

    public void DisableCloudCollider()
    {
        if (cloudCollider != null) cloudCollider.enabled = false;
        if (electronCollider != null) electronCollider.enabled = false;
    }

    public GameObject orbitalObject; // 🔧 asigna tu esfera de nube

    public void ShowAllVisuals()
    {
        if (currentElectron != null)
            currentElectron.SetActive(true);
        if (currentTapMarker != null)
            currentTapMarker.SetActive(true);
        if (orbitalObject != null)
            orbitalObject.SetActive(true);
    }

    public void HideAllVisuals()
    {
        if (currentElectron != null)
            currentElectron.SetActive(false);
        if (currentTapMarker != null)
            currentTapMarker.SetActive(false);
        if (orbitalObject != null)
            orbitalObject.SetActive(false);
    }

}
