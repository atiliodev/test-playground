using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public BikesControlerSystem bikesControler;
    public Slider healthBar;
    public float maxFallDamage = 30f;
    public float fallDamageMultiplier = 0.1f;
    public float impactDamageMultiplier = 0.1f;
    public BikeController bikeSystem;
    public GameObject DiePanel;
    public string refName = "SetBikes";

    bool getDamage;

    [HideInInspector] public bool die;
    [HideInInspector] public Vector3 initPos;
    [HideInInspector] public Quaternion initQuat;
    public GameObject atualObj;

    bool waitSet;
    public bool resetHealth;
    [HideInInspector] public bool onDamage;

    private void Start()
    {
        bikeSystem = GetComponent<BikeController>();
        initQuat = bikeSystem.transform.rotation;
        initPos = bikeSystem.transform.position;
        if (resetHealth)
        {
            PlayerPrefs.DeleteKey("health");
            resetHealth = false;
        }

        if (PlayerPrefs.HasKey("health"))
        {
            currentHealth = PlayerPrefs.GetFloat("health");
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    private void Update()
    {
        if ((bikeSystem == null || bikesControler == null || healthBar == null) && !waitSet)
        {
            StartCoroutine("CheckApply");

            bikeSystem = GetComponent<BikeController>();

            waitSet = true;
        }


        onDamage = bikeSystem.crashed;

        DiePanel.SetActive(die);

        if ((bikeSystem.crashed || GetComponent<FreestyleSystem>().isImpactAboveThreshold && !getDamage))
        {
            DamageHealth();
            getDamage = true;
        }
        else if (getDamage && !(bikeSystem.crashed || GetComponent<FreestyleSystem>().isImpactAboveThreshold && !getDamage))
        {
            getDamage = false;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            die = true;

            // ✅ DESATIVA OS SCRIPTS ANTIGOS DA MOTO
            if (bikeSystem != null)
            {
                RiderControler rider = bikeSystem.GetComponentInChildren<RiderControler>();
                if (rider != null)
                    rider.enabled = false;

                BikeController controller = bikeSystem.GetComponent<BikeController>();
                if (controller != null)
                    controller.enabled = false;
            }
        }
        else
        {
            die = false;
        }
        healthBar.value = currentHealth;
    }

    IEnumerator CheckApply()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.EndsWith(refName) || obj.name.EndsWith(refName + "(Clone)"))
            {
                atualObj = obj;
            }
        }
        bikesControler = atualObj.GetComponent<BikesControlerSystem>();
        healthBar = bikesControler.slider;
        DiePanel = bikesControler.diePanel;
        waitSet = false;
        yield return new WaitForSeconds(2);
    }

    void DamageHealth()
    {
        bool done = false;
        if (!done && currentHealth > 0)
        {
            currentHealth -= maxFallDamage * impactDamageMultiplier + fallDamageMultiplier;
            done = true;
        }
    }



    public void RestHealth()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {

        //Vector3 relativeVelocity = collision.relativeVelocity;
        // float impactForce = relativeVelocity.magnitude;


        //float collisionForce = collision.impulse.magnitude;

        //fallDamageMultiplier = collisionForce / 12;
        /*
        if (collisionForce >= 15)
        {
            impactDamageMultiplier = 0.53f * impactForce / 6;
        }
        else
        {
            impactDamageMultiplier = 0.3f * impactForce / 6;
        }

        if(currentHealth >= 3)
        {
           // PlayerPrefs.SetFloat("health", currentHealth);
           // PlayerPrefs.Save();
        }
        else
        {
           /// PlayerPrefs.DeleteKey("health");
        }*/
    }
}
