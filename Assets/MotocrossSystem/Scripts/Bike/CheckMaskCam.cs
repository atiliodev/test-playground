using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMaskCam : MonoBehaviour
{
 
    [Header("Referências")]
    public Transform cameraPivot; // Onde a câmera está presa, normalmente a cabeça
    public Transform cameraTransform; // A câmera propriamente dita

    [Header("Configurações")]
    public float cameraDistance = 0.5f; // Distância da câmera a partir do pivot
    public float minDistance = 0.1f; // Distância mínima da parede
    public LayerMask collisionLayers; // Camadas com as quais a câmera deve colidir

    private Vector3 desiredPosition;

    void LateUpdate()
    {
        Vector3 direction = cameraTransform.forward;
        RaycastHit hit;

        // Verifica se há colisão à frente da câmera
        if (Physics.Raycast(cameraPivot.position, direction, out hit, cameraDistance, collisionLayers))
        {
            // Se houver algo na frente, ajusta a posição da câmera para não atravessar
            float adjustedDistance = Mathf.Clamp(hit.distance - 0.05f, minDistance, cameraDistance);
            desiredPosition = cameraPivot.position + direction * adjustedDistance;
        }
        else
        {
            // Nenhuma colisão, posição normal
            desiredPosition = cameraPivot.position + direction * cameraDistance;
        }

        // Atualiza posição da câmera
        cameraTransform.position = desiredPosition;
    }
}


