using UnityEngine;

public class CloudRotate : MonoBehaviour {
    private Vector3 rotationSpeed = new Vector3(0f, 1f, 0f);

    void Update() {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}