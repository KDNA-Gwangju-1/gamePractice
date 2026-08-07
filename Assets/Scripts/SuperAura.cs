using UnityEngine;

public class SuperAura : MonoBehaviour
{
    public Color orbColor = new Color(1f, 0.82f, 0.15f);
    public float orbitRadius = 1.4f;
    public float orbSize = 0.22f;

    Transform[] pivots;
    float[] spinSpeeds;

    void Awake()
    {
        Material orbMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        orbMat.color = orbColor;
        orbMat.EnableKeyword("_EMISSION");
        orbMat.SetColor("_EmissionColor", orbColor * 2.2f);
        orbMat.SetFloat("_Metallic", 0.1f);
        orbMat.SetFloat("_Smoothness", 0.95f);

        var trailMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        trailMat.color = orbColor;

        Vector3[] tilts = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(65f, 20f, 0f),
            new Vector3(-65f, -20f, 0f)
        };

        pivots = new Transform[tilts.Length];
        spinSpeeds = new float[tilts.Length];

        for (int i = 0; i < tilts.Length; i++)
        {
            var pivot = new GameObject("Orbit_" + i);
            pivot.transform.parent = transform;
            pivot.transform.localPosition = Vector3.zero;
            pivot.transform.localRotation = Quaternion.Euler(tilts[i]);
            pivots[i] = pivot.transform;
            spinSpeeds[i] = (i % 2 == 0 ? 1f : -1f) * (140f + i * 60f);

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "Orb";
            orb.transform.parent = pivot.transform;
            orb.transform.localPosition = new Vector3(orbitRadius, 0f, 0f);
            orb.transform.localScale = Vector3.one * orbSize;
            orb.GetComponent<Renderer>().sharedMaterial = orbMat;
            Object.Destroy(orb.GetComponent<Collider>());

            var orbLight = orb.AddComponent<Light>();
            orbLight.type = LightType.Point;
            orbLight.color = orbColor;
            orbLight.range = 1.5f;
            orbLight.intensity = 1.5f;

            var trail = orb.AddComponent<TrailRenderer>();
            trail.time = 0.35f;
            trail.startWidth = orbSize * 0.9f;
            trail.endWidth = 0f;
            trail.material = trailMat;
            trail.colorGradient = MakeFadeGradient(orbColor);
            trail.minVertexDistance = 0.02f;
        }

        BuildEnergyParticles();
    }

    void BuildEnergyParticles()
    {
        var sparkGo = new GameObject("EnergyParticles");
        sparkGo.transform.parent = transform;
        sparkGo.transform.localPosition = Vector3.zero;
        var ps = sparkGo.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = orbColor;
        main.startSize = 0.14f;
        main.startLifetime = 0.7f;
        main.startSpeed = 2.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;

        var emission = ps.emission;
        emission.rateOverTime = 45f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.6f;

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        vel.y = new ParticleSystem.MinMaxCurve(1.5f, 3f);

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(orbColor, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = grad;

        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0.1f)));

        var psr = sparkGo.GetComponent<ParticleSystemRenderer>();
        var particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (particleShader == null) particleShader = Shader.Find("Universal Render Pipeline/Unlit");
        var particleMat = new Material(particleShader);
        particleMat.SetColor("_BaseColor", Color.white);
        psr.material = particleMat;
        psr.renderMode = ParticleSystemRenderMode.Billboard;
    }

    static Gradient MakeFadeGradient(Color c)
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        return g;
    }

    void Update()
    {
        for (int i = 0; i < pivots.Length; i++)
        {
            pivots[i].Rotate(Vector3.forward, spinSpeeds[i] * Time.deltaTime, Space.Self);
        }
    }
}
