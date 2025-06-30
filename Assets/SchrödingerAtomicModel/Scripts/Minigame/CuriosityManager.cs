using UnityEngine;
using TMPro;
using Lean.Touch;
using System.Collections;

public class CuriosityManager : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] curiosityTexts;

    public TextMeshProUGUI curiosityText;

    private int lastIndex = -1;
    private Coroutine autoChangeRoutine;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += OnFingerTap;
        autoChangeRoutine = StartCoroutine(ChangeEvery10Seconds());
    }

    void OnDisable()
    {
        LeanTouch.OnFingerTap -= OnFingerTap;
        if (autoChangeRoutine != null)
        {
            StopCoroutine(autoChangeRoutine);
        }
    }

    private void Start()
    {
        ShowRandomCuriosity();
    }

    // Detecta toque con LeanTouch
    private void OnFingerTap(LeanFinger finger)
    {
        // Raycast a UI
        var ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                ShowRandomCuriosity();
            }
        }
    }

    // Cambia automáticamente cada 10 segundos
    private IEnumerator ChangeEvery10Seconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);
            ShowRandomCuriosity();
        }
    }

    private void ShowRandomCuriosity()
    {
        if (curiosityTexts.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, curiosityTexts.Length);
        } while (randomIndex == lastIndex && curiosityTexts.Length > 1);

        curiosityText.text = curiosityTexts[randomIndex];
        lastIndex = randomIndex;
    }
}

