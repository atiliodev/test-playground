using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMaskCam : MonoBehaviour
{
 
    [Header("Refer�ncias")]
    public Transform cameraPivot; // Onde a c�mera est� presa, normalmente a cabe�a
    public Transform cameraTransform; // A c�mera propriamente dita

    [Header("Configura��es")]
    public float cameraDistance = 0.5f; // Dist�ncia da c�mera a partir do pivot
    public float minDistance = 0.1f; // Dist�ncia m�nima da parede
    public LayerMask collisionLayers; // Camadas com as quais a c�mera deve colidir

    private Vector3 desiredPosition;

    void LateUpdate()
    {
        Vector3 direction = cameraTransform.forward;
        RaycastHit hit;

        // Verifica se h� colis�o � frente da c�mera
        if (Physics.Raycast(cameraPivot.position, direction, out hit, cameraDistance, collisionLayers))
        {
            // Se houver algo na frente, ajusta a posi��o da c�mera para n�o atravessar
            float adjustedDistance = Mathf.Clamp(hit.distance - 0.05f, minDistance, cameraDistance);
            desiredPosition = cameraPivot.position + direction * adjustedDistance;
        }
        else
        {
            // Nenhuma colis�o, posi��o normal
            desiredPosition = cameraPivot.position + direction * cameraDistance;
        }

        // Atualiza posi��o da c�mera
        cameraTransform.position = desiredPosition;
    }
}


