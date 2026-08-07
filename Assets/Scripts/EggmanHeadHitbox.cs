using UnityEngine;

public class EggmanHeadHitbox : MonoBehaviour
{
    public Eggman owner;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;
        if (rb.linearVelocity.y <= 0.2f && owner != null)
        {
            owner.RegisterHeadHit(rb);
        }
    }
}
