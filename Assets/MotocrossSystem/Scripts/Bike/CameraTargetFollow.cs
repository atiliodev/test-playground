using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public BikesControlerSystem bikesControler;
    public Transform bikeTransform;       // Transform da moto
    public Vector3 offset;                // Deslocamento em relação à moto
    public float followSpeed = 10f;       // Velocidade de seguimento
    public float rotationSmoothTime = 0.1f; // Suavização de rotação
    public float heightDamping = 0.2f;    // Suavização de altura
    public float maxHeightOffset = 5f;    // Deslocamento máximo em altura para manter a moto visível
    public GameObject ragdoll;
    private Vector3 currentVelocity;

    private void Update() {
        if (ragdoll == null)
        {
            ragdoll = GameObject.FindGameObjectWithTag("ragdoll");
        }


        if(bikesControler.bikeController.
            
            
            
            
            ed && ragdoll != null)
        {
            bikeTransform = ragdoll.transform;
        }
        else
        {
            
            bikeTransform = bikesControler.atualBike.transform;
        }
    }

    void LateUpdate()
    {
        
        
        if (bikeTransform != null)
        {
            // Calcula a posição alvo da câmera com o offset
            Vector3 targetPosition = bikeTransform.position + offset;

            // Suaviza a altura da câmera para evitar tremores
            float targetHeight = Mathf.Clamp(bikeTransform.position.y + offset.y,
                                             bikeTransform.position.y - maxHeightOffset,
                                             bikeTransform.position.y + maxHeightOffset);

            // Suaviza a transição da altura para evitar tremores
            float smoothedHeight = Mathf.Lerp(transform.position.y, targetHeight, heightDamping);
            targetPosition.y = smoothedHeight;

            // Move a câmera suavemente para a posição alvo
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Suaviza a rotação da câmera para evitar tremores na direção
            Quaternion targetRotation = Quaternion.LookRotation(bikeTransform.position - transform.position);

            // Aplica a rotação suavizada
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothTime * Time.deltaTime);
        }
    }

    // Desenha raios no editor para depuração visual
    void OnDrawGizmos()
    {
        if (bikeTransform != null)
        {
            // Desenha um raio na direção da moto
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, bikeTransform.position);
        }
    }
}
