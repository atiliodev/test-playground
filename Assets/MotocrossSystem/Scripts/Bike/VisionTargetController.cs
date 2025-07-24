using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTargetController : MonoBehaviour
{
    public Transform motorcycle; // Referência para a moto
    public float positionDamping = 5f; // Damping para suavizar a posição
    public float rotationDamping = 5f; // Damping para suavizar a rotação
    public float turnAdjustment = 2f; // Ajuste horizontal durante curvas
    public float wheelieAdjustment = -2f; // Ajuste vertical durante wheelies
    public float distanceInFront = 5f; // Distância à frente da moto
    public float heightOffset = 1f; // Offset vertical

    private Vector3 defaultPosition; // Posição padrão do VisionTarget

    private void Start()
    {
        // Define a posição inicial padrão do VisionTarget
        UpdateDefaultPosition();
    }

    private void Update()
    {
        // Verifica o ângulo da moto para determinar se está em curva ou wheelie
        bool isCurving = Mathf.Abs(motorcycle.eulerAngles.y) > 1f; // Ajuste conforme necessário
        bool isWheelie = Mathf.Abs(motorcycle.eulerAngles.x) > 10f; // Ajuste conforme necessário

        if (isCurving || isWheelie)
        {
            // Ajuste horizontal para curvas
            Vector3 turnOffset = motorcycle.right * (Mathf.Sin(motorcycle.eulerAngles.y * Mathf.Deg2Rad) * turnAdjustment);

            // Ajuste vertical para wheelies
            float currentHeightOffset = isWheelie ? wheelieAdjustment : 0;
            Vector3 targetPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + turnOffset + new Vector3(0, currentHeightOffset, heightOffset);

            // Atualiza a posição do VisionTarget
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
        }
        else
        {
            // Quando não há curva ou wheelie, retorna à posição padrão em frente à moto
            Vector3 targetPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + new Vector3(0, heightOffset, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
        }

        // Suaviza a rotação para alinhar com a moto
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(motorcycle.forward, Vector3.up), rotationDamping * Time.deltaTime);
    }

    private void UpdateDefaultPosition()
    {
        // Define a posição padrão do VisionTarget em frente à moto
        defaultPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + new Vector3(0, heightOffset, 0);
    }
}
