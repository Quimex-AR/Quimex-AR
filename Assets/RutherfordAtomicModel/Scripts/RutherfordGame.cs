using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class RutherfordSimpleGame : MonoBehaviour
{
    [Header("Atom Background")]
    public RawImage atomBackgroundImage;

    [Header("Interactive Circles")]
    public Button nucleusButton;
    public Button electronButton;
    public Button orbitButton;
    public Button vacuumButton;

    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public Button backButton;
    public GameObject winPanel;
    public TextMeshProUGUI winText;
    public Button playAgainButton;

    [Header("How to Play")]
    public GameObject howToPlayPanel;
    public Button howToPlayButton;
    public Button closeHowToPlayButton;

    [Header("Visual Settings")]
    public Color normalColor = new Color(1, 1, 1, 0.3f);
    public Color hoverColor = new Color(1, 1, 0, 0.5f);
    public Color correctColor = new Color(0, 1, 0, 0.6f);
    public Color incorrectColor = new Color(1, 0, 0, 0.6f);

    private int currentRound = 0;
    private int score = 0;
    private int correctAnswers = 0;
    private string[] targets = { "núcleo", "electrón", "órbita", "vacío" };
    private string currentTarget;
    private bool canClick = true;

    void Start()
    {
        SetupButtons();
        SetupCircles();
        StartNewRound();
        UpdateUI();

        if (howToPlayButton)
            howToPlayButton.onClick.AddListener(ShowHowToPlay);

        if (closeHowToPlayButton)
            closeHowToPlayButton.onClick.AddListener(HideHowToPlay);

        if (playAgainButton)
            playAgainButton.onClick.AddListener(RestartGame);
    }

    void SetupButtons()
    {
        if (backButton)
            backButton.onClick.AddListener(() => SceneManager.LoadScene(0));

        nucleusButton.onClick.AddListener(() => CheckAnswer("núcleo"));
        electronButton.onClick.AddListener(() => CheckAnswer("electrón"));
        orbitButton.onClick.AddListener(() => CheckAnswer("órbita"));
        vacuumButton.onClick.AddListener(() => CheckAnswer("vacío"));
    }

    void SetupCircles()
    {
        SetCircleAppearance(nucleusButton, normalColor);
        SetCircleAppearance(electronButton, normalColor);
        SetCircleAppearance(orbitButton, normalColor);
        SetCircleAppearance(vacuumButton, normalColor);

        AddHoverEffect(nucleusButton);
        AddHoverEffect(electronButton);
        AddHoverEffect(orbitButton);
        AddHoverEffect(vacuumButton);
    }

    void SetCircleAppearance(Button button, Color color)
    {
        Image circleImage = button.GetComponent<Image>();
        if (circleImage)
            circleImage.color = color;
    }

    void AddHoverEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            if (canClick) SetCircleAppearance(button, hoverColor);
        });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            if (canClick) SetCircleAppearance(button, normalColor);
        });
        trigger.triggers.Add(exitEntry);
    }

    void StartNewRound()
    {
        if (currentRound >= targets.Length)
        {
            WinGame();
            return;
        }

        currentTarget = targets[currentRound];
        canClick = true;

        SetCircleAppearance(nucleusButton, normalColor);
        SetCircleAppearance(electronButton, normalColor);
        SetCircleAppearance(orbitButton, normalColor);
        SetCircleAppearance(vacuumButton, normalColor);

        instructionText.text = $"Selecciona: {currentTarget.ToUpper()}";
        if (feedbackText) feedbackText.text = "";
    }

    void CheckAnswer(string selection)
    {
        if (!canClick) return;
        canClick = false;

        Button selectedButton = GetButtonByType(selection);

        if (selection == currentTarget)
        {
            correctAnswers++;
            score += 25;
            SetCircleAppearance(selectedButton, correctColor);
            ShowFeedback("¡CORRECTO!", Color.green);
            StartCoroutine(NextRoundDelay(1.5f));
        }
        else
        {
            score = Mathf.Max(0, score - 10);
            SetCircleAppearance(selectedButton, incorrectColor);
            ShowFeedback($"Incorrecto. Eso es: {selection}", Color.red);
            StartCoroutine(ShowCorrectAnswer());
        }

        UpdateUI();
    }

    Button GetButtonByType(string type)
    {
        switch (type)
        {
            case "núcleo": return nucleusButton;
            case "electrón": return electronButton;
            case "órbita": return orbitButton;
            case "vacío": return vacuumButton;
            default: return null;
        }
    }

    IEnumerator ShowCorrectAnswer()
    {
        yield return new WaitForSeconds(1f);

        Button correctButton = GetButtonByType(currentTarget);
        SetCircleAppearance(correctButton, correctColor);
        ShowFeedback($"La respuesta correcta era: {currentTarget}", Color.yellow);

        yield return new WaitForSeconds(2f);

        currentRound++;
        StartNewRound();
    }

    IEnumerator NextRoundDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentRound++;
        StartNewRound();
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.transform.localScale = Vector3.zero;
            StartCoroutine(AnimateScale(feedbackText.transform, Vector3.one, 0.3f));
        }
    }

    IEnumerator AnimateScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = target.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float overshoot = 1.70158f;
            t = 1f + (--t) * t * ((overshoot + 1f) * t + overshoot);
            target.localScale = Vector3.LerpUnclamped(initialScale, targetScale, t);
            yield return null;
        }

        target.localScale = targetScale;
    }

    void UpdateUI()
    {
        if (scoreText)
            scoreText.text = $"Puntuación: {score} | Ronda: {currentRound + 1}/4";
    }

    void WinGame()
    {
        canClick = false;

        if (winPanel)
        {
            winPanel.SetActive(true);
            winPanel.transform.SetAsLastSibling();

            if (winText)
            {
                float accuracy = (float)correctAnswers / targets.Length * 100;
                winText.text = $"¡Juego Completado!\n\n" +
                               $"Respuestas correctas: {correctAnswers}/4\n" +
                               $"Precisión: {accuracy:F0}%\n" +
                               $"Puntuación final: {score}\n\n" +
                               "Has aprendido las partes básicas del modelo atómico de Rutherford";
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void HideHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void SetAtomBackground(Texture texture)
    {
        if (atomBackgroundImage != null && texture != null)
        {
            atomBackgroundImage.texture = texture;
            atomBackgroundImage.enabled = true;
        }
    }
}
