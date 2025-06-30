using UnityEngine;
using Vuforia;

public class CustomTrackingHandler : MonoBehaviour
{
    public GameObject modelToControl; // El objeto que contiene tu modelo y scripts

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        // Solo activamos si está TRACKED o EXTENDED_TRACKED
        bool isTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

        if (modelToControl != null)
        {
            modelToControl.SetActive(isTracked);
        }
    }
}
