using UnityEngine;

public class GravityCameraFollow : MonoBehaviour
{
    public CharacterMovement3D player;
    public Vector3 offset = new Vector3(0f, 1f, -15f);
    public float positionSmooth = 10f;
    public float rotationSmooth = 8f;

    void LateUpdate()
    {
        if (player == null) return;

        Quaternion targetRot = player.ViewRotation;
        Vector3 desiredPos = player.transform.position + targetRot * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPos,
            1f - Mathf.Exp(-positionSmooth * Time.deltaTime));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
            1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
    }
}