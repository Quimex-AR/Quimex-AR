using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ManagerAtom : MonoBehaviour
{
    public GameObject protonPrefab;   // Prefab para el protón
    public GameObject neutronPrefab;  // Prefab para el neutrón
    public GameObject nucleusPrefab;  // Prefab para el núcleo (esfera de tamaño 0.3, 0.3, 0.3)
    public GameObject electronPrefab;

    public GameObject sOrbitalPrefab; // Prefab para el orbital s
    public GameObject pOrbitalPrefab; // Prefab para el orbital p
    public GameObject dOrbitalPrefab; // Prefab para el orbital d
    public GameObject fOrbitalPrefab; // Prefab para el orbital f

    public int protonCount = 8;   // Número de protones (por defecto 8 para oxígeno)
    public int neutronCount = 8;  // Número de neutrones
    public int electronCount = 8; // Número de electrones

    private List<GameObject> protons = new List<GameObject>();
    private List<GameObject> neutrons = new List<GameObject>();
    private List<GameObject> electrons = new List<GameObject>();
    private List<GameObject> orbitals = new List<GameObject>();

    public GameObject nucleusObject; // GameObject vacío para el núcleo

    void Start()
    {
        GenerateAtom();  // Generar el átomo al inicio
    }

    // Método para generar el átomo
    public void GenerateAtom()
    {
        // Limpiar cualquier átomo previamente generado
        ClearAtom();

        // Instanciar el núcleo en una posición visible (por ejemplo, en el centro de la escena)
        nucleusObject = Instantiate(nucleusPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        nucleusObject.name = "Nucleus";

        // Crear protones y neutrones en el núcleo
        CreateNucleus();

        // Crear electrones y orbitales S, P, D, F
        GenerateElectronsAndOrbitals(electronCount);

        // Asegurarse de que el núcleo esté activado
        nucleusObject.SetActive(true);
    }

    // Crear protones y neutrones dentro de la esfera del núcleo
    private void CreateNucleus()
    {
        float nucleusRadius = 0.15f;  // Radio del núcleo (la mitad de la esfera)

        // Distribuir los protones de manera aleatoria dentro de la esfera
        for (int i = 0; i < protonCount; i++)
        {
            Vector3 position = Random.onUnitSphere * nucleusRadius;  // Generar posición aleatoria
            GameObject proton = Instantiate(protonPrefab, position + nucleusObject.transform.position, Quaternion.identity);
            proton.transform.SetParent(nucleusObject.transform);
            protons.Add(proton);
        }

        // Distribuir los neutrones de manera aleatoria dentro de la esfera
        for (int i = 0; i < neutronCount; i++)
        {
            Vector3 position = Random.onUnitSphere * nucleusRadius;  // Generar posición aleatoria
            GameObject neutron = Instantiate(neutronPrefab, position + nucleusObject.transform.position, Quaternion.identity);
            neutron.transform.SetParent(nucleusObject.transform);
            neutrons.Add(neutron);
        }
    }

    // Método para generar electrones y orbitales (centrados en orbitales S, P, D y F)
    private void GenerateElectronsAndOrbitals(int electronCount)
    {
        int electronsLeft = electronCount;
        int level = 1;

        while (electronsLeft > 0)
        {
            if (level == 1)
            {
                int sElectrons = Mathf.Min(electronsLeft, 2);
                electronsLeft -= AddOrbital("s", level, sElectrons);
            }
            else if (level == 2)
            {
                int sElectrons = Mathf.Min(electronsLeft, 2);
                electronsLeft -= AddOrbital("s", level, sElectrons);

                if (electronsLeft > 0)
                {
                    int pElectrons = Mathf.Min(electronsLeft, 6);
                    electronsLeft -= AddOrbital("p", level, pElectrons);
                }
            }
            level++;
        }
    }

    private int AddOrbital(string type, int level, int maxElectrons)
    {
        GameObject orbitalPrefab = null;

        if (type == "s") orbitalPrefab = sOrbitalPrefab;
        else if (type == "p") orbitalPrefab = pOrbitalPrefab;

        float orbitalDistance = 0.5f + level * 0.8f;

        GameObject orbital = Instantiate(orbitalPrefab, nucleusObject.transform.position, Quaternion.identity);
        orbital.transform.SetParent(nucleusObject.transform);
        orbitals.Add(orbital);

        float scaleFactor = 1.5f + (level * 0.5f);
        orbital.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        for (int i = 0; i < maxElectrons; i++)
        {
            GameObject electron = Instantiate(electronPrefab, orbital.transform);
            Vector3 dir = Random.onUnitSphere;
            electron.transform.localPosition = dir * 0.6f;
            electrons.Add(electron);
        }

        return maxElectrons;
    }

    // Limpiar el átomo generado (protones, neutrones, electrones y orbitales)
    void ClearAtom()
    {
        foreach (var proton in protons) Destroy(proton);
        foreach (var neutron in neutrons) Destroy(neutron);
        foreach (var electron in electrons) Destroy(electron);
        foreach (var orbital in orbitals) Destroy(orbital);
        if (nucleusObject != null) Destroy(nucleusObject);

        protons.Clear();
        neutrons.Clear();
        electrons.Clear();
        orbitals.Clear();
    }
}
