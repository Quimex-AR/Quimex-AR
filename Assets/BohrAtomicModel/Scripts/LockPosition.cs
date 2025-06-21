using UnityEngine;

public class LockPosition : MonoBehaviour {
    private Vector3 initialLocalPosition;

    void Start() {
        initialLocalPosition = transform.localPosition;
    }

    void Update() {
        transform.localPosition = initialLocalPosition;
    }
}
