using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeInstantiate : MonoBehaviour
{
    public GameObject bikePrefab; // Prefab da moto que será instanciada
    public Transform garageSpawnPoint; // Ponto de spawn na garagem
    public PlayerHealth playerHealth; // Referência ao script de saúde do jogador
    public delegate void BikeInstantiated(GameObject newBike);
    public static event BikeInstantiated OnBikeInstantiated;
    private GameObject currentBike; // Referência à moto instanciada
    
    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>(); // Tenta encontrar o PlayerHealth na cena se não estiver atribuído
        }

        // Instancia a moto na garagem no início do jogo, se necessário
        InstantiateBike(garageSpawnPoint.position, garageSpawnPoint.rotation);
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            // Instancia uma nova moto na garagem se a saúde do jogador zerou
            RespawnBike(garageSpawnPoint.position, garageSpawnPoint.rotation);
        }
    }

    public void HandleMinorFall(Vector3 fallPosition)
    {
        // Remove a moto atual e instancia uma nova próxima ao local da queda
        RemoveCurrentBike();
        InstantiateBike(fallPosition, Quaternion.identity);
    }

    // Método para remover a moto atual
    void RemoveCurrentBike()
    {
        if (currentBike != null)
        {
            Destroy(currentBike); // Remove a moto atual
            currentBike = null; // Garantir que a moto atual não seja referenciada novamente
        }
    }

    // Método para instanciar uma nova moto
    void InstantiateBike(Vector3 position, Quaternion rotation)
{
    currentBike = Instantiate(bikePrefab, position, rotation);
    currentBike.tag = "Player";
    Debug.Log("Nova moto instanciada na posição: " + position);

    // ✅ Atualiza as referências para os scripts da nova moto
    RiderControler riderControl = currentBike.GetComponentInChildren<RiderControler>();
    BikeController bikeController = currentBike.GetComponent<BikeController>();
    PlayerHealth playerHealth = currentBike.GetComponent<PlayerHealth>();

    // 🔔 Notifica os ouvintes
    OnBikeInstantiated?.Invoke(currentBike);
}

    // Método para instanciar a moto na garagem
    void RespawnBike(Vector3 position, Quaternion rotation)
    {
        RemoveCurrentBike(); // Remove a moto atual antes de instanciar uma nova
        InstantiateBike(position, rotation); // Instancia a nova moto na garagem
    }
}