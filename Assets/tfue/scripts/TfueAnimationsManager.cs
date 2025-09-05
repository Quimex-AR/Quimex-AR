using System;
using System.Collections;
using UnityEngine;

public class TfueAnimationsManager : MonoBehaviour
{
    public static TfueAnimationsManager Instance;

    [Header("Animators")]
    [SerializeField] private HandAnimator handAnimator;
    [SerializeField] private AtomicModelAnimator atomicModelAnimator;

    [Header("Configurations")]
    [Range(0f, 3f)]
    [SerializeField] private float timeBetweenAnimationLoops = 1f;

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
        atomicModelAnimator.AtomicModelBaseRotation = atomicModelAnimator.AtomicModel.transform.rotation;
        atomicModelAnimator.AtomicModelBaseLocalScale = atomicModelAnimator.AtomicModel.transform.localScale;

        atomicModelAnimator.ImageTargetHide();
        atomicModelAnimator.TooltipHide();
        atomicModelAnimator.AtomicModelHide();
        atomicModelAnimator.PlateAtomicModelNameHide();
        atomicModelAnimator.PlateGameHide();
        atomicModelAnimator.CanvasInfoHide();
    }

    #region Animations
    public void HandSalutation()
    {
        handAnimator.SetHandModelPosition(-0.04f, -2f, -6.09f);
        handAnimator.SetHandModelRotation(-14.083f, 180.347f, -0.537f);
        handAnimator.SetHandModelScale(0.1f, 0.1f, 0.1f);
        handAnimator.Animator.SetTrigger("salutation");
    }
    public void ImageTargetShow()
    {
        handAnimator.Model.SetActive(false);
        atomicModelAnimator.ImageTargetShow();
    }
    public void AtomicModelShow()
    {
        atomicModelAnimator.AtomicModelShow();

        atomicModelAnimator.TooltipHide();
        atomicModelAnimator.PlateAtomicModelNameHide();
        atomicModelAnimator.PlateGameHide();
        atomicModelAnimator.CanvasInfoHide();
    }
    public void HandSwipe()
    {
        handAnimator.Model.SetActive(true);
        handAnimator.SetHandModelPosition(0.69f, -1.02f, -7.58f);
        handAnimator.SetHandModelRotation(-13.57f, 169.803f, -14.981f);
        handAnimator.SetHandModelScale(0.05f, 0.05f, 0.05f);
        handAnimator.Animator.SetTrigger("swipe");
        PlaySpinLoop();
    }
    public void HandZoomOut()
    {
        StopSpinLoop();
        atomicModelAnimator.AtomicModel.transform.rotation = atomicModelAnimator.AtomicModelBaseRotation;

        handAnimator.SetHandModelPosition(0.438f, -0.815f, -7.429f);
        handAnimator.SetHandModelRotation(1.155f, 183.989f, -25.317f);
        handAnimator.Animator.SetTrigger("zoom in");
        PlayScaleLoop(1f, 0.5f);
    }

    public void HandZoomIn()
    {
        StopSpinLoop();
        atomicModelAnimator.AtomicModel.transform.localScale = atomicModelAnimator.AtomicModelBaseLocalScale;

        handAnimator.SetHandModelPosition(0.364f, -0.934f, -7.753f);
        handAnimator.SetHandModelRotation(-13.57f, 144.381f, -14.981f);
        handAnimator.Animator.SetTrigger("zoom out");
        PlayScaleLoop(1f, 3f);
    }
    public void AtomicModelBackToNormal()
    {
        handAnimator.Model.SetActive(false);

        StopScaleLoop();
        atomicModelAnimator.AtomicModel.transform.localScale = new Vector3(3f, 3f, 3f);

        StartLoopedAnimation(
            () => atomicModelAnimator.ScaleAtomicModelCoroutine(3f, 1f),
            timeBetweenAnimationLoops,
            ref animationRoutine,
            animationFlag,
            () =>
            {
                atomicModelAnimator.AtomicModel.transform.localScale = new Vector3(3f, 3f, 3f);
            }
        );
    }
    public void AtomicModelTooltips()
    {
        StopScaleLoop();
        atomicModelAnimator.AtomicModel.transform.localScale = atomicModelAnimator.AtomicModelBaseLocalScale;

        atomicModelAnimator.TooltipShow();
    }
    public void AtomicModelTooltipsInformationShow()
    {
        handAnimator.Model.SetActive(true);
        handAnimator.SetHandModelPosition(0.811f, -0.53f, -7.23f);
        handAnimator.SetHandModelRotation(-17.176f, 143.661f, -12.267f);
        handAnimator.SetHandModelScale(0.06f, 0.06f, 0.06f);

        animationFlag.Value = true;
        animationRoutine = StartCoroutine(PlayCanvasInfoShowLoop());
    }
    public void AtomicModelPlatesShow()
    {
        StopCanvasInfoShowLoop();

        atomicModelAnimator.CanvasInfoHide();
        atomicModelAnimator.PlateAtomicModelNameShow();
        atomicModelAnimator.PlateGameShow();

    }
    public void AtomicModelPlatesModelNameShow()
    {
        atomicModelAnimator.PlateGameHide();
    }
    public void AtomicModelPlatesGameShow()
    {
        atomicModelAnimator.PlateAtomicModelNameHide();
        atomicModelAnimator.PlateGameShow();
    }
    public void HandAdieu()
    {
        atomicModelAnimator.PlateAtomicModelNameShow();
        handAnimator.Model.SetActive(true);
        handAnimator.SetHandModelPosition(-0.04f, -1.214f, -7.919f);
        handAnimator.SetHandModelRotation(-14.083f, 180.347f, -0.537f);
        handAnimator.SetHandModelScale(0.05f, 0.05f, 0.05f);
        handAnimator.Animator.SetTrigger("salutation");
    }
    #endregion

    #region Animations Helpers 
    private Coroutine animationRoutine;
    private BoolFlag animationFlag = new();

    private void PlaySpinLoop()
    {
        StartLoopedAnimation(
            () => atomicModelAnimator.SpinAtomicModelCoroutine(),
            timeBetweenAnimationLoops,
            ref animationRoutine,
            animationFlag,
            () =>
            {
                atomicModelAnimator.AtomicModel.transform.rotation = atomicModelAnimator.AtomicModelBaseRotation;
            }
        );
    }
    private void StopSpinLoop()
    {
        animationFlag.Value = false;
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        atomicModelAnimator.AtomicModel.transform.rotation = atomicModelAnimator.AtomicModelBaseRotation;
    }

    private void PlayScaleLoop(float startScale, float endScale) // bigger
    {
        StartLoopedAnimation(
            () => atomicModelAnimator.ScaleAtomicModelCoroutine(startScale, endScale),
            timeBetweenAnimationLoops,
            ref animationRoutine,
            animationFlag,
            () =>
            {
                atomicModelAnimator.AtomicModel.transform.localScale = atomicModelAnimator.AtomicModelBaseLocalScale;
            }
        );
    }
    private void StopScaleLoop()
    {
        animationFlag.Value = false;
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        atomicModelAnimator.AtomicModel.transform.localScale = atomicModelAnimator.AtomicModelBaseLocalScale;
    }
    public void StopCanvasInfoShowLoop()
    {
        animationFlag.Value = false;
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        if (handAnimator != null && handAnimator.Model != null)
            handAnimator.Model.SetActive(false);
        atomicModelAnimator.CanvasInfoHide();
    }
    private IEnumerator PlayCanvasInfoShowLoop()
    {
        if (handAnimator == null || atomicModelAnimator == null) yield break;

        Animator handAnim = handAnimator.Animator;
        GameObject handModel = handAnimator.Model;

        if (handModel == null || handAnim == null) yield break;

        while (animationFlag.Value)
        {
            handModel.SetActive(true);
            handAnim.ResetTrigger("touch");
            handAnim.SetTrigger("touch");

            float clipLength = GetAnimationClipLength(handAnim, "touch", 1f);
            float waitForTouch = clipLength * 1;
            if (waitForTouch <= 0.01f) waitForTouch = 1f * 1;

            yield return new WaitForSeconds(waitForTouch);

            atomicModelAnimator.CanvasInfoShow();
            handModel.SetActive(false);

            yield return new WaitForSeconds(2.0f);

            atomicModelAnimator.CanvasInfoHide();

            if (timeBetweenAnimationLoops > 0f)
                yield return new WaitForSeconds(timeBetweenAnimationLoops);
        }

        handModel.SetActive(false);
        atomicModelAnimator.CanvasInfoHide();
        animationRoutine = null;
    }
    private float GetAnimationClipLength(Animator animator, string clipName, float fallback)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return fallback;

        var clips = animator.runtimeAnimatorController.animationClips;
        if (clips == null || clips.Length == 0)
            return fallback;

        foreach (var c in clips)
        {
            if (string.Equals(c.name, clipName, StringComparison.OrdinalIgnoreCase))
                return c.length;
        }

        foreach (var c in clips)
        {
            if (c.name.IndexOf(clipName, StringComparison.OrdinalIgnoreCase) >= 0)
                return c.length;
        }

        return clips.Length > 0 ? clips[0].length : fallback;
    }

    /// Starts a looped animation coroutine that repeatedly executes a given animation as long as a flag remains true.
    /// 
    /// Lifecycle:
    ///       BoolFlag: Acts as a control switch for the loop. When its Value is true, the loop continues; when set to false, the loop stops.
    ///       StartLoopedAnimation: 
    ///         If a previous coroutine handle exists, it stops that coroutine to prevent multiple loops.
    ///         Sets flag.Value to true to start the loop.
    ///         Starts the LoopRoutine coroutine and stores its handle.
    ///       LoopRoutine:
    ///         Runs a while loop as long as flag.Value is true.
    ///         Each iteration yields the provided animation enumerator.
    ///         Execute block of code before the wait time.
    ///         If wait is greater than zero, yields a WaitForSeconds to pause between loops.
    ///         When flag.Value is set to false (externally), the loop exits and the coroutine ends.
    class BoolFlag { public bool Value; }

    private Coroutine StartLoopedAnimation(Func<IEnumerator> animationFactory, float wait, ref Coroutine handle, BoolFlag flag, Action betweenLoops = null)
    {
        if (handle != null) StopCoroutine(handle);

        flag.Value = true;
        handle = StartCoroutine(LoopRoutine(animationFactory, wait, flag, betweenLoops));

        return handle;
    }

    private IEnumerator LoopRoutine(Func<IEnumerator> animationFactory, float wait, BoolFlag flag, Action betweenLoops)
    {
        int temp = 1;

        while (flag.Value)
        {
            yield return animationFactory();

            betweenLoops?.Invoke();

            if (wait > 0f) yield return new WaitForSeconds(wait);

            temp++;
        }
    }
    #endregion
}
