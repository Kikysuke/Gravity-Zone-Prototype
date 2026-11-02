using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement3D : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 10f;
    public float jumpSpeed = 8f;
    public Key jumpKey = Key.Space;
    public float killDistance = 50f;

    bool isGround;
    bool facingRight = true;
    bool jumpRequested;
    float horizontal;

    Quaternion viewRot = Quaternion.identity;
    float gravityStrength;
    Vector3 originalGravity;
    Vector3 spawnPos;
    float lockedDepth;

    public Quaternion ViewRotation => viewRot;
    Vector3 Up => viewRot * Vector3.up;
    Vector3 Right => viewRot * Vector3.right;
    Vector3 Forward => viewRot * Vector3.forward;

    void Awake()
    {
        originalGravity = Physics.gravity;
        gravityStrength = originalGravity.magnitude;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        spawnPos = rb.position;
        lockedDepth = Vector3.Dot(rb.position, Forward);
    }

    void OnDestroy()
    {
        Physics.gravity = originalGravity;
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null) return;

        // Roll (around the view direction): same as your old Q/E
        if (kb.qKey.wasPressedThisFrame) Rotate(Quaternion.Euler(0, 0, 90f));
        if (kb.eKey.wasPressedThisFrame) Rotate(Quaternion.Euler(0, 0, -90f));

        // Yaw (around gravity-up): the new 2.5D turn
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame) Rotate(Quaternion.Euler(0, 90f, 0));
            if (mouse.rightButton.wasPressedThisFrame) Rotate(Quaternion.Euler(0, -90f, 0));
        }

        horizontal = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;

        if (kb[jumpKey].wasPressedThisFrame) jumpRequested = true;

        if (horizontal > 0 && !facingRight) Flip();
        if (horizontal < 0 && facingRight) Flip();
    }

    void FixedUpdate()
    {
        float vertical = Vector3.Dot(rb.linearVelocity, Up);

        if (jumpRequested && isGround)
        {
            vertical = jumpSpeed;
            isGround = false;
        }
        jumpRequested = false;

        // Velocity along the view direction is dropped, so you stay on the plane
        rb.linearVelocity = Right * (horizontal * speed) + Up * vertical;

        // Keep the depth coordinate pinned to the plane
        float depthError = lockedDepth - Vector3.Dot(rb.position, Forward);
        rb.position += Forward * depthError;

        if ((rb.position - spawnPos).magnitude > killDistance) Respawn();

        isGround = false; // re-set by OnCollisionStay each physics step
    }

    void Rotate(Quaternion delta)
    {
        viewRot = viewRot * delta;

        // Snap to exact 90?Åã steps so float error never accumulates
        Vector3 e = viewRot.eulerAngles;
        viewRot = Quaternion.Euler(
            Mathf.Round(e.x / 90f) * 90f,
            Mathf.Round(e.y / 90f) * 90f,
            Mathf.Round(e.z / 90f) * 90f);

        ApplyView();
    }

    void ApplyView()
    {
        Physics.gravity = viewRot * Vector3.down * gravityStrength;
        rb.rotation = viewRot;
        rb.linearVelocity = Vector3.zero;
        lockedDepth = Vector3.Dot(rb.position, Forward); // new plane through the player
        isGround = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (!(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box")))
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector3.Dot(collision.GetContact(i).normal, Up) > 0.5f)
            {
                isGround = true;
                return;
            }
        }
    }

    public void Respawn()
    {
        viewRot = Quaternion.identity;
        rb.position = spawnPos;
        ApplyView();
    }

    void Flip()
    {
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
        facingRight = !facingRight;
    }
}