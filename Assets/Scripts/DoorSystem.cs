using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoorSystem : MonoBehaviour
{
    [SerializeField] private Vector2 moveOffset = new Vector2(0f, 0f);
    [SerializeField, Min(0.01f)] private float moveSpeed = 0f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isOpen = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // èâä˙à íuÇ©ÇÁÇ«ÇÍÇæÇØìÆÇ©Ç∑Ç©ÇÃïœêî
        targetPosition = rb.position + moveOffset;
    }

    public void Open()
    {
        isOpen = true;
    }

    private void FixedUpdate()
    {
        if (!isOpen) return;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }
}
