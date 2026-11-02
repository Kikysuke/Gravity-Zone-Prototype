using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushBox : MonoBehaviour
{
    [SerializeField] private float pushSpeed = 2f;

    // 床・壁・ほかの箱など、移動を遮るレイヤー
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody2D rb;
    private ContactFilter2D filter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[16];

    private float pushDirection;
    private float horizontalInput;
    private float lastPushTime = float.NegativeInfinity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(obstacleLayers);
        filter.useTriggers = false;
    }

    private void Update()
    {
        horizontalInput = 0f;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.aKey.isPressed ||
            keyboard.leftArrowKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (keyboard.dKey.isPressed ||
            keyboard.rightArrowKey.isPressed)
        {
            horizontalInput += 1f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        float input = horizontalInput;

        // Playerから箱へ向かう横方向
        float toBoxX =
            rb.worldCenterOfMass.x -
            collision.collider.bounds.center.x;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;

            bool sideContact = Mathf.Abs(normal.x) > 0.5f;
            bool pushingTowardBox = input * toBoxX > 0f;

            Debug.Log(
    $"箱={name}, 相手={collision.gameObject.name}, " +
    $"入力={input}, " +
    $"法線X={normal.x:F3}, 法線Y={normal.y:F3}, " +
    $"接触位置={collision.GetContact(i).point}, " +
    $"横接触={sideContact}, 箱へ入力={pushingTowardBox}"
);

            if (sideContact && pushingTowardBox)
            {
                pushDirection = Mathf.Sign(input);
                lastPushTime = Time.fixedTime;
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        // 接触が続いていない場合は動かさない
        if (Time.fixedTime - lastPushTime >
            Time.fixedDeltaTime * 1.5f)
        {
            return;
        }

        Vector2 direction = Vector2.right * pushDirection;
        float distance = pushSpeed * Time.fixedDeltaTime;

        // 移動先に壁があれば、その手前で止める
        int count = rb.Cast(
            direction, filter, hits, distance + 0.02f
        );

        for (int i = 0; i < count; i++)
        {
            // 床など、移動を遮らない向きの面は除外
            if (Vector2.Dot(hits[i].normal, direction) < -0.5f)
            {
                distance = Mathf.Min(
                    distance,
                    Mathf.Max(0f, hits[i].distance - 0.02f)
                );
            }
        }

        rb.MovePosition(rb.position + direction * distance);
    }
}