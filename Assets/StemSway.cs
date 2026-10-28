using UnityEngine;

public class StemSway : MonoBehaviour {

    private float minRot = -5f;
    private float maxRot = 5f;

    private Vector3 baseRotVector;
    private Vector3 swayRight = new Vector3(0f, 0f, 0.002f);
    private Vector3 swayLeft = new Vector3(0f, 0f, -0.002f);

    void Start() {
        baseRotVector = Vector3.zero;
    }

    void Update() {
        while (transform.localEulerAngles.z <= maxRot) {
            transform.Rotate(swayRight * Time.deltaTime);
        }
        while (transform.localEulerAngles.z >= minRot) {
            transform.Rotate(swayLeft * Time.deltaTime);
        }
    }
}