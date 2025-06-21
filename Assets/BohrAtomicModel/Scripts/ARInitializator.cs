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

// i have a problem, i install lean touch for unity, and work excelen in the simulator, but as soon as i try creating an AR scene with vuforia which change the main camera to the vuforia ar camera it like mess up the touch
