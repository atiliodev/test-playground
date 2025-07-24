using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInstantiate : MonoBehaviour
{
    public GameObject instantReset;
    private GameObject baseReload;


    static bool startAction;
    void Update()
    {
        if (baseReload == null)
        {
            baseReload = GameObject.FindGameObjectWithTag("EventBase");
        }
        if(startAction)
        {
            StartCoroutine(doAction());
            startAction = false;
        }
    }

    bool x = false;
    public static void ResetEvent()
    {
        startAction = true;
    }

    IEnumerator doAction()
    {
        yield return new WaitForSeconds(0);
        if (!x)
        {
            GameObject newParent = Instantiate(instantReset, baseReload.transform.position, baseReload.transform.rotation);
            baseReload.transform.SetParent(newParent.transform);
            x = true;
        }
        yield return new WaitForSeconds(0);
        x = false;
    }
}
