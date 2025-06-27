using UnityEngine;
using Vuforia;

public class ARInitializator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// This will initialize the delayed Vuforia, this script have to be
    /// somewhere in the scene that will be using vuforia assets to make
    /// them work
    /// </summary>
    void Start()
    {
        VuforiaApplication.Instance.Initialize();
    }
}
