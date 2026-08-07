using UnityEngine;

public class ChaosEmerald : MonoBehaviour
{
    public float spinSpeed = 80f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;
    public float activationDelay = 1f;

    Vector3 basePos;
    float spawnTime;

    void Start()
    {
        basePos = transform.position;
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        Vector3 p = basePos;
        p.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = p;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - spawnTime < activationDelay) return;
        if (GameManager.Instance != null) GameManager.Instance.WinGame();
        Destroy(gameObject);
    }
}
