using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform bikeTransform;
    public GameObject ragdoll;
    public Transform target; // O alvo que a câmera seguirá (a moto)
    public float distance = 5f; // Distância da câmera em relação ao alvo
    public float height = 2f;   // Altura da câmera em relação ao alvo
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento
    public Vector3 offset; // Offset para ajustar a posição da câmera em relação ao alvo
    public LayerMask collisionLayers; // Camadas para verificar colisões
    public float collisionBuffer = 0.5f; // Distância para evitar sobreposição após colisão

    private Vector3 currentDesiredPosition; // Posição desejada suavizada

    private void Update()
    {
        if (ragdoll == null)
        {
           // ragdoll = GameObject.FindGameObjectWithTag("ragdoll");
        }

        if (ragdoll != null)
        {
            bikeTransform = ragdoll.transform;
        }
        else
        {
            GameObject targetD = GameObject.FindGameObjectWithTag("Target");
            bikeTransform = targetD?.transform;
        }

        target = bikeTransform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Posição desejada da câmera em relação ao alvo
        Vector3 desiredPosition = target.position + target.TransformDirection(offset) - target.forward * distance + Vector3.up * height;

        // Verifica se há obstruções entre a câmera e o alvo
        RaycastHit hit;
        Vector3 directionToCamera = (desiredPosition - target.position).normalized;
        float distanceToCamera = Vector3.Distance(target.position, desiredPosition);

        // Faz um Raycast para verificar colisões
        if (Physics.Raycast(target.position, directionToCamera, out hit, distanceToCamera, collisionLayers))
        {
            // Ajusta a posição da câmera para evitar colisão
            desiredPosition = hit.point - directionToCamera * collisionBuffer;
        }

        // Suaviza a transição para a posição desejada (mesmo com colisões)
        currentDesiredPosition = Vector3.Lerp(currentDesiredPosition == Vector3.zero ? transform.position : currentDesiredPosition, desiredPosition, smoothSpeed);

        // Impede que a câmera vá para baixo do terreno
        currentDesiredPosition.y = Mathf.Max(currentDesiredPosition.y, target.position.y + 0.5f);

        // Atualiza a posição da câmera
        transform.position = currentDesiredPosition;

        // Faz a câmera olhar para o alvo
        transform.LookAt(target);
    }
}
