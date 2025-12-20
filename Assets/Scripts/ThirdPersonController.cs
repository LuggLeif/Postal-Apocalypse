using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    [Header("Input (Input System)")]
    [SerializeField] private InputActionReference moveAction;   // Vector2
    [SerializeField] private InputActionReference runAction;    // Button (optional)

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 5.5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float gravity = -20f;

    [Header("Rotation")]
    [SerializeField] private float turnSmoothTime = 0.08f;
    [SerializeField, Range(0f, 180f)] private float cameraTurnThreshold = 120f;
    [SerializeField] private float cameraTurnSpeed = 360f;

    [Header("Animator Params (optional)")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string movingParam = "IsMoving";
    [SerializeField] private bool setMovingBool = true;

    [Header("Turn-in-place Animation")]
    [Tooltip("Degrees/second where we START blending from idle -> walk while turning in place.")]
    [SerializeField] private float turnStartDegPerSec = 60f;

    [Tooltip("Degrees/second where we are FULLY at turnAnimSpeed.")]
    [SerializeField] private float turnFullDegPerSec = 180f;

    [Tooltip("Extra normalized speed to feed animator while turning in place (should land in your walk range).")]
    [SerializeField, Range(0f, 1f)] private float turnAnimSpeed = 0.35f;

    [Tooltip("Only apply turn-walk if current movement speed is below this.")]
    [SerializeField, Range(0f, 1f)] private float idleSpeedThreshold01 = 0.05f;

    [Tooltip("Smoothing time (seconds) for animator Speed parameter.")]
    [SerializeField] private float speedDampTime = 0.1f;

    private CharacterController controller;
    private float turnSmoothVelocity;
    private float verticalVelocity;
    private float currentSpeed;

    // Turning detection
    private float _lastYaw;
    private float _turnDegPerSec;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        _lastYaw = transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        moveAction?.action?.Enable();
        runAction?.action?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action?.Disable();
        runAction?.action?.Disable();
    }

    private void Update()
    {
        UpdateTurnRate(); // <- computes _turnDegPerSec each frame

        if (cameraTransform == null)
        {
            ApplyGravity();
            UpdateAnimator();
            return;
        }

        HandleMovementCameraRelative();
        ApplyGravity();
        UpdateAnimator();
    }

    private void UpdateTurnRate()
    {
        float yaw = transform.eulerAngles.y;
        float yawDelta = Mathf.DeltaAngle(_lastYaw, yaw);
        _lastYaw = yaw;

        _turnDegPerSec = Mathf.Abs(yawDelta) / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    private void HandleMovementCameraRelative()
    {
        Vector2 input = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        input = Vector2.ClampMagnitude(input, 1f);

        bool isMoving = input.sqrMagnitude > 0.0001f;
        bool isRunning = runAction != null && runAction.action.IsPressed();

        float targetSpeed = (isRunning ? runSpeed : walkSpeed) * input.magnitude;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Camera-relative direction on XZ
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x);
        if (moveDir.sqrMagnitude > 0.0001f)
            moveDir.Normalize();

        Vector3 horizontalMove = moveDir * currentSpeed;
        controller.Move((horizontalMove + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (isMoving)
        {
            float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);
        }
        else
        {
            AutoTurnToKeepCameraBehind();
        }
    }

    private void AutoTurnToKeepCameraBehind()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        if (camForward.sqrMagnitude < 0.0001f) return;
        camForward.Normalize();

        Vector3 playerForward = transform.forward;
        playerForward.y = 0f;
        if (playerForward.sqrMagnitude < 0.0001f) return;
        playerForward.Normalize();

        float angle = Vector3.Angle(playerForward, camForward);
        if (angle > cameraTurnThreshold)
        {
            Quaternion targetRot = Quaternion.LookRotation(camForward, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, cameraTurnSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float normalizedMoveSpeed = (runSpeed > 0f) ? Mathf.Clamp01(currentSpeed / runSpeed) : 0f;

        // Turning in place -> "fake" a little speed so walk anim plays while rotating
        float turn01 = Mathf.InverseLerp(turnStartDegPerSec, turnFullDegPerSec, _turnDegPerSec);
        float turningAnimSpeed = turn01 * turnAnimSpeed;

        // Only apply turning animation assist when basically idle
        float finalNormalizedSpeed = normalizedMoveSpeed;
        if (normalizedMoveSpeed < idleSpeedThreshold01)
            finalNormalizedSpeed = Mathf.Max(normalizedMoveSpeed, turningAnimSpeed);

        if (!string.IsNullOrEmpty(speedParam))
            animator.SetFloat(speedParam, finalNormalizedSpeed, speedDampTime, Time.deltaTime);

        if (setMovingBool && !string.IsNullOrEmpty(movingParam))
            animator.SetBool(movingParam, finalNormalizedSpeed > idleSpeedThreshold01);
    }
}
