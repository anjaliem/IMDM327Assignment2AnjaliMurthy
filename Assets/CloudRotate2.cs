using UnityEngine;

public class CloudRotate2 : MonoBehaviour {
    private Vector3 rotationSpeed = new Vector3(0f, 0.002f, 0f);

    void Update() {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}