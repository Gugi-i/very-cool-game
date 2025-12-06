using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    [Header("Settings")]
    public float stickDeadzone = 0.1f;
    public float stickRange = 5f;

    [SerializeField] private Transform player; // assign player transform in inspector

    private Camera mainCam;
    private Vector2 aimInput;
    private Vector3 lastWorldPos;

    void Awake()
    {
        mainCam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        lastWorldPos = player.position;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Vector3 targetPos = lastWorldPos;

        if (Mouse.current != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 screenWithZ = new Vector3(mouseScreen.x, mouseScreen.y, Mathf.Abs(mainCam.transform.position.z));
            targetPos = mainCam.ScreenToWorldPoint(screenWithZ);
        }
        else if (aimInput.sqrMagnitude > stickDeadzone * stickDeadzone)
        {
            targetPos = player.position + (Vector3)(aimInput.normalized * stickRange);
        }

        lastWorldPos = targetPos;
        transform.position = lastWorldPos;
    }
}
