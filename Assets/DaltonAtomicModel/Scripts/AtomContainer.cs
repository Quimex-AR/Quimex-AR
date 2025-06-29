using UnityEngine;
using UnityEngine.UI;

public class AtomContainer : MonoBehaviour
{
    [Header("Container Settings")]
    public Text containerLabel;
    public Image containerImage;
    public int containerIndex { get; private set; }
    
    private DaltonAtomGame gameManager;
    
    public void Setup(int index, DaltonAtomGame manager)
    {
        containerIndex = index;
        gameManager = manager;
        
        // Setup container appearance based on index
        string[] elementNames = { "H", "O", "C", "N" };
        Color[] elementColors = { 
            Color.white,      // H - Hidrógeno
            Color.red,        // O - Oxígeno  
            Color.black,      // C - Carbono
            Color.blue        // N - Nitrógeno
        };
        
        if (containerLabel && index < elementNames.Length)
        {
            containerLabel.text = elementNames[index];
        }
        
        if (containerImage && index < elementColors.Length)
        {
            containerImage.color = elementColors[index];
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Optional: Visual feedback when atom enters container area
        DraggableAtom atom = other.GetComponent<DraggableAtom>();
        if (atom != null)
        {
            // Add visual feedback like highlighting the container
            if (containerImage)
            {
                containerImage.color = containerImage.color * 1.2f; // Brighten
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        // Reset visual feedback
        DraggableAtom atom = other.GetComponent<DraggableAtom>();
        if (atom != null)
        {
            Setup(containerIndex, gameManager); // Reset color
        }
    }
}