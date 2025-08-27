using UnityEngine;
using System.Collections;
using Lean.Touch;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[AddComponentMenu("AR/Tooltip Manager")]
public class TooltipModalManager : MonoBehaviour
{
    public static TooltipModalManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Root overlay object (full-screen). Should be initially inactive.")]
    [SerializeField] private GameObject overlayRoot;

    [Tooltip("The modal container (child of overlayRoot).")]
    [SerializeField] private RectTransform modalRoot;

    [Tooltip("Title text (TextMeshPro).")]
    [SerializeField] private TMP_Text titleText;

    [Tooltip("Body text (TextMeshPro).")]
    [SerializeField] private TMP_Text bodyText;

    // [Tooltip("Overlay background Button (click to close).")]
    // [SerializeField] private Button overlayCloseButton;

    [Header("Animation")]
    [Tooltip("CanvasGroup used to fade in/out the modal (optional).")]
    [SerializeField] private CanvasGroup modalCanvasGroup;

    [Tooltip("Fade duration seconds.")]
    [SerializeField][Min(0f)] private float fadeDuration = 0.12f;

    // Public read-only state for other scripts to check
    public static bool IsOpen { get; private set; } = false;

    private Coroutine runningFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ensure overlay is hidden at start
        if (overlayRoot != null) overlayRoot.SetActive(false);

    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HideTooltip;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HideTooltip;
    }

    /// <summary>
    /// Show a tooltip modal for the given TooltipTrigger.
    /// </summary>
    public void ShowTooltip(TooltipModalTrigger trigger)
    {
        if (trigger == null) return;

        if (overlayRoot == null || modalRoot == null || titleText == null || bodyText == null)
        {
            Debug.LogWarning("[TooltipManager] UI references not assigned.");
            return;
        }

        // Populate text
        titleText.text = trigger.title ?? "";
        bodyText.text = trigger.body ?? "";

        // Show overlay
        overlayRoot.SetActive(true);
        IsOpen = true;

        // Cancel any running fade and start fade-in
        if (runningFade != null) StopCoroutine(runningFade);
        if (modalCanvasGroup != null) runningFade = StartCoroutine(FadeCanvas(modalCanvasGroup, 0f, 1f, fadeDuration));
        else
        {
            // Ensure modal root active and visible if no canvas group
            modalRoot.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the tooltip/modal.
    /// </summary>
    private void CloseTooltip()
    {
        if (runningFade != null) StopCoroutine(runningFade);
        if (modalCanvasGroup != null) runningFade = StartCoroutine(FadeOutAndDisable());
        else
        {
            modalRoot.gameObject.SetActive(false);
            overlayRoot.SetActive(false);
            IsOpen = false;
        }
    }

    public void HideTooltip(LeanFinger finger)
    {
        if (!IsOpen) return;
        if (!finger.IsOverGui) return;

        var results = new List<RaycastResult>();
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = finger.ScreenPosition;
        EventSystem.current.RaycastAll(eventData, results);

        bool tappedOverlay = results.Exists(r =>
            r.gameObject == overlayRoot || r.gameObject.transform.IsChildOf(overlayRoot.transform));

        bool tappedModal = results.Exists(r =>
            r.gameObject == modalRoot.gameObject || r.gameObject.transform.IsChildOf(modalRoot));

        if (tappedOverlay && !tappedModal)
        {
            Debug.Log("[TooltipModalManager] Closing modal");
            CloseTooltip();
        }

    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return FadeCanvas(modalCanvasGroup, modalCanvasGroup.alpha, 0f, fadeDuration);
        overlayRoot.SetActive(false);
        IsOpen = false;
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;
        float elapsed = 0f;
        cg.alpha = from;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        cg.alpha = to;

        // If fully hidden, block interactions
        if (Mathf.Approximately(to, 0f))
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        else
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }
}
