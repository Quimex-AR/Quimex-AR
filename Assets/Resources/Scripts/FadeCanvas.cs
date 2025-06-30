using System.Collections;
using UnityEngine;

public class FadeCanvas : MonoBehaviour
{
  public static FadeCanvas Instance { get; private set; }

  [Header("Scene Transition Elements")]
  public GameObject parent;
  [Tooltip("The panel used for fading effects.")]
  public GameObject fadePanel;
  [Tooltip("The CanvasGroup component used to control the alpha value.")]
  public CanvasGroup canvasGroup;

  [Header("Fade Settings")]
  [Tooltip("Duration of the fade effect in seconds.")]
  public float fadeDuration = 0.5f;

  void Start()
  {
    if (Instance != null)
    {
      StartCoroutine(Instance.FadeAndHide());
    }
  }

  void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    // DontDestroyOnLoad(gameObject); // Optional: persist across scenes
  }

  public IEnumerator FadeToBlack()
  {
    // Start fade in (0 -> 1)
    float t = 0f;
    canvasGroup.alpha = 0f;
    fadePanel.SetActive(true);

    while (t < fadeDuration)
    {
      t += Time.deltaTime;
      canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
      yield return null;
    }

    canvasGroup.alpha = 1f;
    yield return new WaitForSeconds(0.5f);
  }

  public IEnumerator FadeAndHide()
  {
    // Start fade out (1 -> 0)
    float t = 0f;
    canvasGroup.alpha = 1f;
    fadePanel.SetActive(true);

    while (t < fadeDuration)
    {
      t += Time.deltaTime;
      canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
      yield return null;
    }

    canvasGroup.alpha = 0f;

    fadePanel.SetActive(false);
  }

  public void Show()
  {
    fadePanel.SetActive(true);
  }

  public void Hide()
  {
    fadePanel.SetActive(false);
  }
}
