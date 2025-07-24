using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionB : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    void Start()
    {
        // Armazena a posição e a rotação iniciais do objeto
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Obtém a referência ao Rigidbody, se houver
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Verifica se a tecla "T" foi pressionada
        if (Input.GetKeyDown(KeyCode.T))
        {
           // ResetObject();
        }
    }

    void ResetObject()
    {
        // Redefine a posição e a rotação do objeto
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Se houver um Rigidbody, redefine a velocidade e a rotação
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

