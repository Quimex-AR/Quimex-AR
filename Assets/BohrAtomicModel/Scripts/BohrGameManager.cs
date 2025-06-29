using System.Collections;
using Lean.Touch;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BohrGameManager : MonoBehaviour
{
    [Header("Game Logic Settings")]
    public Material correctMaterial;
    public Material incorrectMaterial;
    public Material blinkMaterial;
    public float blinkDuration = 2f;
    public float blinkInterval = 2f;
    public float WinCheckDelay = 2f;

    [Header("UI References")]
    public Button returnBtn;
    public Button restart;
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;
    public GameObject loadingContainer; // In scene this is deactivated (have TextMeshProGUI and Loading Model with animation [spinner like])


    [Header("Prefab References")]
    public GameObject easyPrefab;
    public GameObject mediumPrefab;
    public GameObject hardPrefab;


    private GameObject currentAtomInstance;
    private ElectronSnapper[] allElectrons;
    private ShellZone[] allZones;
    private bool gameEnded = false;

    void Start()
    {
        if (FadeCanvas.Instance != null)
        {
            StartCoroutine(FadeCanvas.Instance.FadeAndHide());
        }

        // Hook up button events
        easyBtn.onClick.AddListener(() => LoadAtom(easyPrefab, easyBtn));
        mediumBtn.onClick.AddListener(() => LoadAtom(mediumPrefab, mediumBtn));
        hardBtn.onClick.AddListener(() => LoadAtom(hardPrefab, hardBtn));
        restart.onClick.AddListener(Restart);
        returnBtn.onClick.AddListener(returnBtn.GetComponent<ReturnUIButton>().OnRestartClicked);

        // Start with loading spinner and restart hidden
        loadingContainer.SetActive(false);
        restart.gameObject.SetActive(false);
    }

    void LoadAtom(GameObject atomPrefab, Button selectedButton)
    {
        StartCoroutine(SpawnAtomRoutine(atomPrefab, selectedButton));
    }

    IEnumerator SpawnAtomRoutine(GameObject atomPrefab, Button selectedButton)
    {
        // UI state before load
        loadingContainer.SetActive(true);
        HideAllButtons();

        // Clear previous atom if exists
        if (currentAtomInstance != null)
        {
            Destroy(currentAtomInstance);
            yield return new WaitForSeconds(0.3f);
        }

        // Simulate loading delay
        yield return new WaitForSeconds(1f);

        // Instantiate atom
        currentAtomInstance = Instantiate(atomPrefab, transform.position, transform.rotation);
        BohrAtomPhysicsController.isGameReady = false;
        StartCoroutine(SetupAtomReferencesAfterDelay());

        // UI state after load
        loadingContainer.SetActive(false);
        restart.gameObject.SetActive(true);
        returnBtn.GetComponent<ReturnUIButton>().ShowButton();
        selectedButton.GetComponent<DifficultyUIBtn>().ShowButton();
        selectedButton.GetComponent<DifficultyUIBtn>().DisableButton();
    }

    IEnumerator SetupAtomReferencesAfterDelay()
    {
        yield return new WaitForSeconds(WinCheckDelay);

        allElectrons = currentAtomInstance.GetComponentsInChildren<ElectronSnapper>();
        allZones = currentAtomInstance.GetComponentsInChildren<ShellZone>();

        gameEnded = false;
        Debug.Log("SetupAtomReferencesAfterDelay");
    }

    void Update()
    {
        if (!gameEnded && BohrAtomPhysicsController.isGameReady && allElectrons != null)
        {
            if (AllElectronsSnapped())
            {
                gameEnded = true;
                StartCoroutine(EvaluateWinCondition());
            }
        }
    }

    void Restart()
    {
        if (currentAtomInstance != null)
        {
            Destroy(currentAtomInstance);
        }

        restart.gameObject.SetActive(false);
        ShowAllButtons();
        EnableAllButtons();
    }

    void DisableAllButtons()
    {
        easyBtn.GetComponent<DifficultyUIBtn>().DisableButton();
        mediumBtn.GetComponent<DifficultyUIBtn>().DisableButton();
        hardBtn.GetComponent<DifficultyUIBtn>().DisableButton();
    }

    void EnableAllButtons()
    {
        easyBtn.GetComponent<DifficultyUIBtn>().EnableButton();
        mediumBtn.GetComponent<DifficultyUIBtn>().EnableButton();
        hardBtn.GetComponent<DifficultyUIBtn>().EnableButton();
    }

    void HideAllButtons()
    {
        easyBtn.GetComponent<DifficultyUIBtn>().HideButton();
        mediumBtn.GetComponent<DifficultyUIBtn>().HideButton();
        hardBtn.GetComponent<DifficultyUIBtn>().HideButton();
        returnBtn.GetComponent<ReturnUIButton>().HideButton();
    }

    void ShowAllButtons()
    {
        easyBtn.GetComponent<DifficultyUIBtn>().ShowButton();
        mediumBtn.GetComponent<DifficultyUIBtn>().ShowButton();
        hardBtn.GetComponent<DifficultyUIBtn>().ShowButton();
        returnBtn.GetComponent<ReturnUIButton>().ShowButton();
    }

    bool AllElectronsSnapped()
    {
        foreach (var e in allElectrons)
        {
            if (!e.IsSnapped()) return false;
        }

        return true;
    }

    IEnumerator EvaluateWinCondition()
    {
        bool allCorrect = true;

        foreach (var zone in allZones)
        {
            if (zone.GetSnappedCount() > zone.maxElectrons)
            {
                allCorrect = false;
                foreach (Transform electron in zone.GetSnappedElectrons())
                {
                    SetElectronMaterial(electron, incorrectMaterial);
                }
            }
            else
            {
                foreach (Transform electron in zone.GetSnappedElectrons())
                {
                    SetElectronMaterial(electron, correctMaterial);
                }
            }
        }

        if (allCorrect)
        {
            DissableElectrons();
            gameObject.GetComponent<TipPhrasesManager>().ShowWinMessage();
            yield return StartCoroutine(BlinkMaterials());
            yield return StartCoroutine(AnimateOrbiting());
            Restart();
        }
        else
        {
            gameEnded = false;
        }
    }

    void DissableElectrons()
    {
        foreach (var e in allElectrons)
        {
            LeanSelectableByFinger lsbf = e.GetComponent<LeanSelectableByFinger>();
            lsbf.enabled = false;
        }
    }

    IEnumerator BlinkMaterials()
    {
        float timer = 0f;
        bool toggle = false;

        while (timer < blinkDuration)
        {
            foreach (var e in allElectrons)
            {
                SetElectronMaterial(e.transform, toggle ? blinkMaterial : correctMaterial);
            }

            toggle = !toggle;
            timer += blinkInterval;

            yield return new WaitForSeconds(blinkInterval);
        }

        foreach (var e in allElectrons)
        {
            SetElectronMaterial(e.transform, correctMaterial);
        }
    }

    IEnumerator AnimateOrbiting()
    {
        float rotationSpeed = 90f;
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            foreach (var zone in allZones)
            {
                foreach (Transform electron in zone.GetSnappedElectrons())
                {
                    electron.RotateAround(zone.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SetElectronMaterial(Transform electron, Material mat)
    {
        Renderer rend = electron.GetComponent<Renderer>();
        if (rend != null) rend.material = mat;
    }
}
