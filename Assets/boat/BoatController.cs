using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Engine & Speed")]
    public float motorForce = 4500f;
    public float reverseForce = 2000f;
    public float maxSpeed = 12f;

    [Header("Steering (ความลื่นไหลในการเลี้ยว)")]
    [Tooltip("ความเร็วในการหันหัวเรือ (องศา/วินาที)")]
    public float turnSpeed = 80f;
    [Tooltip("ช่วยให้เรือค่อยๆ คืนพวงมาลัย ไม่หักเลี้ยวกระชาก")]
    public float turnSmoothness = 6f;

    [Header("Tilt / Waves Adaptation")]
    public float maxTiltAngle = 16f;
    public float uprightForce = 6f;

    private Rigidbody rb;
    private Vector2 inputVector;
    private float currentYaw;
    private float steerVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentYaw = transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void Update()
    {
        ReadInputs();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplySmoothSteeringAndTilt();
    }

    private void ReadInputs()
    {
        if (moveAction != null)
        {
            inputVector = moveAction.action.ReadValue<Vector2>();
            return;
        }

        Vector2 directInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) directInput.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) directInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) directInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) directInput.x += 1f;
        }
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.04f) directInput = stick;
        }
        inputVector = directInput;
    }

    private void ApplyMovement()
    {
        float forwardInput = inputVector.y;

        // ขับเคลื่อนตามระนาบขนานน้ำเสมอ
        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0f;
        forwardDir.Normalize();

        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (forwardInput > 0 && horizontalVel.magnitude < maxSpeed)
        {
            rb.AddForce(forwardDir * forwardInput * motorForce, ForceMode.Force);
        }
        else if (forwardInput < 0 && horizontalVel.magnitude < maxSpeed * 0.5f)
        {
            rb.AddForce(forwardDir * forwardInput * reverseForce, ForceMode.Force);
        }
    }

    private void ApplySmoothSteeringAndTilt()
    {
        float steerInput = inputVector.x;

        // 1. คำนวณอัตราทดการเลี้ยว: ยิ่งวิ่ง ยิ่งเลี้ยวได้คมขึ้น
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / 1.5f);
        float effectiveTurnSpeed = turnSpeed * (0.35f + 0.65f * speedFactor);

        // 2. คำนวณแกนหันหัวเรือ (Yaw) แบบ Smooth ไม่มีการสั่ง Force มาขัด
        currentYaw += steerInput * effectiveTurnSpeed * Time.fixedDeltaTime;

        // 3. จัดการแกนเอียงตามคลื่น (Pitch & Roll)
        Vector3 currentEuler = transform.eulerAngles;
        float pitch = NormalizeAngle(currentEuler.x);
        float roll = NormalizeAngle(currentEuler.z);

        float clampedPitch = Mathf.Clamp(pitch, -maxTiltAngle, maxTiltAngle);
        float clampedRoll = Mathf.Clamp(roll, -maxTiltAngle, maxTiltAngle);

        // ดึงการเอียงกลับสู่จุดปลอดภัยอย่างนุ่มนวล
        float smoothPitch = Mathf.Lerp(pitch, clampedPitch, Time.fixedDeltaTime * uprightForce);
        float smoothRoll = Mathf.Lerp(roll, clampedRoll, Time.fixedDeltaTime * uprightForce);

        // รวมร่างมุมทั้งหมดเข้าด้วยกันในคำสั่งเดียว ไม่มีการทับซ้อน
        rb.MoveRotation(Quaternion.Euler(smoothPitch, currentYaw, smoothRoll));

        // เคลียร์แรงหมุนแกนตกค้าง ป้องกันฟิสิกส์แอบสะสมแรงเหวี่ยง
        rb.angularVelocity = Vector3.zero;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}