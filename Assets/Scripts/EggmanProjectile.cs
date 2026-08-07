using UnityEngine;

public class EggmanProjectile : MonoBehaviour
{
    Vector3 direction;
    float speed;
    float knockback;
    float lifetime = 5f;

    public void Init(Vector3 dir, float spd, float kb)
    {
        direction = dir;
        speed = spd;
        knockback = kb;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.FlashHit();
        var rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0.3f;
            dir.Normalize();
            rb.AddForce(dir * knockback, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }
}
