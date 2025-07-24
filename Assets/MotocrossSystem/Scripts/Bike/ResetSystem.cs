using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResetSystem : MonoBehaviour
{
    public static bool resetPlayer;
    public BikesControlerSystem bikesControler;
    public FreestyleSystem freestyleSystemBs;
    public Animator animCanva;
    bool wait;
    public float timeWait = 2f;
    public float factoryIncreaseValue = 2;
    public TextMeshProUGUI timeText;
    void Update()
    {


        if (freestyleSystemBs == null)
        {
            freestyleSystemBs = bikesControler.bikeSystem;
        }
        PlayerResetSystem();
    
    }
    private void FixedUpdate() {
        if (wait)
        {
            float waitTime = timeWait;

           // waitTime =- 1 * Time.deltaTime / 5f;
            timeText.text = "Restarting at; " + waitTime.ToString("f0") + "s";
        }
        else
        {
            timeText.text = " ";
        }

        	
        
    }
    void PlayerResetSystem()
    {
        if (freestyleSystemBs.bikeAnim != null && freestyleSystemBs.bkController.crashed && !wait)
        {
            
            StartCoroutine("WaitAnother");

            wait = true;

        }
    }
    IEnumerator WaitAnother()
    {
        yield return new WaitForSeconds(timeWait);
        resetPlayer = true;
        animCanva.SetBool("PlayRest", true);
        yield return new WaitForSeconds(1f);
        animCanva.SetBool("PlayRest", false);
        resetPlayer = false;
        wait = false;
    }
}
