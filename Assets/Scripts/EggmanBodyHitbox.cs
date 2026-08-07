using UnityEngine;

public class EggmanBodyHitbox : MonoBehaviour
{
    public Eggman owner;

    void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    void TryHit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        if (rb == null || owner == null) return;
        owner.RegisterBodyTouch(rb);
    }
}
