using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    public TypeOfEvent typeOfEvent;

    public GeneralEventControl managerEvent;
    public TricksScore scoreSystem;
    public ActiveControl activeControl;
    public EventTrigger trigger;
    public GameObject baseS;
    public GameObject baseSIfWin;
    public GameObject disableSIfWin;

    public UnityEvent thingWhenLose;

    bool isFoamPit;
    bool isFreestyle;
    bool isRace;
    public enum TypeOfEvent
    {
        freestyleFoamPitEvent,
        freestyleEvent,
        raceEvent
    };

    void Start()
    {
      switch(typeOfEvent)
        {
            case TypeOfEvent.freestyleFoamPitEvent:
                isFoamPit = true;
                break;
            case TypeOfEvent.freestyleEvent:
                isFreestyle = true;
                break;
            case TypeOfEvent.raceEvent:
                isRace = true;
                break;

        }
    }

    
    void Update()
    {
        
    }

    public void StartFreestyle()
    {
        if (isFreestyle)
        {
            managerEvent.StartForFreestyle();
            scoreSystem.enabled = true;
            trigger.enabled = false;
            baseS.SetActive(false);
            disableSIfWin.SetActive(true);
        }
    }

    public void FinishLose()
    {
        managerEvent.Finish();
        /*scoreSystem.enabled = false;
        trigger.enabled = true;
        baseS.SetActive(true);
        StartCoroutine(ToDisable());*/
    }

    IEnumerator ToDisable()
    {
        yield return new WaitForSeconds(2);
        thingWhenLose.Invoke();

    }

    public void FinishWin()
    {
        managerEvent.Finish();
        scoreSystem.enabled = false;
        baseSIfWin.SetActive(true);
        disableSIfWin.SetActive(false);
    }

    public void StartEvent()
    {
        if (isFoamPit)
        {
            managerEvent.StartForFoamPit();
            scoreSystem.enabled = true;
            trigger.enabled = false;
            baseS.SetActive(false);
            disableSIfWin.SetActive(true);
        }
    }
}
