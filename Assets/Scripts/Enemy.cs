using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDefeated = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDefeated) return;

        // Playerタグ以外との衝突は無視される。
        if (!collision.gameObject.CompareTag("Player")) return;

        bool isStomp = false;

        // プレイヤーが敵に対して下方向に接近している
        if (collision.relativeVelocity.y < 0f)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D contact = collision.GetContact(i);

                // 敵の上面に接触している
                if (contact.normal.y < -0.5f)
                {
                    isStomp = true;
                    break;
                }
            }
        }

        if (isStomp)
        {
            // 踏みつけ成功：敵が消える
            isDefeated = true;
            Destroy(gameObject);
        }
        else
        {
            // 横や下から接触：Playerが消える
            Destroy(collision.gameObject);
        }
    }
}