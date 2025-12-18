using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetLook : MonoBehaviour
{
    [SerializeField] private InputActionReference lookAction;

    [Header("Tuning")]
    [SerializeField] private float mouseSensitivity = 0.08f;     // degrees per pixel-ish (tune)
    [SerializeField] private float stickSensitivity = 180f;      // degrees per second (tune)
    [SerializeField] private bool invertY = false;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch;

    private void OnEnable() => lookAction?.action?.Enable();
    private void OnDisable() => lookAction?.action?.Disable();

    private void Awake()
    {
        Vector3 e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = NormalizePitch(e.x);
    }

    private void LateUpdate()
    {
        if (lookAction == null) return;

        Vector2 look = lookAction.action.ReadValue<Vector2>();

        // Mouse delta is already "per-frame", stick is typically "per-second"
        bool usingMouse = Mouse.current != null && Mouse.current.delta.IsActuated();
        float dt = Time.deltaTime;

        float sens = usingMouse ? mouseSensitivity : stickSensitivity * dt;

        yaw += look.x * sens;
        pitch += (invertY ? look.y : -look.y) * sens;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private static float NormalizePitch(float x)
    {
        if (x > 180f) x -= 360f;
        return x;
    }
}
