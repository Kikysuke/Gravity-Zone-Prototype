using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoorSystem : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveOffset =
        new Vector2(0f, 3f);

    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 closedPosition;
    private Vector2 openPosition;
    private bool isOpen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        closedPosition = rb.position;
        openPosition = closedPosition + moveOffset;
    }

    public void Open()
    {
        SetOpen(true);
    }

    public void SetOpen(bool value)
    {
        isOpen = value;
    }

    private void FixedUpdate()
    {
        Vector2 target =
            isOpen ? openPosition : closedPosition;

        rb.MovePosition(Vector2.MoveTowards(
            rb.position,
            target,
            moveSpeed * Time.fixedDeltaTime
        ));
    }
}