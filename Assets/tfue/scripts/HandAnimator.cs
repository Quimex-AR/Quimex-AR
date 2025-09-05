using UnityEngine;

[System.Serializable]
public class HandAnimator
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject model;

    public GameObject Model
    {
        get => model;
        set => model = value;
    }

    public Animator Animator
    {
        get => animator;
        set => animator = value;
    }

    public void SetHandModelPosition(float x, float y, float z)
    {
        model.transform.localPosition = new Vector3(x, y, z);
    }

    public void SetHandModelRotation(float x, float y, float z)
    {
        model.transform.rotation = Quaternion.Euler(x, y, z);
    }
    public void SetHandModelScale(float x, float y, float z)
    {
        model.transform.localScale = new Vector3(x, y, z);
    }
}
