using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiePanel : MonoBehaviour
{
    public BikesControlerSystem bikesControler;
    public TMP_Text infText;
    public string[] listOfInformation;
    public Transform player;
    public PlayerHealth healthSystem;

    int actualText;

    Vector3 initPos;

    bool done;
    void Start()
    {
        healthSystem = bikesControler.healthSystem;
        player = bikesControler.atualBike.transform;
        initPos = player.position;
    }


    void Update()
    {
        if (healthSystem == null || player == null)
        {
            healthSystem = bikesControler.healthSystem;
            player = bikesControler.atualBike.transform;
        }
        infText.text = listOfInformation[actualText];

        if (ResetSystem.resetPlayer)
        {
            player.position = healthSystem.initPos;
           // player.rotation = healthSystem.initQuat;
           if(healthSystem.currentHealth <= 0.5f)
           {
                healthSystem.RestHealth();
           }
        }

        if (!healthSystem.die)
        {
            done = false;
        }
    }


    void LateUpdate()
    {
        if (healthSystem.die && !done)
        {
            actualText = Random.Range(0, listOfInformation.Length - 1);
            done = true;
        }
    }

}
