using System.Collections;
using UnityEngine;

[System.Serializable]
public class AtomicModelAnimator
{
    [SerializeField] private GameObject imageTarget;
    [SerializeField] private GameObject atomicModel;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private GameObject plateAtomicModelName;
    [SerializeField] private GameObject plateGame;
    [SerializeField] private GameObject canvasInfo;

    private Quaternion atomicModelBaseRotation;
    private Vector3 atomicModelBaseLocalScale;

    public GameObject AtomicModel
    {
        get => atomicModel;
        set => atomicModel = value;
    }

    public Quaternion AtomicModelBaseRotation
    {
        get => atomicModelBaseRotation;
        set => atomicModelBaseRotation = value;
    }

    public Vector3 AtomicModelBaseLocalScale
    {
        get => atomicModelBaseLocalScale;
        set => atomicModelBaseLocalScale = value;
    }

    public void ImageTargetShow()
    {
        if (imageTarget != null)
        {
            imageTarget.SetActive(true);
        }
    }

    public void ImageTargetHide()
    {
        if (imageTarget != null)
        {
            imageTarget.SetActive(false);
        }
    }

    public void TooltipShow()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(true);
        }
    }

    public void TooltipHide()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }

    public void PlateAtomicModelNameShow()
    {
        if (plateAtomicModelName != null)
        {
            plateAtomicModelName.SetActive(true);
        }
    }

    public void PlateAtomicModelNameHide()
    {
        if (plateAtomicModelName != null)
        {
            plateAtomicModelName.SetActive(false);
        }
    }

    public void PlateGameShow()
    {
        if (plateGame != null)
        {
            plateGame.SetActive(true);
        }
    }

    public void PlateGameHide()
    {
        if (plateGame != null)
        {
            plateGame.SetActive(false);
        }
    }

    public void AtomicModelShow()
    {
        if (atomicModel != null)
        {
            atomicModel.SetActive(true);
        }
    }

    public void AtomicModelHide()
    {
        if (atomicModel != null)
        {
            atomicModel.SetActive(false);
        }
    }

    public void CanvasInfoShow()
    {
        if (canvasInfo != null)
        {
            canvasInfo.SetActive(true);
        }
    }

    public void CanvasInfoHide()
    {
        if (canvasInfo != null)
        {
            canvasInfo.SetActive(false);
        }
    }

    public IEnumerator SpinAtomicModelCoroutine()
    {
        float duration = 3f;
        float elapsed = 0f;
        float startSpeed = 30f;
        float endSpeed = 480f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease-in: accelerate rotation speed
            float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t * t);
            atomicModel.transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime, Space.Self);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ScaleAtomicModelCoroutine(float startScale, float endScale)
    {
        float duration = 2f;
        float elapsed = 0f;
        float startSpeed = 1f;
        float endSpeed = 0.6f;

        Vector3 initialScale = atomicModel.transform.localScale;
        Vector3 targetScale = initialScale * (endScale / startScale);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease-in: accelerate scaling speed at start, slower at end
            float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t * t);
            float scaleFactor = Mathf.Lerp(startScale, endScale, t);
            atomicModel.transform.localScale = initialScale * (scaleFactor / startScale);
            elapsed += Time.deltaTime * currentSpeed;
            yield return null;
        }
        atomicModel.transform.localScale = targetScale;
    }
}
