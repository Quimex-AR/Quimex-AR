using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

public class TfueManager : MonoBehaviour, IPointerClickHandler
{
    public static TfueManager Instance;

    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJsonAsset;

    // [Header("Animation Manager")]
    // [SerializeField] private Animator handAnimator;

    private TextMeshProUGUI dialogueTMP;
    private Story story;

    private bool waitingForInput = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        story = new Story(inkJsonAsset.text);
        dialogueTMP = gameObject.GetComponent<TextMeshProUGUI>();
        DisplayNextLine();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogueTMP == null) return;

        if (waitingForInput)
        {
            waitingForInput = false;
            DisplayNextLine();
        }
    }

    private void DisplayNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            dialogueTMP.text = text;

            foreach (string tag in story.currentTags)
            {
                if (tag.StartsWith("animation:"))
                {
                    string anim = tag["animation:".Length..];
                    string animFunc = string.Join("",
                        anim.Split('_')
                            .Select(s => char.ToUpper(s[0]) + s.Substring(1))
                    );

                    if (animFunc != "none" || !string.IsNullOrEmpty(animFunc))
                    {
                        Debug.Log($"Triggering animation: {animFunc}");
                        ReflectionHelper.InvokeMethod(TfueAnimationsManager.Instance, animFunc);
                    }
                }
            }

            waitingForInput = true;
        }
        else if (story.currentChoices.Count > 0)
        {
            story.ChooseChoiceIndex(0);
            DisplayNextLine();
        }
        else
        {
            PlayerPrefs.SetInt("IsTutorialComplete", 1);

            ToastManager.Instance.ShowToast("Tutorial completado", 0.35f);

            if (FadeCanvas.Instance != null)
            {
                StartCoroutine(FadeCanvas.Instance.FadeToBlack());
            }

            LoadingScreenController.targetScene = "Quimex AR";
            SceneManager.LoadScene("Loading Scene");
        }
    }
}

public class ReflectionHelper
{
    public static object InvokeMethod(object targetClass, string methodName, object[] methodArguments = null)
    {
        if (targetClass == null) return null;
        if (string.IsNullOrEmpty(methodName)) return null;

        Type type = targetClass.GetType();
        MethodInfo method = type.GetMethod(methodName);

        if (method == null) return null;

        try
        {
            return method.Invoke(targetClass, methodArguments ?? new object[0]);
        }
        catch (TargetParameterCountException)
        {
            Debug.LogWarning("Arguments count mismatch");
        }
        catch (ArgumentException ex)
        {
            Debug.LogWarning($"Argment type mismatch {ex}");
        }

        return null;
    }

    public static T InvokeMethod<T>(object targetClass, string methodName, object[] methodArguments = null)
    {
        object result = InvokeMethod(targetClass, methodName, methodArguments);
        return (T)result;
    }
}
