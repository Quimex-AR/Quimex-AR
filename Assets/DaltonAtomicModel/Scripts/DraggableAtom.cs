using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Touch;

public class DraggableAtom : MonoBehaviour
{
    [Header("Components")]
    public Image atomImage;
    public TextMeshProUGUI atomText;
    
    [Header("Touch Settings")]
    public float touchRadius = 50f; // Radio de detección más preciso
    
    private DaltonAtomGame.AtomData atomData;
    private DaltonAtomGame gameManager;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private LeanFinger draggingFinger;
    private bool isDragging = false;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (atomImage == null)
            atomImage = GetComponent<Image>();
        
        if (atomText == null)
            atomText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        
        // Ajustar el collider al tamaño real del átomo
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Hacer el collider más pequeño y centrado
        collider.size = rectTransform.sizeDelta * 0.8f; // 80% del tamaño para evitar overlaps
        collider.isTrigger = true;
    }
    
    void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }
    
    void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }
    
    public void Setup(DaltonAtomGame.AtomData data, DaltonAtomGame manager)
    {
        atomData = data;
        gameManager = manager;
        originalPosition = transform.position;
        
        // Setup visual appearance
        if (atomImage)
        {
            atomImage.color = atomData.atomColor;
        }
        
        if (atomText)
        {
            atomText.text = atomData.elementName;
            atomText.color = GetContrastColor(atomData.atomColor);
        }
    }
    
    Color GetContrastColor(Color color)
    {
        // Caso especial para el carbono (verde) - forzar texto blanco
        if (atomData.elementName == "C")
        {
            return Color.white;
        }
        
        // Para el resto de elementos, usar la lógica de contraste normal
        float brightness = (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f);
        return brightness > 0.5f ? Color.black : Color.white;
    }
    
    void HandleFingerDown(LeanFinger finger)
    {
        // Prevenir múltiples selecciones simultáneas
        if (isDragging) return;
        
        // Convertir posición del toque a coordenadas del canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            finger.ScreenPosition,
            canvas.worldCamera,
            out localPoint
        );
        
        Vector3 worldPoint = canvas.transform.TransformPoint(localPoint);
        
        // Verificar distancia precisa al centro del átomo
        float distance = Vector2.Distance(transform.position, worldPoint);
        
        // Solo seleccionar si el toque está dentro del radio específico del átomo
        if (distance <= touchRadius)
        {
            // Verificar que no haya otro átomo más cerca
            if (IsClosestAtomToTouch(worldPoint))
            {
                draggingFinger = finger;
                isDragging = true;
                originalPosition = transform.position;
                transform.SetAsLastSibling(); // Traer al frente
                
                // Efecto visual de selección
                transform.localScale = Vector3.one * 1.1f;
            }
        }
    }
    
    bool IsClosestAtomToTouch(Vector3 touchWorldPos)
    {
        DraggableAtom[] allAtoms = FindObjectsOfType<DraggableAtom>();
        float myDistance = Vector2.Distance(transform.position, touchWorldPos);
        
        foreach (DraggableAtom otherAtom in allAtoms)
        {
            if (otherAtom == this) continue;
            
            float otherDistance = Vector2.Distance(otherAtom.transform.position, touchWorldPos);
            
            // Si hay otro átomo más cerca, no seleccionar este
            if (otherDistance < myDistance)
            {
                return false;
            }
        }
        
        return true;
    }
    
    void HandleFingerUpdate(LeanFinger finger)
    {
        if (finger != draggingFinger || !isDragging) return;
        
        // Convertir posición del dedo a posición en el canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            finger.ScreenPosition,
            canvas.worldCamera,
            out localPoint
        );
        
        transform.position = canvas.transform.TransformPoint(localPoint);
    }
    
    void HandleFingerUp(LeanFinger finger)
    {
        if (finger != draggingFinger || !isDragging) return;
        
        draggingFinger = null;
        isDragging = false;
        transform.localScale = Vector3.one;
        
        CheckForDrop();
    }
    
    void CheckForDrop()
    {
        // Buscar todos los contenedores
        AtomContainer[] containers = FindObjectsOfType<AtomContainer>();
        bool droppedCorrectly = false;
        
        foreach (AtomContainer container in containers)
        {
            RectTransform containerRect = container.GetComponent<RectTransform>();
            
            // Verificar si el centro del átomo está dentro del contenedor
            if (RectTransformUtility.RectangleContainsScreenPoint(
                containerRect,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, transform.position),
                canvas.worldCamera))
            {
                // Verificar si es el contenedor correcto
                if (container.containerIndex == atomData.containerIndex)
                {
                    gameManager.AtomPlacedCorrectly(gameObject);
                    droppedCorrectly = true;
                }
                else
                {
                    gameManager.AtomPlacedIncorrectly(gameObject);
                }
                break;
            }
        }
        
        // Si no se soltó en ningún contenedor, volver a la posición original
        if (!droppedCorrectly)
        {
            transform.position = originalPosition;
        }
    }
    
    // Método para debugging - puedes removerlo después
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, touchRadius);
    }
}