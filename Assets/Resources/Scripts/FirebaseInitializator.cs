using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializator : MonoBehaviour
{
    public static bool IsFirebaseReady { get; private set; } = false;
    public static FirebaseApp AppInstance { get; private set; }

    private static bool alreadyInitialized = false;

    void Awake()
    {
        if (alreadyInitialized)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        alreadyInitialized = true;
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                AppInstance = FirebaseApp.DefaultInstance;
                IsFirebaseReady = true;
                Debug.Log("Firebase is ready!");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                IsFirebaseReady = false;
            }
        });
    }
}
