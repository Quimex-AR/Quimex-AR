using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Gameplay")]
    public GameObject atomModel;
    public float timeLimit = 30f;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI collectedText;
    public TextMeshProUGUI remainingText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    
    [Header("Instructions Panel")]
    public GameObject instructionsPanel; // Referencia al panel de instrucciones

    private List<GameObject> electrons = new List<GameObject>();
    private int collected = 0;
    private float timer;
    private bool gameRunning = false; // Cambiado: empezar pausado
    private float timeElapsed = 0f;
    private bool instructionsActive = true; // Nuevo: controlar estado de instrucciones

    void Start()
    {
        timer = timeLimit;
        timeElapsed = 0f;

        gameOverPanel.SetActive(false);
        
        // Al inicio SIEMPRE ocultar el modelo hasta que se cierren las instrucciones
        atomModel.SetActive(false);
        instructionsActive = true;
        gameRunning = false;

        foreach (Transform child in atomModel.transform)
        {
            if (child.name.ToLower().Contains("electron"))
            {
                electrons.Add(child.gameObject);
                Electron script = child.gameObject.AddComponent<Electron>();
                script.gameController = this;
            }
        }

        UpdateUI();
    }

    void Update()
    {
        if (!gameRunning) return;

        // SOLO avanzar el contador si el modelo está activo
        if (atomModel.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            timeElapsed += Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                gameRunning = false;
                EndGame_TimeOut();
            }

            UpdateUI();
        }
    }

    public void StartGame()
    {
        atomModel.SetActive(true);
        gameRunning = true;
        instructionsActive = false;
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            atomModel.SetActive(false);
            gameRunning = false;
            instructionsActive = true;
        }
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            StartGame();
        }
    }

    public void OnElectronTapped(GameObject electron)
    {
        if (!gameRunning) return;

        collected++;
        electrons.Remove(electron);
        Destroy(electron);

        if (electrons.Count == 0)
        {
            gameRunning = false;
            EndGame_Win();
        }

        UpdateUI();
    }

    public void OnWrongTap()
    {
        if (!gameRunning) return;

        gameRunning = false;
        EndGame_WrongTap();
    }

    void EndGame_WrongTap()
    {
        atomModel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverText.text = " Perdiste por tocar la masa positiva.";
    }

    void EndGame_TimeOut()
    {
        atomModel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverText.text = $" Tiempo agotado.\nRecolectaste {collected} electrones.\nTe faltaron {electrons.Count}.";
    }

    void EndGame_Win()
    {
        atomModel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverText.text = $" ¡Ganaste!\nTiempo: {timeElapsed:F1} segundos.";
    }

    void UpdateUI()
    {
        timeText.text = $"Tiempo: {timer:F1}s";
        collectedText.text = $"Recolectados: {collected}";
        remainingText.text = $"Faltan: {electrons.Count}";
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReactivateAtomModel()
    {
        StartCoroutine(DelayedActivation());
    }

    private IEnumerator DelayedActivation()
    {
        yield return new WaitForSeconds(0.5f);
        atomModel.SetActive(true);
    }
}