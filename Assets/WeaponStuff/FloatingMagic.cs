using UnityEngine;

public class FloatingMagic : MonoBehaviour
{
    [Header("Magic Motion")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.1f;
    public float rotateSpeed = 0f; // Set to 50 to make it spin

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        // Gentle Bobbing
        float newY = startPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        // Gentle Rotation (Optional)
        if (rotateSpeed != 0)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }
}