using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    public float launchForce = 24f;
    public Vector3 launchDirection = new Vector3(0.7f, 1f, -0.1f);
    public float cooldown = 0.3f;

    float lastLaunchTime = -10f;

    void OnCollisionEnter(Collision collision)
    {
        TryLaunch(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        TryLaunch(collision.collider);
    }

    void TryLaunch(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastLaunchTime < cooldown) return;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        lastLaunchTime = Time.time;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
        StartCoroutine(BounceAnim());
    }

    IEnumerator BounceAnim()
    {
        Vector3 original = transform.localScale;
        Vector3 squashed = new Vector3(original.x * 1.15f, original.y * 0.6f, original.z * 1.15f);
        float t = 0f;
        while (t < 0.08f)
        {
            transform.localScale = Vector3.Lerp(original, squashed, t / 0.08f);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0f;
        while (t < 0.15f)
        {
            transform.localScale = Vector3.Lerp(squashed, original, t / 0.15f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = original;
    }
}
