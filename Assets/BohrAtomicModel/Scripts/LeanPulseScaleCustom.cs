using UnityEngine;

namespace Lean.Touch
{
    public class LeanPulseScaleCustom : LeanPulseScale
    {
        private float lastPulseInterval;
        private bool pulseActive = true;
        private bool transitioningToBaseScale = false;

        /// <summary>
        /// Stops the pulsing effect.
        /// </summary>
        public void StopPulse()
        {
            if (pulseActive)
            {
                lastPulseInterval = PulseInterval;
                PulseInterval = Mathf.Infinity;
                pulseActive = false;
            }
        }

        /// <summary>
        /// Resumes the pulsing effect.
        /// </summary>
        public void ResumePulse()
        {
            if (!pulseActive)
            {
                transitioningToBaseScale = true;
                pulseActive = true;
            }
        }

        protected override void Update()
        {
            if (!pulseActive) return; // Exit early avoiding pinch to scale conflict.

            // Make transition to return to base scale.
            if (transitioningToBaseScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, BaseScale, Time.deltaTime * Damping);

                if (Vector3.Distance(transform.localScale, BaseScale) < 0.01f)
                {
                    transform.localScale = BaseScale;
                    transitioningToBaseScale = false;
                    PulseInterval = lastPulseInterval;
                }

                return; // Exit early to initialize the pulse in the next call.
            }

            base.Update(); // Pulse Logic from parent `LeanPulseScale` class
        }
    }
}
