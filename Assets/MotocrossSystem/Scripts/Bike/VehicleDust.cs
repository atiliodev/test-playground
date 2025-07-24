using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleSystemSettings
{
    public ParticleSystem particles; // Refer�ncia ao sistema de part�culas
    public float minEmissionRate = 10f; // Taxa m�nima de emiss�o de part�culas
    public float maxEmissionRate = 50f; // Taxa m�xima de emiss�o de part�culas
    public float minSpeedForEmission = 0.5f; // Velocidade m�nima para come�ar a emitir poeira
}

public class VehicleDust : MonoBehaviour
{
    public ParticleSystemSettings[] particleSystems; // Configura��es dos sistemas de part�culas
    public LayerMask groundLayer; // Camada que representa o ch�o
    public float tractionRPMThreshold = 3000f; // Limite de RPM para considerar alta tra��o
    public float extraEmissionFactor = 2f; // Fator extra de emiss�o para alta tra��o

    private Rigidbody rb; // Refer�ncia ao Rigidbody do ve�culo
    private WheelCollider[] wheels; // Lista de colliders das rodas do ve�culo
    private WheelCollider rearWheel; // Refer�ncia ao WheelCollider da roda traseira
    private float currentRearWheelRPM; // RPM atual da roda traseira

    public float groundCheckDistance = 0.3f; // Dist�ncia de verifica��o de ch�o para cada roda

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<WheelCollider>();
        
        // Assume que a roda traseira � a segunda na lista de wheels
        if (wheels.Length > 1)
        {
            rearWheel = wheels[1];
        }
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        // Verifica se qualquer roda est� no ch�o
        bool isAnyWheelOnGround = IsAnyWheelOnGround();
        // Verifica se a roda da frente est� levantada (em um wheelie)
        bool isFrontWheelInAir = IsFrontWheelInAir();

        // Obt�m o RPM da roda traseira
        currentRearWheelRPM = GetRearWheelRPM();

        foreach (var ps in particleSystems)
        {
            var emission = ps.particles.emission;

            // Se a moto estiver no ch�o, n�o estiver em um wheelie e estiver em movimento
            if (isAnyWheelOnGround && !isFrontWheelInAir && speed > ps.minSpeedForEmission)
            {
                // Calcula a emiss�o de part�culas com base na velocidade
                float emissionRate = Mathf.Lerp(0f, ps.maxEmissionRate, (speed - ps.minSpeedForEmission) / (10f - ps.minSpeedForEmission));

                // Aumenta a emiss�o se o RPM da roda traseira estiver acima do limite de tra��o
                if (currentRearWheelRPM > tractionRPMThreshold)
                {
                    emissionRate *= extraEmissionFactor;
                    Debug.Log("Increased emission rate: " + emissionRate); // Verifique o valor de emiss�o
                }

                emission.rateOverTime = emissionRate;

                if (!ps.particles.isPlaying)
                {
                    ps.particles.Play();
                }
            }
            else
            {
                emission.rateOverTime = 0f;
                if (ps.particles.isPlaying)
                {
                    ps.particles.Stop();
                }
            }
        }
    }

    float GetRearWheelRPM()
    {
        if (rearWheel != null)
        {
            // Obt�m o RPM da roda traseira usando o motor torque e a rota��o da roda
            return rearWheel.rpm;
        }
        return 0f;
    }

    bool IsAnyWheelOnGround()
    {
        foreach (var wheel in wheels)
        {
            Vector3 wheelPos;
            Quaternion wheelRot;
            wheel.GetWorldPose(out wheelPos, out wheelRot);

            RaycastHit hit;
            if (Physics.Raycast(wheelPos, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                return true;
            }
        }

        return false;
    }

    bool IsFrontWheelInAir()
    {
        if (wheels.Length > 0)
        {
            var frontWheel = wheels[0];
            Vector3 wheelPos;
            Quaternion wheelRot;
            frontWheel.GetWorldPose(out wheelPos, out wheelRot);

            RaycastHit hit;
            if (!Physics.Raycast(wheelPos, Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                return true; // A roda da frente est� no ar
            }
        }

        return false;
    }
}
