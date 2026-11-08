using UnityEngine;

public class SawController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime * -0.5f);
    }
}
