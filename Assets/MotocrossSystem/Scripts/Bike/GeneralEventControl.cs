using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEventControl : MonoBehaviour
{
    public GameObject instantReset;
    public GameObject baseReload;
    public ClockDisplay clock;
    public RaceDisplayInfo raceInfo;
    public TricksScore scoreInfo;
    public EventController eventController;
    public ScoreDataDisplay scoreDisplay;
    public SplashInfoWithXp splashInfo;


    public Animator resetCanva;
    public Transform t_respawn;
    public BikesControlerSystem bike;

    bool stopVerification;
    bool win;
    void Update()
    {
        if(clock.timeEnd && !stopVerification)
        {
            if(raceInfo.thePlayerIsTheFirst)
            {
                win = true;
                scoreInfo.FinishWin();
                StartCoroutine("FinishEverthing");
                stopVerification = true;
            }
            else
            {
                
                StartCoroutine("FinishEverthing");
                stopVerification = true;
            }
        }
    }

    bool haveWin;

    IEnumerator FinishEverthing()
    {
        yield return new WaitForSeconds(3);
        if (win)
        {
            eventController.FinishWin();
            eventController.enabled = false;
            haveWin = true;
            win = false;
        }
        else if(!haveWin)
        {
            eventController.FinishLose();
            scoreInfo.FinishLost();
        }
        scoreInfo.enabled = false;
        yield return new WaitForSeconds(3);
        

    }
     // Função que estava sendo chamada mas não existia
   
    public void StartForFreestyle()
    {
        haveWin = false;
        clock.gameObject.SetActive(true);
        raceInfo.gameObject.SetActive(true);
        scoreDisplay.gameObject.SetActive(true);
    }

    public void StartForFoamPit()
    {
        haveWin = false;
        clock.gameObject.SetActive(false);
        raceInfo.gameObject.SetActive(false);
        scoreDisplay.gameObject.SetActive(true);
    }

    public void StartForRace()
    {
        haveWin = false;
        clock.gameObject.SetActive(false);
        raceInfo.gameObject.SetActive(true);
        scoreDisplay.gameObject.SetActive(true);
    }

    // Coloca isso como variável da classe (fora de qualquer método)
bool x = false;

public void Finish()
{
    StartCoroutine(stopSet());

    // ⚠️ Esse trecho estava comentado. Se precisar usar, é só descomentar!
    /*
    FadeControl.StartFade();
    clock.RestartTime();
    clock.gameObject.SetActive(false);
    raceInfo.gameObject.SetActive(false);
    scoreDisplay.gameObject.SetActive(false);
    stopVerification = false;
    */
    }

    // Agora o método está fora de Finish(), como deve ser
    IEnumerator stopSet()
    {
    if (!x)
    {
            EventInstantiate.ResetEvent();
        x = true;
    }        

    Destroy(baseReload, 0.5f);
    yield return new WaitForSeconds(2);

    splashInfo.EndDisplay();
    resetCanva.SetBool("PlayRest", true);
    resetCanva.SetBool("PlayRest", false);

    // Descomente se quiser resetar a posição da moto
    // bike.atualBike.transform.position = t_respawn.position;
   }
}