using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    public static string info_t;
    public static bool notify;
    public TextMeshProUGUI textNotify;
    public string info;
    bool wait;

    Animator anim;
    bool showState;

    private void Start() 
    {
        anim = GetComponent<Animator>();
        textNotify.text = " ";
    }

    void Update()
    {
        info = info_t;
        anim.SetBool("Show", showState);
        if(notify && !wait)
        {
            StartCoroutine("ShowInfo");
            showState = false;
            wait = true;
        }
    }

    IEnumerator ShowInfo()
    {
        yield return new WaitForSeconds(0.1f);
        showState = true;
        textNotify.text = info_t;
        yield return new WaitForSeconds(1f);
        notify = false;
        wait = false;

    }

    public static void Notify(string info)
    {
        info_t = info;
        notify = true;
    }
}
