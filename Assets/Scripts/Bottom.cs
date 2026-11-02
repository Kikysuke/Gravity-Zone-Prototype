using UnityEngine;

public class Bottom : MonoBehaviour
{
    [SerializeField] private DoorSystem targetDoor;

    private bool isPressed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPressed) return;
        if (!other.CompareTag("Player")) return;

        if (targetDoor == null)
        {
            //ターゲットがない時にログにエラーを表示する。
            Debug.LogWarning("動かす壁を設定してください。", this);
            return;
        }

        if (!targetDoor.CompareTag("Door")) return;

        isPressed = true;
        targetDoor.Open();
    }
}