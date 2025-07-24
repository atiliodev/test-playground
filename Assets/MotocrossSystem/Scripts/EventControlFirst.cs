using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventControlFirst : MonoBehaviour
{
    public GameObject toActive;
    public FreestyleEventSystemFreestyleFarm s_Freestyle;
    public EventTrigger eventTrigger;
    public GameObject otherActive;
    public GameObject objectsSet;
    

    public void StartEvent()
    {
        eventTrigger.enabled = false;
        toActive.SetActive(true);
        //s_Freestyle.callBack();
        objectsSet.SetActive(false);
    }

    public void EndEvent()
    {
        toActive.SetActive(false);
        //s_Freestyle.callBack();
        otherActive.SetActive(true);
        SplashInfoWithXp.ShowInfoXp("You Lost, Try Again!", 0);
    }

}
