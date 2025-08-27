using UnityEngine;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Handles spawning prefabs when Vuforia image targets are detected.
/// Can spawn either attached to the image target (real AR)
/// or as a sticky object in the middle of the screen (stable mode).
/// </summary>
public class DynamicModelLoader : MonoBehaviour
{
    [System.Serializable]
    public class TargetPrefabPair
    {
        [Tooltip("The Vuforia TargetName (from your Vuforia database).")]
        public string targetName;
        [Tooltip("Prefab to spawn when this target is recognized.")]
        public GameObject prefab;
    }

    [Header("Target → Prefab Mappings")]
    [SerializeField] private List<TargetPrefabPair> targetPrefabPairs = new();

    private Dictionary<string, GameObject> targetPrefabMap;

    [Header("Runtime State (Debug)")]
    [Tooltip("The currently spawned instance (if any).")]
    [SerializeField] private GameObject spawnedInstance;
    [Tooltip("The name of the current target (if any).")]
    [SerializeField] private string currentTarget;

    public static DynamicModelLoader Instance { get; private set; }

    private static bool isAnyModelActive = false;

    [Header("Sticky (Stable) Placement")]
    [Tooltip("Default sticky mode on start (can be changed at runtime via the static StickyMode).")]
    [SerializeField] private bool defaultStickyMode = false;

    /// <summary>
    /// Static toggle. Set from other scripts: DynamicModelLoader.StickyMode = true;
    /// True => spawn object anchored to center of screen (sticky stable placement).
    /// False => spawn as child of the detected ImageTarget (normal AR).
    /// </summary>
    public static bool StickyMode { get; set; }
    public float stickyDistance = 1.0f;

    [Tooltip("Rotation offset (in degrees) applied to the spawned object in StickyMode.")]
    public Vector3 stickySpawnRotationOffset = Vector3.zero;

    [Tooltip("How fast the sticky anchor follows the camera center (higher = snappier).")]
    public float stickyFollowSpeed = 10f;

    [Tooltip("If true, sticky spawned object will face the camera.")]
    public bool stickyFaceCamera = true;

    [Tooltip("If true, sticky anchor will NOT update its rotation each frame (recommended).")]
    [SerializeField] private bool stickyLockRotation = true;

    [Tooltip("If true and stickyLockRotation == false, only yaw will be used when facing the camera.")]
    [SerializeField] private bool stickyYawOnly = true;

    // Internal anchor (world object; not parented to camera).
    private Transform stickyAnchor;
    private Quaternion stickyAnchorInitialRotation = Quaternion.identity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[DynamicModelLoader] No instance found");
        }

        Instance = this;
        StickyMode = defaultStickyMode;

        BuildDictionary();
    }

    private void Start()
    {
        foreach (var observer in FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None))
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        EnsureStickyAnchorExists();
    }

    private void OnDestroy()
    {
        foreach (var observer in FindObjectsByType<ObserverBehaviour>(FindObjectsSortMode.None))
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        if (stickyAnchor != null && StickyMode)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                Vector3 targetPos = cam.transform.position + cam.transform.forward * stickyDistance;
                stickyAnchor.position = Vector3.Lerp(stickyAnchor.position, targetPos, Time.deltaTime * stickyFollowSpeed);

                if (!stickyLockRotation)
                {
                    if (stickyFaceCamera)
                    {
                        Vector3 lookDir = stickyAnchor.position - cam.transform.position;
                        if (stickyYawOnly)
                        {

                            lookDir.y = 0f;
                        }

                        if (lookDir.sqrMagnitude > 0.001f)
                        {
                            stickyAnchor.rotation = Quaternion.Lerp(
                                stickyAnchor.rotation,
                                Quaternion.LookRotation(lookDir.normalized, Vector3.up),
                                Time.deltaTime * stickyFollowSpeed
                            );
                        }
                    }
                    else
                    {
                        stickyAnchor.rotation = Quaternion.Lerp(
                            stickyAnchor.rotation,
                            stickyAnchorInitialRotation,
                            Time.deltaTime * stickyFollowSpeed
                        );

                    }
                }
                else
                {
                    stickyAnchor.rotation = stickyAnchorInitialRotation;
                }
            }
        }
    }

    private void BuildDictionary()
    {
        targetPrefabMap = new Dictionary<string, GameObject>();
        foreach (var pair in targetPrefabPairs)
        {
            if (!string.IsNullOrEmpty(pair.targetName) && pair.prefab != null)
            {
                if (!targetPrefabMap.ContainsKey(pair.targetName))
                {
                    Debug.Log($"[DynamicModelLoader] Target name added: {pair.targetName}");
                    targetPrefabMap.Add(pair.targetName, pair.prefab);
                }
                else
                {
                    Debug.LogWarning($"[DynamicModelLoader] Duplicate target name found: {pair.targetName}. Ignoring duplicate.");
                }
            }
        }
    }

    private void EnsureStickyAnchorExists()
    {
        if (stickyAnchor != null) return;

        var go = GameObject.Find("DynamicModelLoader_StickyAnchor");
        if (go == null)
        {
            go = new GameObject("DynamicModelLoader_StickyAnchor");
        }

        stickyAnchor = go.transform;

        var cam = Camera.main;
        if (cam != null)
        {
            stickyAnchor.position = cam.transform.position + cam.transform.forward * stickyDistance;
        }

        stickyAnchor.rotation = Quaternion.identity;
        stickyAnchorInitialRotation = stickyAnchor.rotation;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        string targetName = behaviour.TargetName;

        if (ShouldSpawn(targetName, targetStatus))
        {
            SpawnModel(targetName, behaviour);
        }
        else if (ShouldDestroy(targetName, targetStatus))
        {
            DestroyModel();
        }
    }

    /// <summary>
    /// Decide if a model should be spawned
    /// </summary>
    private bool ShouldSpawn(string targetName, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL)
        {
            return !isAnyModelActive && spawnedInstance == null && targetPrefabMap.ContainsKey(targetName);
        }

        return false;
    }

    /// <summary>
    /// Decide if a model should be destroyed
    /// </summary>
    private bool ShouldDestroy(string targetName, TargetStatus status)
    {
        if (spawnedInstance == null) return false;

        if (StickyMode)
        {
            return status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL && targetName != currentTarget;
        }
        else
        {
            return status.Status == Status.NO_POSE || status.Status == Status.EXTENDED_TRACKED;
        }
    }

    /// <summary>
    /// Spawn prefab either in sticky mode or AR mode
    /// </summary>
    private void SpawnModel(string targetName, ObserverBehaviour behaviour)
    {
        if (StickyMode)
        {
            EnsureStickyAnchorExists();
            if (stickyAnchor == null)
            {
                Debug.LogWarning("[DynamicModelLoader] Sticky anchor missing. Reverting to AR fallback mode.");
                SetStickyMode(false);
                spawnedInstance = Instantiate(targetPrefabMap[targetName], behaviour.transform);
            }
            else
            {
                spawnedInstance = Instantiate(targetPrefabMap[targetName], stickyAnchor);
                spawnedInstance.transform.localPosition = Vector3.zero;
                spawnedInstance.transform.localRotation = Quaternion.Euler(stickySpawnRotationOffset);
            }
        }
        else
        {
            spawnedInstance = Instantiate(targetPrefabMap[targetName], behaviour.transform);
        }

        currentTarget = targetName;
        isAnyModelActive = true;
    }

    /// <summary>
    /// Destroy currently spawned model
    /// </summary>
    private void DestroyModel()
    {
        if (spawnedInstance != null)
        {
            Destroy(spawnedInstance);
            spawnedInstance = null;
            currentTarget = null;
            isAnyModelActive = false;
        }
    }

    /// <summary>
    /// Toggle sticky mode programmatically.
    /// Example: DynamicModelLoader.Instance?.SetStickyMode(true);
    /// or set the static flag directly: DynamicModelLoader.StickyMode = true;
    /// </summary>
    public void SetStickyMode(bool enabled)
    {
        StickyMode = enabled;
        if (enabled) EnsureStickyAnchorExists();
    }

    // <summary>
    /// Gets the transform that serves as the anchor for sticky objects in the scene.
    /// Use this property to access the anchor's position, rotation, or scale.
    /// </summary>
    public Transform StickyAnchorTransform => stickyAnchor;
}
