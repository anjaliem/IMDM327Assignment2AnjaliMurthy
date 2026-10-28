// IMDM 327 FALL 2026
// Assignment 2 - Anjali Murthy
// Starter code by instructor Myungin Lee

using UnityEngine;
using System.Linq;

public class Dandelion : MonoBehaviour{
    private string device;
    private AudioClip clip;
    private const int sampleWindow = 1024;
    private float[] samples = new float[sampleWindow];
    // private bool isInit;
    public float volume;
    private Vector3 volumeVector;

    private float G = 1000f;              // gravitational constant - make it public and not const so we can adjust with slider
    BodyProperty[] bp;                  // list of all bodies
    private int numberOfSpheres = 250;    // number of bodies
    class BodyProperty{                 // body object class
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }
    private float minimumDistance = 1f;  // control distance of Boids separation property
    private float separationForce = 10f; // control force of Boids separation property
    // Boids - cohesion property
    // Boids - alignment property
    private float fasterTime = 1f;       // adjustable time multiplier
    private const float myMass = 2f;
    private float r; // can probs either adjust this value or move camera back (-z axis) based on framing

    void Start(){
        r = 0f;
        // create array for each body and its properties
        bp = new BodyProperty[numberOfSpheres];
        // assign initial conditions for each body in the array
        for (int i = 0; i < numberOfSpheres; i++){
            r = Random.Range(5f, 40f);
            // create game object for each body
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // bp[i].body.transform.localScale = Vector3.one * 3f;
            // position is (x,y,z), so plot bodies on a circle with r
            // z = 180 places body in front of camera, near origin, looking along +z axis (try other positions too)
            float theta = 2 * Mathf.PI / numberOfSpheres * i;
            bp[i].body.transform.position = new Vector3(r * Mathf.Cos(theta) + Random.Range(-5f,5f),
                                                        r * Mathf.Sin(theta) + Random.Range(-5f,5f) + 25,
                                                        180f);
            bp[i].velocity = Vector3.zero; // try different initial condition - for example bp[i].velocity = new Vector3(5f, Random.Range(), );
            bp[i].mass = myMass;                // simplified - try different initial condition

            // trail
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            trailRenderer.time = 0.5f;         // Duration of the trail
            trailRenderer.startWidth = 0.5f;    // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;      // Width of the trail at the end
            trailRenderer.material = new Material(Shader.Find("Sprites/Default")); // a material to the trail
            Gradient gradient = new Gradient(); // Set the trail color over time
            Color targetColor = Color.grey;
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(targetColor, 0.8f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            trailRenderer.colorGradient = gradient;
        }
        StartMic();
        volume = 0f;
    }


    void Update(){
        // Set properties/behavior of all bodies

        volume = getVolume();
        volumeVector = new Vector3(volume,volume,volume);

        // Set initial acceleration
        for (int i = 0; i < numberOfSpheres; i++){
            bp[i].acceleration = Vector3.zero;
        }
        // Set gravity & acceleration, and enforce Boids behaviors
        for (int i = 0; i < numberOfSpheres; i++){
            for (int j = i + 1; j < numberOfSpheres; j++){  // j + 1 because we don't want to do equal pairs or duplicate pairs
                // Gravity
                Vector3 distance = bp[i].body.transform.position - bp[j].body.transform.position;
                Vector3 gravity = CalculateGravity(distance, bp[i].mass, bp[j].mass);
                // Acceleration
                bp[i].acceleration -= gravity / bp[i].mass;
                bp[j].acceleration += gravity / bp[j].mass;
                // Boids - Separation Property
                if (distance.magnitude > minimumDistance){
                    bp[i].acceleration -= separationForce * gravity / bp[i].mass;
                    bp[j].acceleration -= separationForce * gravity / bp[j].mass;
                }
                // Boids - Cohesion
                // Boids - Alignment
            }
        }
        for (int i = 0; i < numberOfSpheres; i++){
            bp[i].acceleration *= volume;
            bp[i].velocity += (bp[i].acceleration * Time.deltaTime * fasterTime) * this.volume;             //velocity

            Vector3 currPos = bp[i].body.transform.position;
            Vector3 posDelta = bp[i].velocity * Time.deltaTime * fasterTime;
            bp[i].body.transform.position = new Vector3 (currPos.x + posDelta.x, currPos.y + posDelta.y, currPos.z);  //position
            //bp[i].body.transform.localScale += volumeVector/50f;
        }
    }

    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2){
        Vector3 gravity = Vector3.zero;
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude + 1f) * distanceVector.normalized; // 1f can help avoid the issue of divergence
        return gravity;
    }

    private void StartMic() {
        if (Microphone.devices.Length > 0) {
            device = Microphone.devices[0];
            clip = Microphone.Start(device, true, 1, AudioSettings.outputSampleRate);
        } else {
            Debug.LogError("no microphone detected!");
        }
    }

    private float getVolume() {
        float vol = 0f;
        if (clip == null) {
            return vol; // 0f
        }
        int micPos = Microphone.GetPosition(device) - sampleWindow;
        if (micPos < 0) {
            return vol; // 0f
        }
        clip.GetData(samples, micPos);
        return samples.Select(Mathf.Abs).Average(); // average absolute amplitude
    }
}