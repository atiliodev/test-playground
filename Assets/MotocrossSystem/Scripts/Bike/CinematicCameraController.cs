using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CinematicCameraController : MonoBehaviour
{
    [SerializeField] private List<Camera> cinematicCameras; // Lista de câmeras cinemáticas no mapa
    [SerializeField] private KeyCode cinematicKey = KeyCode.C; // Tecla para ativar câmeras cinemáticas
    [SerializeField] private Transform playerTransform; // Referência para o transform do jogador (a moto)
    
    private Camera activeCinematicCamera; // Câmera cinemática atualmente ativa
    private CameraSystem cameraSystem; // Sistema de câmeras principal

    private void Start()
    {
        // Desativa todas as câmeras cinematográficas no início
        foreach (var camera in cinematicCameras){
            if(camera != null)
            camera.gameObject.SetActive(false);
        }

        // Encontra o sistema de câmeras principal
        cameraSystem = FindObjectOfType<CameraSystem>();

        if (cameraSystem == null)
        {
            Debug.LogError("CameraSystem principal não encontrado! Verifique se ele está na cena.");
        }

        // Tenta buscar o transform da moto (player) no início
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform; // Busca o objeto com a tag "Player"
            if (playerTransform == null)
            {
                Debug.LogError("Não foi possível encontrar o player (moto) com a tag 'Player'.");
            }
        }
    }

    private void Update()
    {
        // Atualiza a referência ao playerTransform caso a moto tenha sido trocada ou instanciada
        UpdatePlayerTransform();

        // Ativa a câmera cinematográfica mais próxima quando a tecla é pressionada
        if (Input.GetKeyDown(cinematicKey))
        {
            ActivateClosestCinematicCamera();
        }
        // Retorna à câmera principal quando a tecla é solta
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
            Debug.LogError("PlayerTransform não encontrado! A câmera não pode ser ativada.");
            return;
        }

        float closestDistance = Mathf.Infinity; // Inicializa a distância mais próxima com um valor alto
        Camera closestCamera = null; // Armazena a referência da câmera mais próxima

        // Percorre todas as câmeras cinemáticas e calcula a distância entre a moto (player) e cada câmera
        foreach (var camera in cinematicCameras)
        {
            float distance = Vector3.Distance(camera.transform.position, playerTransform.position); // Calcula a distância entre a moto e a câmera
            Debug.Log($"Distância até {camera.name}: {distance}"); // Log para depuração da distância

            // Se a distância atual for menor que a distância mais próxima, atualiza a câmera mais próxima
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCamera = camera;
            }
        }

        // Se uma câmera mais próxima for encontrada, ativa ela
        if (closestCamera != null)
        {
            // Desativa a câmera cinemática anterior, se houver
            if (activeCinematicCamera != null)
            {
                activeCinematicCamera.gameObject.SetActive(false);
            }

            // Ativa a câmera mais próxima
            closestCamera.gameObject.SetActive(true);
            activeCinematicCamera = closestCamera;

            Debug.Log($"Câmera cinemática ativada: {closestCamera.name}"); // Log para depuração da câmera ativada

            // Desativa a câmera principal do jogo, se necessário
            if (cameraSystem != null)
            {
               // cameraSystem.DeactivateCurrentCamera();
            }
        }
        else
        {
            Debug.LogWarning("Nenhuma câmera cinematográfica encontrada.");
        }
    }

    private void ResetToMainCamera()
    {
        // Desativa a câmera cinemática atual
        if (activeCinematicCamera != null)
        {
            activeCinematicCamera.gameObject.SetActive(false);
            activeCinematicCamera = null;
        }

        // Reativa a câmera principal do jogo
        if (cameraSystem != null)
        {
           // cameraSystem.ActivateMainCamera();
        }
    }
}
