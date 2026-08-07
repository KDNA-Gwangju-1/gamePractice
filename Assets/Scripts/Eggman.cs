using UnityEngine;
using System.Collections;

public class Eggman : MonoBehaviour
{
    public int hitsToDefeat = 3;
    public float meleeRange = 3.5f;
    public float knockback = 9f;
    public float meleeCooldown = 1.4f;
    public float projectileRange = 22f;
    public float projectileCooldown = 2.2f;
    public float projectileSpeed = 10f;
    public float moveSpeed = 3.5f;
    public float holdDistance = 3.0f;
    public float hoverOffset = 1.0f;
    public float headHitBounce = 8f;
    public float hitCooldown = 0.6f;

    Transform player;
    int hitsTaken;
    float lastAttackTime = -10f;
    float lastPlayerHitTime = -10f;
    Transform armR;
    Renderer podRenderer;
    Color podOriginalColor;

    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        armR = transform.Find("ArmR");
        var pod = transform.Find("Pod");
        if (pod != null)
        {
            podRenderer = pod.GetComponent<Renderer>();
            podOriginalColor = podRenderer.material.color;
        }
    }

    void OnEnable()
    {
        hitsTaken = 0;
        if (GameManager.Instance != null) GameManager.Instance.UpdateBossHits(hitsTaken, hitsToDefeat);
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float dist = flatToPlayer.magnitude;

        if (dist > holdDistance)
        {
            Vector3 moveDir = flatToPlayer.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        Vector3 pos = transform.position;
        RaycastHit hit;
        Vector3 origin = new Vector3(pos.x, pos.y + 50f, pos.z);
        if (Physics.Raycast(origin, Vector3.down, out hit, 200f, ~0, QueryTriggerInteraction.Ignore))
        {
            float targetY = hit.point.y + hoverOffset;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 4f);
            transform.position = pos;
        }

        if (flatToPlayer.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(flatToPlayer.normalized);
        }

        if (dist <= meleeRange)
        {
            if (Time.time - lastAttackTime > meleeCooldown)
            {
                lastAttackTime = Time.time;
                StartCoroutine(MeleeAttack());
            }
        }
        else if (dist <= projectileRange)
        {
            if (Time.time - lastAttackTime > projectileCooldown)
            {
                lastAttackTime = Time.time;
                FireProjectile();
            }
        }
    }

    IEnumerator MeleeAttack()
    {
        if (armR != null)
        {
            Vector3 armStart = armR.localPosition;
            Vector3 extended = armStart + Vector3.forward * 1.2f;
            float t = 0f;
            while (t < 0.12f)
            {
                armR.localPosition = Vector3.Lerp(armStart, extended, t / 0.12f);
                t += Time.deltaTime;
                yield return null;
            }
            t = 0f;
            while (t < 0.15f)
            {
                armR.localPosition = Vector3.Lerp(extended, armStart, t / 0.15f);
                t += Time.deltaTime;
                yield return null;
            }
            armR.localPosition = armStart;
        }

        if (player != null && Vector3.Distance(transform.position, player.position) <= meleeRange + 0.5f)
        {
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null) ApplyKnockback(rb);
        }
    }

    void FireProjectile()
    {
        var proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proj.name = "EggmanProjectile";
        proj.transform.position = transform.position + transform.forward * 1.4f + Vector3.up * 0.3f;
        proj.transform.localScale = Vector3.one * 0.5f;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = Color.red;
        proj.GetComponent<Renderer>().sharedMaterial = mat;
        var col = proj.GetComponent<Collider>();
        col.isTrigger = true;
        var rb = proj.AddComponent<Rigidbody>();
        rb.useGravity = false;

        Vector3 dir = player.position - proj.transform.position;
        dir.y = 0f;
        dir.Normalize();

        var script = proj.AddComponent<EggmanProjectile>();
        script.Init(dir, projectileSpeed, knockback);
    }

    public void RegisterHeadHit(Rigidbody rb)
    {
        if (Time.time - lastPlayerHitTime < hitCooldown) return;
        lastPlayerHitTime = Time.time;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * headHitBounce, ForceMode.Impulse);
        hitsTaken++;
        StartCoroutine(FlashPod());
        if (GameManager.Instance != null) GameManager.Instance.UpdateBossHits(hitsTaken, hitsToDefeat);
        if (hitsTaken >= hitsToDefeat)
        {
            Defeat();
        }
    }

    public void RegisterBodyTouch(Rigidbody rb)
    {
        if (Time.time - lastPlayerHitTime < hitCooldown) return;
        lastPlayerHitTime = Time.time;
        ApplyKnockback(rb);
    }

    IEnumerator FlashPod()
    {
        if (podRenderer == null) yield break;
        podRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        if (podRenderer != null) podRenderer.material.color = podOriginalColor;
    }

    void ApplyKnockback(Rigidbody rb)
    {
        Vector3 dir = rb.transform.position - transform.position;
        dir.y = 0.3f;
        dir.Normalize();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * knockback, ForceMode.Impulse);
        var pc = rb.GetComponent<PlayerController>();
        if (pc != null) pc.FlashHit();
    }

    void Defeat()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnEggmanDefeated(transform.position);
        gameObject.SetActive(false);
    }
}
