using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class DaltonAtomGame : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject atomPrefab;
    public Transform atomSpawnArea;
    public Transform[] containers; // H, O, C, N containers

    [Header("Atom Data")]
    public AtomData[] atomTypes;

    [Header("UI")]
    public GameObject gameUI; // Nuevo campo para el panel del juego
    public GameObject containersPanel; // Nuevo campo para ocultar los contenedores
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button backButton;
    public Button howToPlayButton; // Nuevo botón
    public GameObject howToPlayPanel; // Panel de instrucciones
    public Button closeHowToPlayButton; // Botón para cerrar instrucciones
    public GameObject winPanel;
    public TextMeshProUGUI winScoreText;
    public Button playAgainButton;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public Button retryButton;
    
    [Header("Tips System")]
    public TextMeshProUGUI tipsText;
    public float tipChangeInterval = 5f; // Cambiar tip cada 5 segundos
    private string[] gameTips = {
        "Tip: El Hidrógeno (H) es el elemento más ligero",
        "Curiosidad: Dalton propuso que los átomos eran esferas sólidas en 1803",
        "Dato: El Nitrógeno (N) forma el 78% de nuestra atmósfera",
        "Pista: Los átomos rojos son Oxígeno (O)",
        "Curiosidad: El Carbono (C) es la base de toda la vida",
        "Tip: ¡Arrastra rápido! El tiempo corre",
        "Consejo: Cada acierto suma 10 puntos",
        "Cuidado: Los errores restan 5 puntos",
        "Dato: John Dalton era daltónico (no distinguía colores)"
    };
    
    [Header("Game Settings")]
    public int atomsToGenerate = 12;
    public float gameTime = 60f;
    public float spawnDelay = 0.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip winSound;

    private int score = 0;
    private float currentTime;
    private List<GameObject> activeAtoms = new List<GameObject>();
    private bool gameActive = false;

    [System.Serializable]
    public class AtomData
    {
        public string elementName; // H, O, C, N
        public Color atomColor;
        public int containerIndex;
    }

    void Start()
    {
        currentTime = gameTime;
        SetupContainers();
        StartCoroutine(StartGame());
        StartCoroutine(CycleTips()); // Iniciar el ciclo de tips
        
        if (backButton)
            backButton.onClick.AddListener(backButton.GetComponent<ReturnUIButton>().OnReturnClicked);
            
        if (howToPlayButton)
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
            
        if (closeHowToPlayButton)
            closeHowToPlayButton.onClick.AddListener(HideHowToPlay);
            
        if (playAgainButton)
            playAgainButton.onClick.AddListener(RestartGame);

        if (retryButton)
            retryButton.onClick.AddListener(RestartGame);

        // Inicializar los datos de los átomos si no están configurados
        if (atomTypes.Length == 0)
        {
            atomTypes = new AtomData[]
            {
            new AtomData { elementName = "H", atomColor = new Color(1f, 0f, 1f), containerIndex = 0 }, // Magenta/Púrpura
            new AtomData { elementName = "O", atomColor = Color.red, containerIndex = 1 }, // Rojo
            new AtomData { elementName = "C", atomColor = Color.green, containerIndex = 2 }, // Verde
            new AtomData { elementName = "N", atomColor = Color.blue, containerIndex = 3 } // Azul
            };
        }
    }

    void Update()
    {
        if (!gameActive) return;

        currentTime -= Time.deltaTime;
        UpdateUI();

        if (currentTime <= 0)
        {
            EndGame();
        }

        if (activeAtoms.Count == 0 && gameActive)
        {
            WinGame();
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        yield return GenerateAtomsWithAnimation();
        gameActive = true;
        UpdateUI();
    }

    void SetupContainers()
    {
        for (int i = 0; i < containers.Length; i++)
        {
            AtomContainer container = containers[i].GetComponent<AtomContainer>();
            if (container == null)
            {
                container = containers[i].gameObject.AddComponent<AtomContainer>();
            }
            container.Setup(i, this);

            // Asegurar que tenga BoxCollider2D
            var collider = containers[i].GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = containers[i].gameObject.AddComponent<BoxCollider2D>();
                RectTransform rect = containers[i].GetComponent<RectTransform>();
                collider.size = rect.sizeDelta;
                collider.isTrigger = true;
            }
        }
    }

    IEnumerator GenerateAtomsWithAnimation()
    {
        // Limpiar la lista antes de generar nuevos átomos
        activeAtoms.Clear();

        for (int i = 0; i < atomsToGenerate; i++)
        {
            AtomData randomAtomType = atomTypes[Random.Range(0, atomTypes.Length)];

            Vector3 spawnPos = GetRandomSpawnPosition();

            // Crear el átomo como hijo del Canvas, NO del AtomSpawnArea
            Canvas canvas = atomSpawnArea.GetComponentInParent<Canvas>();
            GameObject newAtom = Instantiate(atomPrefab, spawnPos, Quaternion.identity, canvas.transform);

            // Asegurar que esté en la posición correcta
            newAtom.transform.position = spawnPos;

            // Animación de aparición simple sin LeanTween
            newAtom.transform.localScale = Vector3.one;

            DraggableAtom atomScript = newAtom.GetComponent<DraggableAtom>();
            if (atomScript == null)
            {
                atomScript = newAtom.AddComponent<DraggableAtom>();
            }

            atomScript.Setup(randomAtomType, this);

            // Agregar a la lista ANTES de generar el siguiente
            activeAtoms.Add(newAtom);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        RectTransform spawnRect = atomSpawnArea.GetComponent<RectTransform>();

        // Obtener los límites del área de spawn en coordenadas locales
        Vector3[] corners = new Vector3[4];
        spawnRect.GetLocalCorners(corners);

        // Calcular posición aleatoria dentro del área
        float minX = corners[0].x + 60; // Margen para que no aparezcan en el borde
        float maxX = corners[2].x - 60;
        float minY = corners[0].y + 60;
        float maxY = corners[2].y - 60;

        // Intentar encontrar una posición libre
        int maxAttempts = 50;
        float minDistance = 150f; // Distancia mínima entre átomos (diámetro del átomo)

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            // Convertir a posición mundial
            Vector3 localPos = new Vector3(randomX, randomY, 0);
            Vector3 worldPos = atomSpawnArea.TransformPoint(localPos);

            // Verificar si está lo suficientemente lejos de otros átomos
            bool positionIsValid = true;
            foreach (GameObject atom in activeAtoms)
            {
                if (atom != null)
                {
                    float distance = Vector3.Distance(worldPos, atom.transform.position);
                    if (distance < minDistance)
                    {
                        positionIsValid = false;
                        break;
                    }
                }
            }

            if (positionIsValid)
            {
                return worldPos;
            }
        }

        // Si no encuentra una posición libre después de muchos intentos,
        // devolver una posición aleatoria de todas formas
        float finalX = Random.Range(minX, maxX);
        float finalY = Random.Range(minY, maxY);
        Vector3 finalLocalPos = new Vector3(finalX, finalY, 0);
        return atomSpawnArea.TransformPoint(finalLocalPos);
    }

    public void AtomPlacedCorrectly(GameObject atom)
    {
        score += 10;
        activeAtoms.Remove(atom);

        // Efectos
        if (audioSource && correctSound)
            audioSource.PlayOneShot(correctSound);

        // Destruir directamente sin animación
        Destroy(atom);

        UpdateUI();
    }

    public void AtomPlacedIncorrectly(GameObject atom)
    {
        score = Mathf.Max(0, score - 5);

        // Efectos
        if (audioSource && wrongSound)
            audioSource.PlayOneShot(wrongSound);

        // Efecto visual simple de error
        StartCoroutine(ShakeAtom(atom));

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Puntuación: " + score;
        if (timerText)
        {
            timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
            if (currentTime < 10)
                timerText.color = Color.red;
        }
    }
    
 void EndGame()
{
    gameActive = false;
    
    // DESTRUIR O DESACTIVAR TODOS LOS ÁTOMOS
    DestroyAllAtoms();
    
    // Pausar el tiempo del juego
    Time.timeScale = 0f;
    
    // Ocultar el UI del juego y los contenedores
    if (gameUI) gameUI.SetActive(false);
    if (containersPanel) containersPanel.SetActive(false);
    
    if (gameOverPanel) 
    {
        gameOverPanel.SetActive(true);
        // Asegurar que el panel esté al frente
        gameOverPanel.transform.SetAsLastSibling();
        
        if (gameOverScoreText)
            gameOverScoreText.text = "Puntuación Final: " + score;
    }
}

// Agregar este nuevo método
void DestroyAllAtoms()
{
    // Método 1: Destruir todos los átomos activos
    foreach (GameObject atom in activeAtoms)
    {
        if (atom != null)
        {
            Destroy(atom);
        }
    }
    activeAtoms.Clear();
    
    // Método 2: Por si acaso, buscar todos los átomos restantes
    DraggableAtom[] remainingAtoms = FindObjectsOfType<DraggableAtom>();
    foreach (DraggableAtom atom in remainingAtoms)
    {
        if (atom != null)
        {
            Destroy(atom.gameObject);
        }
    }
}
    void WinGame()
    {
        gameActive = false;

        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        // Ocultar el UI del juego y los contenedores
        if (gameUI) gameUI.SetActive(false);
        if (containersPanel) containersPanel.SetActive(false);

        if (winPanel)
        {
            winPanel.SetActive(true);
            // Asegurar que el panel esté al frente
            winPanel.transform.SetAsLastSibling();
            
            // Animación de fade in opcional
            Image panelImage = winPanel.GetComponent<Image>();
            if (panelImage)
            {
                Color c = panelImage.color;
                c.a = 0;
                panelImage.color = c;
                StartCoroutine(FadeIn(panelImage, 0.5f));
            }
            if (winScoreText)
                winScoreText.text = "¡Excelente!\nPuntuación: " + score;
        }

        if (audioSource && winSound)
            audioSource.PlayOneShot(winSound);
    }
    
    IEnumerator FadeIn(Image image, float duration)
    {
        float elapsed = 0;
        Color c = image.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0, 1, elapsed / duration);
            image.color = c;
            yield return null;
        }
        
        c.a = 1;
        image.color = c;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaurar el tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoBackToAR()
    {
        Time.timeScale = 1f; // Restaurar el tiempo
        SceneManager.LoadScene(0); // Índice 0 = DaltonScene (escena AR)
    }

    // Corrutina simple para hacer shake sin LeanTween
    IEnumerator ShakeAtom(GameObject atom)
    {
        Vector3 originalPos = atom.transform.position;
        float shakeAmount = 10f;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
            atom.transform.position = new Vector3(x, originalPos.y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        atom.transform.position = originalPos;
    }
    
    // Sistema de tips rotatorios
    IEnumerator CycleTips()
    {
        int currentTipIndex = 0;
        
        while (true)
        {
            if (tipsText != null && gameActive)
            {
                // Fade out
                yield return StartCoroutine(FadeTextTo(0f, 0.5f));
                
                // Cambiar texto
                tipsText.text = gameTips[currentTipIndex];
                currentTipIndex = (currentTipIndex + 1) % gameTips.Length;
                
                // Fade in
                yield return StartCoroutine(FadeTextTo(1f, 0.5f));
            }
            
            yield return new WaitForSeconds(tipChangeInterval);
        }
    }
    
    IEnumerator FadeTextTo(float targetAlpha, float duration)
    {
        if (tipsText == null) yield break;
        
        Color currentColor = tipsText.color;
        float startAlpha = currentColor.a;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            tipsText.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }
        
        tipsText.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
    
    // Mostrar panel de instrucciones
    public void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
            Time.timeScale = 0f; // Pausar el juego
        }
    }
    
    public void HideHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            Time.timeScale = 1f; // Reanudar el juego
        }
    }
}

