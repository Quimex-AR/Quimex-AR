using UnityEngine;
using UnityEngine.SceneManagement;
using Lean.Touch;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attach to each tooltip collider (box). Stores title + body and notifies TooltipManager on tap.
/// </summary>
[AddComponentMenu("AR/Tooltip Trigger")]
public class TooltipModalTrigger : MonoBehaviour
{
    [Header("Tooltip Content")]
    [Tooltip("Title shown in the modal")]
    public string title;

    [TextArea(3, 10)]
    [Tooltip("Body text shown in the modal")]
    public string body;

    [Header("Raycast Settings")]
    [Tooltip("LayerMask to use for raycast. Set to the layer(s) your tooltip colliders are on.")]
    public LayerMask raycastMask = ~0;


    [Header("Scene Trigger Settings")]
    [Tooltip("If true, this trigger will load a new scene instead of showing the modal.")]
    public bool isSceneTrigger;

    [Tooltip("The name of the scene to load (only used if Is Scene Trigger is true).")]
    public string sceneName;

    private bool isLoading = false;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    private void HandleFingerTap(LeanFinger finger)
    {
        // Ignore taps that started over UI (so modal buttons don't re-open)
        if (finger.IsOverGui) return;

        var ray = finger.GetRay();
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask))
        {
            // Accept hits on this transform or any child
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (isSceneTrigger && !string.IsNullOrEmpty(sceneName))
                {
                    if (!isLoading)
                    {
                        StartCoroutine(StartGame(sceneName));
                    }
                }
                else
                {
                    TooltipModalManager.Instance?.ShowTooltip(this);
                }
            }
        }
    }

    private IEnumerator StartGame(string sceneName)
    {
        isLoading = true;

        yield return new WaitForSeconds(1.5f);

        if (FadeCanvas.Instance != null)
        {
            yield return StartCoroutine(FadeCanvas.Instance.FadeToBlack());
        }

        LoadingScreenController.targetScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }
}


#if UNITY_EDITOR
/// <summary>
/// Custom inspector to hide 'sceneName' unless 'isSceneTrigger' is checked.
/// </summary>
[CustomEditor(typeof(TooltipModalTrigger))]
public class TooltipModalTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("title"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("body"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastMask"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isSceneTrigger"));

        if (serializedObject.FindProperty("isSceneTrigger").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneName"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
