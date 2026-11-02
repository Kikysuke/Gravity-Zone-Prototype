using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rb;
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed = 5.0f;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPoint = 0;
    }

    void Update()
    {
        if (transform.position == patrolPoints[targetPoint].position)
        {
            IncreaseTargetInt();
            Flip();
        }
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterMovement player = collision.gameObject.GetComponent<CharacterMovement>();
        if (player == null) return;

        Vector2 normal = Vector2.zero;
        foreach (var c in collision.contacts) normal += c.normal;
        normal.Normalize();

        Vector2 enemyToPlayer = collision.collider.bounds.center - collision.otherCollider.bounds.center;
        if (Vector2.Dot(normal, enemyToPlayer) < 0) normal = -normal;

        bool hitWithFeet = Vector2.Dot(normal, player.Up) > 0.7f;

        bool onEnemyTop = Vector2.Dot(normal, Vector2.up) > 0.7f;

        if (hitWithFeet && !onEnemyTop)
        {
            Destroy(gameObject);
        }
        else
        {
            player.Respawn();
        }
    }

    void IncreaseTargetInt()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length)
        {
            targetPoint = 0;
        }
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }
}