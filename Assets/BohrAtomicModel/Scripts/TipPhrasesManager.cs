using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TipPhrasesManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public float changeInterval = 7f;

    private TipPhrases tipsPhrases;
    private List<TipPhrases.Tip> tips;
    private int currentIndex = 0;

    private enum GameTipState
    {
        WaitingForStart,
        Playing,
        Won
    }

    private GameTipState currentState = GameTipState.WaitingForStart;
    private bool startedRotatingTips = false;
    private bool hasWon = false;

    void Start()
    {
        LoadTips();
        tipText.text = "Elige una dificultad para empezar.";
    }

    void Update()
    {
        if (!BohrAtomPhysicsController.isGameReady && !hasWon)
        {
            // Waiting to start or loading
            if (currentState != GameTipState.WaitingForStart)
            {
                currentState = GameTipState.WaitingForStart;
                tipText.text = "Elige una dificultad para empezar.";
                StopAllCoroutines();
                startedRotatingTips = false;
            }
        }
        else if (BohrAtomPhysicsController.isGameReady && !hasWon)
        {
            // Game is ready, tips can rotate
            if (!startedRotatingTips && tips != null && tips.Count > 0)
            {
                currentState = GameTipState.Playing;
                StartCoroutine(RotateTips());
                startedRotatingTips = true;
            }
        }
        else if (hasWon && currentState != GameTipState.Won)
        {
            currentState = GameTipState.Won;
            StopAllCoroutines();
            tipText.text = "¡Felicidades! Prueba otro modo.";
        }
    }

    void LoadTips()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("tip-phrases");

        if (jsonFile != null)
        {
            tipsPhrases = JsonUtility.FromJson<TipPhrases>(jsonFile.text);
            tips = tipsPhrases.tips;
        }
        else
        {
            Debug.LogError("Could not load tip-phrases.json from Resources folder!");
        }
    }

    IEnumerator RotateTips()
    {
        while (true)
        {
            tipText.text = tips[currentIndex].text;
            currentIndex = (currentIndex + 1) % tips.Count;
            yield return new WaitForSeconds(changeInterval);
        }
    }

    public void ShowWinMessage()
    {
        hasWon = true;
    }
}
