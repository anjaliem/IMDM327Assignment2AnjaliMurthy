// IMDM 327 FALL 2026
// Assignment 2 - Anjali Murthy

using UnityEngine;
using System.Linq;

public class Dandelion : MonoBehaviour{

    // for input volume
    private string device;
    private AudioClip clip;
    private const int sampleWindow = 1024;
    private float[] samples = new float[sampleWindow];

    public float volume_raw;
    [SerializeField] public float detectVol = 0.02f;

    // for sound effect
    private AudioSource audioSource;

    // boids bodies
    class Seed {
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }
    private float G = 1000f;
    Seed[] seeds;
    private int n = 100; // # of seeds
    [SerializeField] public Material seedmat;
    private Renderer objectRenderer;
    
    // boids - properties
    private float minimumDistance = 1f; // boids separation - distance threshold
    private float separationForce = 2f; // boids separation - force
    private const float myMass = 2f;
    private const float radius = 20f;

    // stem
    private float minAngle = -10f;
    private float maxAngle = 10f;
    private float centerAngle = 0f;
    private float swayspeed;
    private float amplitude;
    private float zRotation;
    public GameObject stemObject;

    // meter
    public GameObject meter;

    // clouds
    public GameObject clouds1;
    private Vector3 cloud1speed = new Vector3(0f, 1f, 0f);
    public GameObject clouds2;
    private Vector3 cloud2speed = new Vector3(0f, 0.02f, 0f);

    void Start() {
        // for mic input
        StartMic();
        // for sound effect
        audioSource = GetComponent<AudioSource>();
        // init seed boids
        seeds = new Seed[n];
        for (int i = 0; i < n; i++) {
            seeds[i] = new Seed();
            seeds[i].body                       = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            seeds[i].body.transform.position    = Random.onUnitSphere * radius;
            Vector3 tempPosition                = seeds[i].body.transform.position;
            tempPosition.z                      = tempPosition.z + 100f;
            seeds[i].body.transform.position    = tempPosition;
            seeds[i].velocity                   = Vector3.zero;
            seeds[i].mass                       = myMass;
            objectRenderer = seeds[i].body.GetComponent<Renderer>();
            objectRenderer.material = seedmat;
            addTrail(seeds[i]);
        }
        // init stem sway
        amplitude = (maxAngle - minAngle) / 2f; // 5f
    }

    void Update() {

        // volume factors
        volume_raw = getVolume();
        //volume_0_1 = Mathf.Clamp01(volume_raw);
        float volume_0_1_approx = volume_raw * (1/0.05f);
        float volume_0_1_approx_clamped = Mathf.Clamp01(volume_0_1_approx);

        // meter
        float volume_meter = Mathf.Clamp01(volume_0_1_approx_clamped) * 4f;
        meter.transform.localScale = new Vector3 (0.05f, volume_meter, 1f);

        // stem sway
        swayspeed = Mathf.Clamp01(volume_raw * 10f) * 10f;
        zRotation = (centerAngle + amplitude) * Mathf.Sin(Time.deltaTime * swayspeed);
        stemObject.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

        // cloud rotate
        clouds1.transform.Rotate(cloud1speed * Time.deltaTime);
        clouds2.transform.Rotate(cloud2speed * Time.deltaTime);

        // dynamic sound effect volume
        if (audioSource != null) {
            audioSource.volume = Mathf.Clamp01(volume_0_1_approx) *2f;
        }

        // dynamic seed behavior
        // initial acceleration
        for (int i = 0; i < n; i++){
            seeds[i].acceleration = Vector3.zero;
        }
        // seed behaviors
        for (int i = 0; i < n; i++){
            for (int j = i + 1; j < n; j++) {
                // gravity
                Vector3 distance = seeds[i].body.transform.position - seeds[j].body.transform.position;
                Vector3 gravity = CalculateGravity(distance, seeds[i].mass, seeds[j].mass);
                // acceleration
                seeds[i].acceleration -= gravity / seeds[i].mass;
                seeds[j].acceleration += gravity / seeds[j].mass;
                // boids - separation
                if (distance.magnitude > minimumDistance) {
                    separationForce = 2f;
                    separationForce = separationForce * (1f + volume_0_1_approx_clamped);
                    seeds[i].acceleration -= separationForce * gravity / seeds[i].mass;
                    Vector3 a = seeds[i].acceleration;
                    seeds[i].acceleration = new Vector3(a.x, a.y, Mathf.Abs(a.z));
                    seeds[j].acceleration -= separationForce * gravity / seeds[j].mass;
                    a = seeds[j].acceleration;
                    seeds[j].acceleration = new Vector3(a.x, a.y, Mathf.Abs(a.z));
                }
                // boids - cohesion
                // boids - alignment
            }
        }

        // seed acceleration & velocity
        float volume_speed = volume_0_1_approx_clamped / 100f;
        for (int i = 0; i < n; i++) {
            seeds[i].acceleration *= volume_speed;
            seeds[i].velocity += (seeds[i].acceleration * Time.deltaTime) * (1f + volume_speed);
            Vector3 v = seeds[i].velocity;
            seeds[i].velocity = new Vector3(v.x, v.y, Mathf.Abs(v.z));

            Vector3 currPos = seeds[i].body.transform.position;
            Vector3 posDelta = seeds[i].velocity * Time.deltaTime;
            float upFloat = Random.Range(0f, 0.01f);
            seeds[i].body.transform.position = new Vector3 (currPos.x + posDelta.x, currPos.y + posDelta.y + upFloat, Mathf.Abs(currPos.z));

            if ((volume_raw) < 0.01f) {
                //seeds[i].acceleration = Vector3.zero;
                Vector3 a = seeds[i].acceleration;
                seeds[i].acceleration = new Vector3(Mathf.Clamp01(a.x)/1.01f, Mathf.Clamp01(a.y)/1.01f, Mathf.Abs(a.z));

                seeds[i].velocity = seeds[i].velocity / 1.05f;
                Vector3 v2 = seeds[i].velocity;
                seeds[i].velocity = new Vector3(v2.x, v2.y, Mathf.Abs(v2.z));
            }
        }
    }



    // helper functions

    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2){
        Vector3 gravity = Vector3.zero;
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude + 1f) * distanceVector.normalized;
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

    private void addTrail(Seed seed) {
        TrailRenderer trailRenderer = seed.body.AddComponent<TrailRenderer>();
        trailRenderer.time = 1f;         // Duration of the trail
        trailRenderer.startWidth = 0.5f;    // Width of the trail at the start
        trailRenderer.endWidth = 0.05f;      // Width of the trail at the end
        trailRenderer.material = new Material(Shader.Find("Sprites/Default")); // a material to the trail
        Gradient gradient = new Gradient(); // Set the trail color over time
        Color targetColor = Color.grey;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(targetColor, 0.8f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        trailRenderer.colorGradient = gradient;
    }
}