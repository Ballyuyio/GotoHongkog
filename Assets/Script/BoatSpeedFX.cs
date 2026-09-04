using UnityEngine;
using UnityEngine.InputSystem;

public class BoatSpeedFX : MonoBehaviour
{
    public Rigidbody boatRb;
    public ParticleSystem wakeParticles;
    public ParticleSystem bowSplashParticles;

    [Header("Speed Settings")]
    public float minSpeed = 0.05f;
    public float maxSpeed = 3f;

    [Header("Emission Scaling")]
    public float maxWakeRate = 80f;
    public float maxWakeSize = 1.5f;
    public float minWakeSize = 0.5f;

    private ParticleSystem.EmissionModule wakeEmission;
    private ParticleSystem.MainModule wakeMain;

    void Start()
    {
        if (boatRb == null) boatRb = GetComponentInParent<Rigidbody>();

        if (wakeParticles != null)
        {
            wakeEmission = wakeParticles.emission;
            wakeMain = wakeParticles.main;
            wakeEmission.enabled = true;
        }
    }

    void Update()
    {
        if (boatRb == null || wakeParticles == null) return;

        // เช็กทั้งความเร็วจริง และการกดปุ่มเคลื่อนที่
        float currentSpeed = new Vector3(boatRb.linearVelocity.x, 0f, boatRb.linearVelocity.z).magnitude;
        bool isPressingGas = Keyboard.current != null && (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed);

        if (currentSpeed > minSpeed || isPressingGas)
        {
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            wakeEmission.rateOverTime = Mathf.Lerp(20f, maxWakeRate, speedFactor);
            wakeMain.startSize = Mathf.Lerp(minWakeSize, maxWakeSize, speedFactor);
        }
        else
        {
            wakeEmission.rateOverTime = 0f;
        }
    }
}