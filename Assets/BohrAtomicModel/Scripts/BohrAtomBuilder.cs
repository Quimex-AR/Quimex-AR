using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class BohrAtomBuilder : MonoBehaviour
{
    [Header("Prefab References (Required)")]
    public GameObject electronPrefab;
    public GameObject protonPrefab;
    public GameObject neutronPrefab;

    [Header("Prefab Scale Settings")]
    public Vector3 electronScale = Vector3.one;
    public Vector3 protonScale = Vector3.one;
    public Vector3 neutronScale = Vector3.one;

    [Header("Nucleus Settings")]
    public float nucleusRadius = 0.3f;
    public int protonCount = 6;
    public int neutronCount = 6;

    [Header("Electron Shells (List of electrons per shell)")]
    public List<int> electronShells = new() { 2, 4 }; // Default: Carbon-12

    [Header("Shell Settings")]
    public float shellDistance = 1.5f;
    public float firstShellMultiplier = 1.5f;
    public int shellCircleSegments = 100;
    public Material shellMaterial;
    public Color shellColor = Color.white;
    public float shellLineWidth = 0.02f;

    [Header("Game Play settings")]
    [Header("Electron Snapper Settings")]
    public float suctionDistance = 0.135f;
    public float detachDistanceThreshold = 0.085f;
    [Header("Shell Zone Settings")]
    public float snapRadius = 0.185f;

    void Start()
    {
        if (electronPrefab == null || protonPrefab == null || neutronPrefab == null)
        {
            Debug.LogWarning("One or more prefabs are not assigned. Atom construction aborted.");
            return;
        }

        CreateNucleus();
        CreateElectronShells();
    }

    void CreateNucleus()
    {
        GameObject nucleus = new("Nucleus");
        nucleus.transform.parent = transform;
        nucleus.transform.localPosition = Vector3.zero;

        for (int i = 0; i < protonCount; i++)
        {
            GameObject p = Instantiate(protonPrefab, nucleus.transform);
            p.transform.localPosition = Random.insideUnitSphere * nucleusRadius;
            p.transform.localScale = protonScale;
        }

        for (int i = 0; i < neutronCount; i++)
        {
            GameObject n = Instantiate(neutronPrefab, nucleus.transform);
            n.transform.localPosition = Random.insideUnitSphere * nucleusRadius;
            n.transform.localScale = neutronScale;
        }
    }

    void CreateElectronShells()
    {
        float lastShellRadius = nucleusRadius * firstShellMultiplier;

        for (int shellIndex = 0; shellIndex < electronShells.Count; shellIndex++)
        {
            int electronsInShell = electronShells[shellIndex];
            float radius;

            if (shellIndex == 0)
            {
                radius = lastShellRadius;
            }
            else
            {
                radius = lastShellRadius + shellDistance;
                lastShellRadius = radius;
            }

            float angleStep = 360f / electronsInShell;

            GameObject shell = new($"Shell_{shellIndex + 1}");
            shell.transform.parent = transform;
            shell.transform.localPosition = Vector3.zero;

            DrawShellCircle(shell.transform, radius, electronsInShell);

            for (int e = 0; e < electronsInShell; e++)
            {
                float angle = e * angleStep * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

                // Electron Instantiation
                GameObject electron = Instantiate(electronPrefab, shell.transform);
                electron.transform.localPosition = position;
                electron.transform.localScale = electronScale;

                // Add interactable components to electron
                Rigidbody2D rb = electron.GetComponent<Rigidbody2D>();
                LeanSelectableByFinger lsbf = electron.GetComponent<LeanSelectableByFinger>();
                LeanDragTranslate ldt = electron.AddComponent<LeanDragTranslate>();
                ldt.Use.RequiredSelectable = lsbf;

                // Add interaction events
                lsbf.OnSelected.AddListener(_ => rb.bodyType = RigidbodyType2D.Kinematic);
                lsbf.OnDeselected.AddListener(_ => rb.bodyType = RigidbodyType2D.Dynamic);

                // Add suction snapper
                ElectronSnapper es = electron.AddComponent<ElectronSnapper>();
                es.suctionDistance = suctionDistance;
                es.detachDistanceThreshold = detachDistanceThreshold;
            }
        }
    }

    void DrawShellCircle(Transform parent, float radius, int electronsInShell)
    {
        GameObject circleObj = new("ShellCircle");
        circleObj.transform.parent = parent;
        circleObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = shellCircleSegments + 1;
        lr.material = shellMaterial;
        lr.startColor = shellColor;
        lr.endColor = shellColor;
        lr.startWidth = shellLineWidth;
        lr.endWidth = shellLineWidth;

        for (int i = 0; i <= shellCircleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / shellCircleSegments;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            lr.SetPosition(i, pos);
        }

        ShellZone zone = parent.gameObject.AddComponent<ShellZone>();
        zone.shellRadius = radius;
        zone.maxElectrons = electronsInShell;
        zone.snapRadius = snapRadius;
    }
}
