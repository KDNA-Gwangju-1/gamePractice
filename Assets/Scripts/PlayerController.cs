using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveForce = 15f;
    public float maxSpeed = 8f;
    public float fallResetY = -10f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.6f;

    Rigidbody rb;
    Vector3 startPosition;
    bool jumpRequested;
    Renderer rend;
    Color originalColor;
    Coroutine flashRoutine;
    bool isSuper;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        rend = GetComponent<Renderer>();
        if (rend != null) originalColor = rend.material.color;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    public void FlashHit()
    {
        if (rend == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        rend.material.color = originalColor;
        flashRoutine = null;
    }

    public void TransformSuper()
    {
        if (isSuper) return;
        isSuper = true;

        moveForce *= 1.6f;
        maxSpeed *= 1.6f;
        jumpForce *= 1.4f;

        Color gold = new Color(1f, 0.85f, 0.15f);
        originalColor = gold;
        if (rend != null)
        {
            rend.material.color = gold;
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", gold * 2f);
        }

        var glowGo = new GameObject("SuperGlow");
        glowGo.transform.parent = transform;
        glowGo.transform.localPosition = Vector3.zero;
        var glow = glowGo.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = gold;
        glow.range = 1.8f;
        glow.intensity = 2f;

        var auraGo = new GameObject("SuperAura");
        auraGo.transform.parent = transform;
        auraGo.transform.localPosition = Vector3.zero;
        auraGo.transform.localRotation = Quaternion.identity;
        auraGo.AddComponent<SuperAura>();
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        float h = 0f;
        float v = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1f;
        }

        Vector3 dir = new Vector3(h, 0f, v);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        rb.AddForce(dir * moveForce, ForceMode.Force);

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (transform.position.y < fallResetY)
        {
            transform.position = startPosition;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
