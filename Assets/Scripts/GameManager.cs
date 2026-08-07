using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Text scoreText;
    public GameObject winText;
    public GameObject eggman;

    int collected = 0;
    int totalPickups = 0;
    bool bossSpawned = false;
    bool bossActive = false;

    void Awake()
    {
        Instance = this;
        totalPickups = GameObject.FindGameObjectsWithTag("Pickup").Length;
        UpdateScoreText();
        if (winText != null) winText.SetActive(false);
        if (eggman != null) eggman.SetActive(false);
    }

    public void CollectPickup()
    {
        collected++;
        UpdateScoreText();
        if (collected >= totalPickups && !bossSpawned)
        {
            bossSpawned = true;
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        bossActive = true;
        if (eggman != null) eggman.SetActive(true);
    }

    public void UpdateBossHits(int hits, int total)
    {
        if (scoreText != null) scoreText.text = $"Eggman: {hits} / {total}";
    }

    public void OnEggmanDefeated(Vector3 position)
    {
        bossActive = false;
        if (scoreText != null) scoreText.text = "Eggman defeated! Find the Chaos Emerald";
        SpawnEmerald(position);
    }

    void SpawnEmerald(Vector3 position)
    {
        var emerald = new GameObject("ChaosEmerald");
        emerald.transform.position = position + Vector3.up * 1.4f;

        var core = new GameObject("GemCore");
        core.transform.parent = emerald.transform;
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = Vector3.one * 1.2f;
        var mf = core.AddComponent<MeshFilter>();
        mf.sharedMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/ChaosEmeraldMesh.asset");
        var mr = core.AddComponent<MeshRenderer>();

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color gemColor = new Color(0.05f, 0.75f, 0.3f);
        mat.color = gemColor;
        mat.SetColor("_EmissionColor", gemColor * 0.8f);
        mat.EnableKeyword("_EMISSION");
        mat.SetFloat("_Metallic", 0.1f);
        mat.SetFloat("_Smoothness", 0.85f);
        mr.sharedMaterial = mat;

        var light = new GameObject("GemGlow");
        light.transform.parent = emerald.transform;
        light.transform.localPosition = Vector3.zero;
        var lightComp = light.AddComponent<Light>();
        lightComp.type = LightType.Point;
        lightComp.color = gemColor;
        lightComp.range = 6f;
        lightComp.intensity = 1.5f;

        var triggerCol = emerald.AddComponent<SphereCollider>();
        triggerCol.isTrigger = true;
        triggerCol.radius = 0.8f;

        emerald.AddComponent<ChaosEmerald>();
    }

    public void WinGame()
    {
        if (winText != null) winText.SetActive(true);
    }

    void UpdateScoreText()
    {
        if (!bossActive) scoreText.text = $"Score: {collected} / {totalPickups}";
    }
}
