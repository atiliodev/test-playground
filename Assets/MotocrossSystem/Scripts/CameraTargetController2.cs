using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetController2 : MonoBehaviour
{
    public Transform bike;  // Refer�ncia � moto
    public float followSpeed = 5f;  // Velocidade de suaviza��o para seguir a moto
    public float rotationSpeed = 5f;  // Velocidade para suavizar a rota��o no eixo Y
    public Vector3 offset;  // Offset para posicionar o target fora da moto
    public string bikeTag = "Player";  // Tag usada para identificar a moto
    public LayerMask groundLayer;  // Camada do terreno para detec��o no solo

    private bool isAirborne;  // Para saber se a moto est� no ar ou no ch�o

    void Update()
    {
         if (bike == null)
        {
            FindNewBike();
        }
    }

    private void LateUpdate()
    {
        // Se a refer�ncia da moto estiver faltando (destru�da ou inexistente), procurar o novo GameObject da moto
       

        if (bike != null)
        {
            // Verificar se a moto est� no ar
            CheckIfAirborne();

            // Seguir a posi��o da moto com suaviza��o
            Vector3 targetPosition = bike.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Se a moto estiver no ch�o, suavizar a rota��o no eixo Y
            if (!isAirborne)
            {
                Vector3 direction = bike.forward;
                direction.y = 0;  // Ignorar rota��o em X e Z
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            // Se a moto estiver no ar, manter a rota��o est�vel
            else
            {
                // N�o ajustar a rota��o da c�mera, mantendo-a est�vel
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation, Time.deltaTime);
            }
        }
    }

    // M�todo para encontrar a nova moto
    void FindNewBike()
    {
        GameObject newBike = GameObject.FindWithTag(bikeTag);
        if (newBike != null)
        {
            bike = newBike.transform;  // Atualizar a refer�ncia da moto
        }
    }

    // Verificar se a moto est� no ar ou no solo
    void CheckIfAirborne()
    {
        // Verifica se a moto est� tocando o solo com um raycast abaixo da moto
        Ray ray = new Ray(bike.position, Vector3.down);
        RaycastHit hit;

        // Se o raycast n�o encontrar o solo, a moto est� no ar
        if (Physics.Raycast(ray, out hit, 1.5f, groundLayer))
        {
            isAirborne = false;  // A moto est� no ch�o
        }
        else
        {
            isAirborne = true;  // A moto est� no ar
        }
    }
}