using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 10.0f;
    Vector2 jumpFoward;
    public Key jumpKey = Key.Space;
    public float jumpPower = 6.5f;
    private bool isGround;
    private bool facingRight = true;

    Vector2 playerPos;

    float gravityAngle = 0f;
    float gravityStrength;
    Vector2 originalGravity;

    public Vector2 Up => Quaternion.Euler(0, 0, gravityAngle) * Vector2.up;

    void Awake()
    {
        originalGravity = Physics2D.gravity;
        gravityStrength = originalGravity.magnitude;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        jumpFoward = new Vector2(0.0f, 0.1f);

        playerPos = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
    }

    void OnDestroy()
    {
        Physics2D.gravity = originalGravity;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (isGround && kb.leftArrowKey.wasPressedThisFrame)
        {
            SetGravityAngle(gravityAngle - 90f);
        }
        if (isGround && kb.rightArrowKey.wasPressedThisFrame)
        {
            SetGravityAngle(gravityAngle + 90f);
        }

        Quaternion rot = Quaternion.Euler(0, 0, gravityAngle);
        Vector2 right = rot * Vector2.right;
        Vector2 up = rot * Vector2.up;

        float horizontal = 0f;
        if (kb.aKey.isPressed)
        {
            horizontal -= 1f;
        }
        if (kb.dKey.isPressed)
        {
            horizontal += 1f;
        }

        float verticalSpeed = Vector2.Dot(rb.linearVelocity, up);
        rb.linearVelocity = right * (horizontal * speed) + up * verticalSpeed;

        if (isGround && kb[jumpKey].wasPressedThisFrame)
        {
            Vector2 jumpDir = rot * jumpFoward;
            rb.AddForce(jumpDir * jumpPower, ForceMode2D.Impulse);
            isGround = false;
        }

        if (transform.position.y < -6)
        {
            Respawn();
        }

        if (horizontal > 0 && !facingRight) Flip();
        if (horizontal < 0 && facingRight) Flip();
    }

    void SetGravityAngle(float angle)
    {
        gravityAngle = Mathf.Repeat(angle, 360f);
        Quaternion rot = Quaternion.Euler(0, 0, gravityAngle);

        Physics2D.gravity = (Vector2)(rot * Vector2.down) * gravityStrength;

        rb.rotation = gravityAngle;

        rb.linearVelocity = Vector2.zero;
        isGround = false;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Vector2.Dot(rb.linearVelocity, Up) <= 0.1f)
            CheckGround(collision);
    }

    void CheckGround(Collision2D collision)
    {
        if (!(collision.gameObject.CompareTag("Ground")))
            return;

        foreach (var contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Up) > 0.5f)
            {
                isGround = true;
                return;
            }
        }
    }

    public void Respawn()
    {
        SetGravityAngle(0f);
        transform.position = playerPos;
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }
}