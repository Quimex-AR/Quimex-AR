using TMPro;
using UnityEngine;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    [Header("Toast Settings")]
    public GameObject toastPrefab;
    public float toastDuration = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowToast(string message, float duration = -1f)
    {
        if (toastPrefab == null) return;

        GameObject toast = Instantiate(toastPrefab, transform);
        TMP_Text text = toast.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = message;

        CanvasGroup canvasGroup = toast.GetComponent<CanvasGroup>();
        StartCoroutine(FadeAndDestroy(toast, canvasGroup, duration > 0 ? duration : toastDuration));
    }

    private IEnumerator FadeAndDestroy(GameObject toast, CanvasGroup group, float duration)
    {
        yield return new WaitForSeconds(duration);

        float fadeTime = 0.5f;
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        Destroy(toast);
    }
}
