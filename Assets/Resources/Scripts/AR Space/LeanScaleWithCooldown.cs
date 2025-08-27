using UnityEngine;
using CW.Common;

namespace Lean.Touch
{
    [AddComponentMenu(LeanTouch.ComponentPathPrefix + "Scale With Cooldown")]
    public class LeanScaleWithCooldown : LeanPinchScale
    {
        public LeanPinchScale pinchScaler;

        [Header("Cooldown Settings")]
        public Vector3 baseScale = Vector3.one;

        public float cooldownTime = 2f;

        public float returnSpeed = 3f;

        private float lastPinchTime;

        protected override void Update()
        {
            base.Update();

            if (LeanTouch.Fingers.Count > 1)
            {
                lastPinchTime = Time.time;
            }

            if (Time.time > lastPinchTime + cooldownTime)
            {
                transform.localScale = Vector3.Lerp(
                    transform.localScale,
                    baseScale,
                    Time.deltaTime + returnSpeed
                );
            }
        }
    }
}


#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
    using UnityEditor;
    using TARGET = LeanScaleWithCooldown;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET), true)]
    public class LeanScaleWithCooldown_Editor : CwEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

            Draw("baseScale", "The original/base scale to return to after cooldown.");
            Draw("cooldownTime", "How long (seconds) after last pinch input before resetting scale.");
            Draw("returnSpeed", "How fast to return to base scale after cooldown.");
        }
    }
}
#endif
