using UnityEngine;
using CW.Common;

namespace Lean.Touch
{
    [AddComponentMenu(LeanTouch.ComponentPathPrefix + "Lean One Finger Rotate Y")]
    public class LeanOneFingerRotateY : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Range(0.1f, 10f)]
        public float rotationSensivity = 1.0f;

        [Header("Runtime Debug")]
        [Tooltip("The finger currently rotating this object.")]
        public static LeanFinger activeFinger;

        private const float PinchTolerance = 0.02f;

        void OnEnable()
        {
            LeanTouch.OnFingerDown += HandleFingerDown;
            LeanTouch.OnFingerUp += HandleFingerUp;
            LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        }

        void OnDisable()
        {
            LeanTouch.OnFingerDown -= HandleFingerDown;
            LeanTouch.OnFingerUp -= HandleFingerUp;
            LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        }

        private void HandleFingerDown(LeanFinger finger)
        {
            if (activeFinger != null) return;
            if (finger.IsOverGui) return;

            var ray = finger.GetRay();
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    Debug.Log($"[LeanOneFingerRotateY] Finger assigned activeFinger");
                    activeFinger = finger;
                }
            }
        }

        private void HandleFingerUp(LeanFinger finger)
        {
            if (finger == activeFinger)
            {
                Debug.Log($"[LeanOneFingerRotateY] Finger activeFinger remove");
                activeFinger = null;
            }
        }

        private void HandleFingerUpdate(LeanFinger finger)
        {

            if (finger != activeFinger) return;
            if (finger.IsOverGui) return;

            var pinchScale = LeanGesture.GetPinchScale(LeanTouch.Fingers);
            if (Mathf.Abs(pinchScale - 1.0f) > PinchTolerance) return;

            float delta = finger.ScreenDelta.x;

            bool isUnderSticky = false;
            if (DynamicModelLoader.Instance != null && DynamicModelLoader.Instance.StickyAnchorTransform != null)
                isUnderSticky = transform.IsChildOf(DynamicModelLoader.Instance.StickyAnchorTransform);

            if (isUnderSticky)
            {
                transform.Rotate(Vector3.up, -delta * rotationSensivity, Space.World);
            }
            else
            {

                transform.Rotate(Vector3.up, -delta * rotationSensivity, Space.Self);
            }

            Debug.Log($"[LeanOneFingerRotateY] Rotated by {-delta * rotationSensivity:F3} degrees (total rotation {transform.localEulerAngles.y:F2})");
        }
    }
}


#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
    using UnityEditor;
    using TARGET = LeanOneFingerRotateY;

    [CustomEditor(typeof(TARGET), true)]
    public class LeanOneFingerRotateY_Editor : CwEditor
    {
        private static GUIStyle fadingLabel;

        protected override void OnInspector()
        {
            TARGET tgt; TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("rotationSensivity", "How sensitive the rotation should be when dragging horizontally with one finger.");

            if (TARGET.activeFinger != null)
            {
                var progress = LeanTouch.CurrentTapThreshold > 0.0f ? TARGET.activeFinger.Age / LeanTouch.CurrentTapThreshold : 0.0f;
                var style = GetFadingLabel(TARGET.activeFinger.Set, progress);
                EditorGUILayout.LabelField($"[{TARGET.activeFinger.Index}]: Taps: {TARGET.activeFinger.TapCount} - Age: {TARGET.activeFinger.Age:0.0}", style);
            }
            else
            {
                EditorGUILayout.LabelField("No active finger.", EditorStyles.label);
            }
        }
        private static GUIStyle GetFadingLabel(bool active, float progress)
        {
            if (fadingLabel == null)
            {
                fadingLabel = new GUIStyle(EditorStyles.label);
            }

            var a = EditorStyles.label.normal.textColor;
            var b = a; b.a = active == true ? 0.5f : 0.0f;

            fadingLabel.normal.textColor = Color.Lerp(a, b, progress);

            return fadingLabel;
        }
    }
}
#endif
