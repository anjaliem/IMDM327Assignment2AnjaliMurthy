// IMDM 327 FALL 2026
// Assignment 1 - Anjali Murthy
// Starter code by instructor Myungin Lee

using UnityEngine;

public class SolarSystemStarter : MonoBehaviour {
    DataCSV solarCSV;
    DataJSON solarJSON;
    const float G = 6.674e-11f;
    PlanetProperty[] planetProperties;
    private int numberOfSpheres = 10;
    class PlanetProperty {
        public GameObject planet;
        public float mass;
        public float radius;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 actualPosition;
    }
    public BodyProperty[] solarBodiesCSV;
    public SolarBody[] solarBodiesJSON;
    float timeController = 1000000f;
    private Material[] planetMaterials; 
    private float[] sizeScales;
    private Color[] trailColors;
    private float[] trailLengths;
    
    void Start(){

        solarCSV = GetComponent<DataCSV>();
        solarBodiesCSV = solarCSV.bp;
        // solarJSON = GetComponent<DataJSON>();
        // solarBodiesJSON = solarJSON.solarData.bodies;

        planetMaterials = new Material[] {
            Resources.Load<Material>("Materials/sunMaterial"),
            Resources.Load<Material>("Materials/mercuryMaterial"),
            Resources.Load<Material>("Materials/venusMaterial"),
            Resources.Load<Material>("Materials/earthMaterial"),
            Resources.Load<Material>("Materials/marsMaterial"),
            Resources.Load<Material>("Materials/jupiterMaterial"),
            Resources.Load<Material>("Materials/saturnMaterial"),
            Resources.Load<Material>("Materials/uranusMaterial"),
            Resources.Load<Material>("Materials/neptuneMaterial"),
            Resources.Load<Material>("Materials/plutoMaterial")        
        };
        sizeScales = new float[] {15f, 4f, 5f, 5f, 5f, 10f, 10f, 8f, 8f, 3f};
        trailColors = new Color[] {
            Color.HSVToRGB(0f, 0f, 1f),
            Color.HSVToRGB(0f, 0f, 0.48f),
            Color.HSVToRGB(39f/360f, 1f, 1f),
            Color.HSVToRGB(165f/360f, 1f, 0.45f),
            Color.HSVToRGB(20f/360f, 1f, 1f),
            Color.HSVToRGB(31f/360f, 0.61f, 1f),
            Color.HSVToRGB(37f/360f, 0.44f, 1f),
            Color.HSVToRGB(186f/360f, 21f, 1f),
            Color.HSVToRGB(199f/360f, 1f, 1f),
            Color.HSVToRGB(21f/360f, 0.38f, 0.62f)
        };
        trailLengths = new float[] {0f, 4f, 5f, 5f, 5f, 200f, 200f, 1000f, 1000f, 1000f};

        planetProperties = new PlanetProperty[numberOfSpheres];
        for (int i = 0; i < numberOfSpheres; i++){
            planetProperties[i] = new PlanetProperty();
            planetProperties[i].planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planetProperties[i].planet.GetComponent<MeshRenderer>().material = planetMaterials[i];
            planetProperties[i].planet.transform.localScale = Vector3.one * sizeScales[i];
        }
        for (int i = 0; i < solarBodiesCSV.Length; i++){
            planetProperties[i].mass = solarBodiesCSV[i].mass;
            planetProperties[i].radius = solarBodiesCSV[i].radius;
            float initAngle = Random.Range(0f, 2f * Mathf.PI);
            float r = solarBodiesCSV[i].distance;
            float initVelocity = solarBodiesCSV[i].initial_velocity;

            planetProperties[i].actualPosition = new Vector3(r * Mathf.Cos(initAngle + Mathf.PI/2f),
                                                             r * Mathf.Sin(initAngle + Mathf.PI/2f),
                                                             0f);
            planetProperties[i].velocity = new Vector3(initVelocity * Mathf.Cos(initAngle),
                                                       initVelocity * Mathf.Sin(initAngle),
                                                       0f);
            
            // trail
            TrailRenderer trailRenderer = planetProperties[i].planet.AddComponent<TrailRenderer>();
            trailRenderer.time = trailLengths[i];         // Duration of the trail
            trailRenderer.startWidth = 1f;    // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;      // Width of the trail at the end
            trailRenderer.material = new Material(Shader.Find("Sprites/Default")); // a material to the trail
            Gradient gradient = new Gradient(); // Set the trail color over time
            Color targetColor = trailColors[i];
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(targetColor, 0.8f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            trailRenderer.colorGradient = gradient;
        }
    }

    void Update(){
        for (int i = 0; i < numberOfSpheres; i++){
            planetProperties[i].acceleration = Vector3.zero;
        }
        for (int i = 0; i < numberOfSpheres; i++){
            for (int j = i + 1; j < numberOfSpheres; j++){
                Vector3 distance = planetProperties[i].actualPosition - planetProperties[j].actualPosition;
                Vector3 gravity = CalculateGravity(distance, planetProperties[i].mass, planetProperties[j].mass);
                planetProperties[i].acceleration -= gravity / planetProperties[i].mass;
                planetProperties[j].acceleration += gravity / planetProperties[j].mass;
            }
        }
        for (int i = 0; i < numberOfSpheres; i++){
            planetProperties[i].velocity += planetProperties[i].acceleration * Time.deltaTime * timeController;
            planetProperties[i].actualPosition += planetProperties[i].velocity * Time.deltaTime * timeController;
            float scaledDistance = Mathf.Sqrt(planetProperties[i].actualPosition.magnitude / 1e8f);
            planetProperties[i].planet.transform.position = scaledDistance * planetProperties[i].actualPosition.normalized;
        }
    }

    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2){
        Vector3 gravity = Vector3.zero;
        gravity = G * m1 * m2 / (distanceVector.sqrMagnitude + 1f) * distanceVector.normalized;
        return gravity;
    }
}