using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Touch;

public class DraggableAtom : MonoBehaviour
{
    [Header("Components")]
    public Image atomImage;
    public TextMeshProUGUI atomText;
    
    private DaltonAtomGame.AtomData atomData;
    private DaltonAtomGame gameManager;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private LeanFinger draggingFinger;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (atomImage == null)
            atomImage = GetComponent<Image>();
        
        if (atomText == null)
            atomText = GetComponentInChildren<TextMeshProUGUI>();
            
        // Agregar LeanSelectableByFinger si no existe
        var selectable = GetComponent<LeanSelectableByFinger>();
        if (selectable == null)
        {
            selectable = gameObject.AddComponent<LeanSelectableByFinger>();
        }
    }
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
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
        
        // Asegurar que tenga BoxCollider2D para detección
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = rectTransform.sizeDelta;
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
        // Verificar si el toque está sobre este átomo
        Vector2 worldPoint = finger.GetWorldPosition(rectTransform.position.z, canvas.worldCamera);
        
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, finger.ScreenPosition, canvas.worldCamera))
        {
            draggingFinger = finger;
            originalPosition = transform.position;
            transform.SetAsLastSibling(); // Traer al frente
            
            // Efecto visual de selección
            transform.localScale = Vector3.one * 1.1f;
        }
    }
    
    void HandleFingerUpdate(LeanFinger finger)
    {
        if (finger != draggingFinger) return;
        
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
        if (finger != draggingFinger) return;
        
        draggingFinger = null;
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
        if (!droppedCorrectly && draggingFinger == null)
        {
            transform.position = originalPosition;
        }
    }
}