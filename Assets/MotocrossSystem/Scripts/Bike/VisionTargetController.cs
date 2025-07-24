using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTargetController : MonoBehaviour
{
    public Transform motorcycle; // Refer�ncia para a moto
    public float positionDamping = 5f; // Damping para suavizar a posi��o
    public float rotationDamping = 5f; // Damping para suavizar a rota��o
    public float turnAdjustment = 2f; // Ajuste horizontal durante curvas
    public float wheelieAdjustment = -2f; // Ajuste vertical durante wheelies
    public float distanceInFront = 5f; // Dist�ncia � frente da moto
    public float heightOffset = 1f; // Offset vertical

    private Vector3 defaultPosition; // Posi��o padr�o do VisionTarget

    private void Start()
    {
        // Define a posi��o inicial padr�o do VisionTarget
        UpdateDefaultPosition();
    }

    private void Update()
    {
        // Verifica o �ngulo da moto para determinar se est� em curva ou wheelie
        bool isCurving = Mathf.Abs(motorcycle.eulerAngles.y) > 1f; // Ajuste conforme necess�rio
        bool isWheelie = Mathf.Abs(motorcycle.eulerAngles.x) > 10f; // Ajuste conforme necess�rio

        if (isCurving || isWheelie)
        {
            // Ajuste horizontal para curvas
            Vector3 turnOffset = motorcycle.right * (Mathf.Sin(motorcycle.eulerAngles.y * Mathf.Deg2Rad) * turnAdjustment);

            // Ajuste vertical para wheelies
            float currentHeightOffset = isWheelie ? wheelieAdjustment : 0;
            Vector3 targetPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + turnOffset + new Vector3(0, currentHeightOffset, heightOffset);

            // Atualiza a posi��o do VisionTarget
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
        }
        else
        {
            // Quando n�o h� curva ou wheelie, retorna � posi��o padr�o em frente � moto
            Vector3 targetPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + new Vector3(0, heightOffset, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
        }

        // Suaviza a rota��o para alinhar com a moto
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(motorcycle.forward, Vector3.up), rotationDamping * Time.deltaTime);
    }

    private void UpdateDefaultPosition()
    {
        // Define a posi��o padr�o do VisionTarget em frente � moto
        defaultPosition = motorcycle.position + (motorcycle.forward * distanceInFront) + new Vector3(0, heightOffset, 0);
    }
}
