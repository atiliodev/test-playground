using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonBikeCamera : MonoBehaviour
{
    [Header("Referências")]
    public Transform bikeBase;        // Estrutura estável da moto
    public Transform pilotBody;       // Parte móvel do piloto (ex: cabeça, tronco)
    public Vector3 baseOffset = new Vector3(0, 1.2f, 0.3f); // Posição base da câmera em relação à moto

    [Header("Comportamento")]
    [Range(0, 1)] public float pilotInfluence = 0.3f; // Quanto a câmera será influenciada pelos movimentos do corpo
    public float positionSmooth = 5f;
    public float rotationSmooth = 6f;

    [Header("Ajustes Manuais")]
    public bool enableRotationOffset = true;              // Ativar ou não o ajuste manual de rotação
    public Vector3 baseRotationOffset = Vector3.zero;     // Rotação fixa ajustável no Inspector

    private Vector3 velocity;

    public Ragdoll_Instant bike;
    public Camera cam;

    private void Start()
    {
        bike = GameObject.FindGameObjectWithTag("Player").GetComponent<Ragdoll_Instant>();
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (bike == null)
        {
            bike = GameObject.FindGameObjectWithTag("Player").GetComponent<Ragdoll_Instant>();
        }
        else
        {
            cam.enabled = !bike.done;
        }
    }

    void LateUpdate()
    {
        if (!bikeBase || !pilotBody) return;

        // --- POSIÇÃO ---
        Vector3 basePosition = bikeBase.TransformPoint(baseOffset);
        Vector3 bodyPosition = pilotBody.position;
        Vector3 targetPosition = Vector3.Lerp(basePosition, bodyPosition, pilotInfluence);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / positionSmooth);

        // --- ROTAÇÃO ---
        Quaternion baseRotation = bikeBase.rotation;
        Quaternion bodyRotation = pilotBody.rotation;
        Quaternion targetRotation = Quaternion.Slerp(baseRotation, bodyRotation, pilotInfluence);

        // Aplica rotação fixa se ativada
        if (enableRotationOffset)
        {
            targetRotation *= Quaternion.Euler(baseRotationOffset);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);
    }
}