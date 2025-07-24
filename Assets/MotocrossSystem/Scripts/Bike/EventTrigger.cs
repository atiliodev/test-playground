using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [TextArea(5,10)]public string infoAboutEvent;
    [TextArea(5,10)] public string infoAboutStart;
    bool onSpaceTrigger;

    public float t_ofStart;
    public float t_ofEvent;

    bool canStartEvent;
    EventController eventController;
    EventControlFirst eventControl;
    void Update()
    {
        
        eventController = GetComponent<EventController>();
        
        ShowToStartEvent();

        if (eventControl == null)
        {
            eventControl = GetComponent<EventControlFirst>();
        }
    }

    void ShowToStartEvent()
    {
        if(canStartEvent && Input.GetKey(KeyCode.Return))
        {
            if (t_ofStart != 0)
            {
                SplashInfo.ShowInfo(infoAboutStart, t_ofStart);
            }
            else
            {
                SplashInfo.ShowInfo(infoAboutStart, t_ofStart);
            }
            if (eventController != null)
            {
                eventController.StartEvent();
                eventController.StartFreestyle();
            }
            else
            {
                eventControl.StartEvent();
            }
            canStartEvent = false;
        }
        if(onSpaceTrigger)
        {
            SplashInfo.ShowInfo(infoAboutEvent);
            onSpaceTrigger = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        { 
            canStartEvent = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            onSpaceTrigger = true;
        }
    }
}
