using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public ScoreManager scoreManager;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        collected = true;

        scoreManager.AddScore(1);

        Destroy(gameObject);
    }
}