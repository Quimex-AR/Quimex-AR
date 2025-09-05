using System;
using System.Collections;
using UnityEngine;

public class TfueAnimationsManager : MonoBehaviour
{
    public static TfueAnimationsManager Instance;

    [Header("Animators")]
    [SerializeField] private Animator handAnimator;
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
        // PlaySpinLoop();
        // PlayScaleLoop(1f, 0.5f);
        // PlayScaleLoop(1f, 3f);
    }

    #region Animations
    public void HandSalutation() { Debug.Log(nameof(HandSalutation)); }
    public void ImageTargetShow() { Debug.Log(nameof(ImageTargetShow)); }
    public void AtomicModelShow() { Debug.Log(nameof(AtomicModelShow)); }
    public void HandSwipe() { Debug.Log(nameof(HandSwipe)); }
    public void HandZoom() { Debug.Log(nameof(HandZoom)); }
    public void AtomicModelBackToNormal() { Debug.Log(nameof(AtomicModelBackToNormal)); }
    public void AtomicModelTooltips() { Debug.Log(nameof(AtomicModelTooltips)); }
    public void AtomicModelTooltipsInformationShow() { Debug.Log(nameof(AtomicModelTooltipsInformationShow)); }
    public void AtomicModelPlatesModelNameShow() { Debug.Log(nameof(AtomicModelPlatesModelNameShow)); }
    public void AtomicModelPlatesGameShow() { Debug.Log(nameof(AtomicModelPlatesGameShow)); }
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
