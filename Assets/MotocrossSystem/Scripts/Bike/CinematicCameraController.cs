using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CinematicCameraController : MonoBehaviour
{
    [SerializeField] private List<Camera> cinematicCameras; // Lista de c�meras cinem�ticas no mapa
    [SerializeField] private KeyCode cinematicKey = KeyCode.C; // Tecla para ativar c�meras cinem�ticas
    [SerializeField] private Transform playerTransform; // Refer�ncia para o transform do jogador (a moto)
    
    private Camera activeCinematicCamera; // C�mera cinem�tica atualmente ativa
    private CameraSystem cameraSystem; // Sistema de c�meras principal

    private void Start()
    {
        // Desativa todas as c�meras cinematogr�ficas no in�cio
        foreach (var camera in cinematicCameras){
            if(camera != null)
            camera.gameObject.SetActive(false);
        }

        // Encontra o sistema de c�meras principal
        cameraSystem = FindObjectOfType<CameraSystem>();

        if (cameraSystem == null)
        {
            Debug.LogError("CameraSystem principal n�o encontrado! Verifique se ele est� na cena.");
        }

        // Tenta buscar o transform da moto (player) no in�cio
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform; // Busca o objeto com a tag "Player"
            if (playerTransform == null)
            {
                Debug.LogError("N�o foi poss�vel encontrar o player (moto) com a tag 'Player'.");
            }
        }
    }

    private void Update()
    {
        // Atualiza a refer�ncia ao playerTransform caso a moto tenha sido trocada ou instanciada
        UpdatePlayerTransform();

        // Ativa a c�mera cinematogr�fica mais pr�xima quando a tecla � pressionada
        if (Input.GetKeyDown(cinematicKey))
        {
            ActivateClosestCinematicCamera();
        }
        // Retorna � c�mera principal quando a tecla � solta
        else if (Input.GetKeyUp(cinematicKey))
        {
            ResetToMainCamera();
        }
    }

    private void UpdatePlayerTransform()
    {
        // Se o playerTransform estiver nulo, tenta buscar a moto com a tag "Player"
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform; 
        }
    }

    private void ActivateClosestCinematicCamera()
    {
        if (playerTransform == null)
        {
            Debug.LogError("PlayerTransform n�o encontrado! A c�mera n�o pode ser ativada.");
            return;
        }

        float closestDistance = Mathf.Infinity; // Inicializa a dist�ncia mais pr�xima com um valor alto
        Camera closestCamera = null; // Armazena a refer�ncia da c�mera mais pr�xima

        // Percorre todas as c�meras cinem�ticas e calcula a dist�ncia entre a moto (player) e cada c�mera
        foreach (var camera in cinematicCameras)
        {
            float distance = Vector3.Distance(camera.transform.position, playerTransform.position); // Calcula a dist�ncia entre a moto e a c�mera
            Debug.Log($"Dist�ncia at� {camera.name}: {distance}"); // Log para depura��o da dist�ncia

            // Se a dist�ncia atual for menor que a dist�ncia mais pr�xima, atualiza a c�mera mais pr�xima
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCamera = camera;
            }
        }

        // Se uma c�mera mais pr�xima for encontrada, ativa ela
        if (closestCamera != null)
        {
            // Desativa a c�mera cinem�tica anterior, se houver
            if (activeCinematicCamera != null)
            {
                activeCinematicCamera.gameObject.SetActive(false);
            }

            // Ativa a c�mera mais pr�xima
            closestCamera.gameObject.SetActive(true);
            activeCinematicCamera = closestCamera;

            Debug.Log($"C�mera cinem�tica ativada: {closestCamera.name}"); // Log para depura��o da c�mera ativada

            // Desativa a c�mera principal do jogo, se necess�rio
            if (cameraSystem != null)
            {
               // cameraSystem.DeactivateCurrentCamera();
            }
        }
        else
        {
            Debug.LogWarning("Nenhuma c�mera cinematogr�fica encontrada.");
        }
    }

    private void ResetToMainCamera()
    {
        // Desativa a c�mera cinem�tica atual
        if (activeCinematicCamera != null)
        {
            activeCinematicCamera.gameObject.SetActive(false);
            activeCinematicCamera = null;
        }

        // Reativa a c�mera principal do jogo
        if (cameraSystem != null)
        {
           // cameraSystem.ActivateMainCamera();
        }
    }
}
