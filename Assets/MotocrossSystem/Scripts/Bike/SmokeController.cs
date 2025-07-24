using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmokeParticleSystemSettings
{
    public ParticleSystem particles;
    public float maxEmissionRate = 50f;
    public float minSpeedForEmission = 0.5f;
    public float minEmissionRate = 0f;
}

public class SmokeController : MonoBehaviour
{
    public SmokeParticleSystemSettings[] particleSystems;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;

    private Rigidbody rb;
    private WheelCollider[] wheels;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<WheelCollider>();
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        bool isAnyWheelOnGround = IsAnyWheelOnGround();
        bool isFrontWheelInAir = IsFrontWheelInAir();

        foreach (var ps in particleSystems)
        {
            var emission = ps.particles.emission;
            float emissionRate = 0f;
            bool shouldEmit = false;

            if (isFrontWheelInAir)
            {
                shouldEmit = speed > ps.minSpeedForEmission;
                emissionRate = shouldEmit ? ps.maxEmissionRate : 0f;
            }
            else if (isAnyWheelOnGround)
            {
                if (speed > ps.minSpeedForEmission)
                {
                    emissionRate = Mathf.Lerp(
                        ps.maxEmissionRate,
                        ps.minEmissionRate,
                        (speed - ps.minSpeedForEmission) / (10f - ps.minSpeedForEmission)
                    );
                }
                else
                {
                    emissionRate = ps.maxEmissionRate;
                }
                shouldEmit = true;
            }

            emission.rateOverTime = emissionRate;

            if (shouldEmit)
            {
                if (!ps.particles.isPlaying) ps.particles.Play();
            }
            else
            {
                if (ps.particles.isPlaying) ps.particles.Stop();
            }
        }
    }

    bool IsAnyWheelOnGround()
    {
        foreach (var wheel in wheels)
        {
            if (Physics.Raycast(GetWheelPosition(wheel), Vector3.down, groundCheckDistance, groundLayer))
                return true;
        }
        return false;
    }

    bool IsFrontWheelInAir()
    {
        if (wheels.Length == 0) return false;
        return !Physics.Raycast(GetWheelPosition(wheels[0]), Vector3.down, groundCheckDistance, groundLayer);
    }

    Vector3 GetWheelPosition(WheelCollider wheel)
    {
        wheel.GetWorldPose(out Vector3 pos, out _);
        return pos;
    }
}
