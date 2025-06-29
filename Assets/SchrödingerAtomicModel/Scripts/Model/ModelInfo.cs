using UnityEngine;
using Vuforia;

public class DaltonDefinitionGUI : MonoBehaviour
{
    [TextArea(3, 10)]
    public string definition =
        "El modelo atómico de Dalton, propuesto en 1803, es el primer modelo atómico con base científica. Su teoría principal es que la materia está formada por partículas indivisibles e indestructibles llamadas átomos, y que los átomos de un mismo elemento son idénticos, mientras que los de diferentes elementos son distintos";

    public Vector2 origin = new Vector2(10, 10);
    [Tooltip("Ancho máximo de la caja en píxeles")]
    public float maxWidth = 800f;

    [Tooltip("Tamaño de fuente en píxeles")]
    public int fontSize = 46;
    [Tooltip("Color del texto")]
    public Color textColor = Color.white;

    // Contador global de modelos trackeados
    private static int trackedCount = 0;

    private bool isTracked = false;
    private ObserverBehaviour observerBehaviour;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDestroy()
    {
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;

        // Si estaba trackeado al destruirse, decrementamos el contador
        if (isTracked)
            trackedCount = Mathf.Max(0, trackedCount - 1);
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool newTracked = (status.Status == Status.TRACKED);
        if (newTracked && !isTracked)
        {
            trackedCount++;
        }
        else if (!newTracked && isTracked)
        {
            trackedCount = Mathf.Max(0, trackedCount - 1);
        }
        isTracked = newTracked;
        Debug.Log($"[DaltonGUI] Model '{behaviour.gameObject.name}' isTracked={isTracked}. Total tracked: {trackedCount}");
    }

    void OnGUI()
    {
        // Si este target no está trackeado, no dibujamos nada
        if (!isTracked)
            return;

        // Si hay más de dos modelos trackeados, no mostramos nada
        if (trackedCount > 2)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
            wordWrap = true,
            fontSize = fontSize,
            normal = { textColor = textColor }
        };

        float height = style.CalcHeight(new GUIContent(definition), maxWidth);
        GUI.Box(new Rect(origin.x, origin.y, maxWidth, height), definition, style);
    }
}