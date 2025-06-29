using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LoadingScreenController : MonoBehaviour
{
  [Header("Loading UI")]
  public Slider progressBar;
  public TextMeshProUGUI phraseText;

  [Header("Settings")]
  public static string targetScene;
  public float phraseChangeInterval = 2f;
  public float minimumLoadScreenTime = 3f;

  // Utilities variables about the loading phrases
  private LoadingPhrases loadingPhrases;
  private List<string> allPhrases;
  private Coroutine phraseCoroutine;

  void Start()
  {
    if (FadeCanvas.Instance != null)
    {
      StartCoroutine(FadeCanvas.Instance.FadeAndHide());
    }

    LoadPhrases();
    StartCoroutine(LoadTargetScene());
  }

  private void LoadPhrases()
  {
    TextAsset jsonFile = Resources.Load<TextAsset>("loading-phrases");

    if (jsonFile != null)
    {
      loadingPhrases = JsonUtility.FromJson<LoadingPhrases>(jsonFile.text);

      allPhrases = new List<string>();
      allPhrases.AddRange(loadingPhrases.categories.scientific);
      allPhrases.AddRange(loadingPhrases.categories.curious);
      allPhrases.AddRange(loadingPhrases.categories.motivational);
    }
    else
    {
      Debug.LogError("Could not load loading-phrases.json from Resources folder!");

      allPhrases = new List<string> { "Cargando..." };
    }
  }

  private IEnumerator LoadTargetScene()
  {
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
    asyncLoad.allowSceneActivation = false;

    phraseCoroutine = StartCoroutine(ChangePhrases());

    float elapsedTime = 0f;

    while (!asyncLoad.isDone)
    {
      elapsedTime += Time.deltaTime;

      if (progressBar != null)
        progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);

      if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadScreenTime)
      {
        if (FadeCanvas.Instance != null)
        {
          StartCoroutine(FadeCanvas.Instance.FadeToBlack());
        }

        asyncLoad.allowSceneActivation = true;
      }

      yield return null;
    }

    if (phraseCoroutine != null)
      StopCoroutine(phraseCoroutine);
  }

  private IEnumerator ChangePhrases()
  {
    while (true)
    {
      if (allPhrases != null && allPhrases.Count > 0 && phraseText != null)
      {
        string randomPhrase = allPhrases[Random.Range(0, allPhrases.Count)];
        phraseText.text = randomPhrase;
      }

      yield return new WaitForSeconds(phraseChangeInterval);
    }
  }
}
