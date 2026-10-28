// IMDM 327 FALL 2026
// Assignment 1 - Anjali Murthy
// Starter code by instructor Myungin Lee

using UnityEngine;

public class ThreeBody : MonoBehaviour{
    public float G = 500f;              // gravitational constant - make it public and not const so we can adjust with slider
    BodyProperty[] bp;                  // list of all bodies
    public int numberOfSpheres = 100;    // number of bodies
    class BodyProperty{                 // body object class
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }
    public float minimumDistance = 1f;  // control distance of Boids separation property
    public float separationForce = 10f; // control force of Boids separation property
    // Boids - cohesion property
    // Boids - alignment property
    public float fasterTime = 1f;       // adjustable time multiplier
    private const float myMass = 1f;
    private const float r = 75f; // can probs either adjust this value or move camera back (-z axis) based on framing

    void Start(){
        // create array for each body and its properties
        bp = new BodyProperty[numberOfSpheres];
        // assign initial conditions for each body in the array
        for (int i = 0; i < numberOfSpheres; i++){
            // create game object for each body
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // bp[i].body.transform.localScale = Vector3.one * 3f;
            // position is (x,y,z), so plot bodies on a circle with r
            // z = 180 places body in front of camera, near origin, looking along +z axis (try other positions too)
            float theta = 2 * Mathf.PI / numberOfSpheres * i;
            bp[i].body.transform.position = new Vector3(r * Mathf.Cos(theta) + Random.Range(-5f,5f),
                                                        r * Mathf.Sin(theta) + Random.Range(-5f,5f),
                                                        180);
            bp[i].velocity = Vector3.zero; // try different initial condition - for example bp[i].velocity = new Vector3(5f, Random.Range(), );
            bp[i].mass = myMass;                // simplified - try different initial condition

            // trail
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            trailRenderer.time = 10.0f;         // Duration of the trail
            trailRenderer.startWidth = 0.5f;    // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;      // Width of the trail at the end
            trailRenderer.material = new Material(Shader.Find("Sprites/Default")); // a material to the trail
            Gradient gradient = new Gradient(); // Set the trail color over time
            Color targetColor = Color.HSVToRGB((float)i / numberOfSpheres, 0.8f, 0.8f);
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(targetColor, 0.8f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            trailRenderer.colorGradient = gradient;
        }
    }


    void Update(){
        // Set properties/behavior of all bodies
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
                if (distance.magnitude < minimumDistance){
                    bp[i].acceleration += separationForce * gravity / bp[i].mass;
                    bp[j].acceleration -= separationForce * gravity / bp[j].mass;
                }
                // Boids - Cohesion
                // Boids - Alignment
            }
        }
        for (int i = 0; i < numberOfSpheres; i++){
            bp[i].velocity += (bp[i].acceleration * Time.deltaTime * fasterTime);             //velocity
            bp[i].body.transform.position += (bp[i].velocity * Time.deltaTime * fasterTime);  //position
        }
    }

    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2){
        Vector3 gravity = Vector3.zero;
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude + 1f) * distanceVector.normalized; // 1f can help avoid the issue of divergence
        return gravity;
    }
}