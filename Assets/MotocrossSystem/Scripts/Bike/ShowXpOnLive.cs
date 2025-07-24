using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowXpOnLive : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI textUIXp;
    public float timeOfDisplay = 7f;

    public float currentTime;

    public static string textInfo;
    public float textXp;

    public static float Xp;
    public float liveXp;

    static bool showInfo;
    static float timeToMake;
    Image bk;
    public Image bk1;

    public bool onIncres;
    public bool onAwait;
    void Start()
    {
        bk = GetComponent<Image>();
        bk.enabled = false;
        bk1.enabled = false;
        textUI.text = " ";
        textUIXp.text = " ";
    }

    void Update()
    {
        if (showInfo)
        {
            if (timeToMake > 0)
            {
                currentTime = timeToMake;
            }
            else
            {
                currentTime = timeOfDisplay;
            }
            StartCoroutine("displayInfo");
            liveXp = 0;
            onIncres = true;
            showInfo = false;
        }

        if(currentTime > 0)
        {
            textXp = liveXp;
        }
        
    }

    private void FixedUpdate()
    {
        if(onIncres && !onAwait)
        {
            if (liveXp < Xp)
            {
                liveXp = liveXp + 70f * Time.deltaTime;
            }
            if (liveXp >= Xp - 10)
            {
                liveXp = Xp;
                //onIncres = false;
                //onAwait = true;
            }
            textUIXp.text = textXp.ToString("f0") + " XP";
        }
    }



    IEnumerator displayInfo()
    {
        yield return new WaitForSeconds(0.2f);
        bk.enabled = true;
        bk1.enabled = true;
        textUI.text = textInfo;
        if (onIncres)
        {
            yield return new WaitForSeconds(currentTime);
            bk.enabled = false;
            bk1.enabled = false;
            textUI.text = "";
            onIncres = false;
            onAwait = false;
            textUIXp.text = "";
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            bk.enabled = false;
            bk1.enabled = false;
            textUI.text = "";
            onIncres = false;
            onAwait = false;
            textUIXp.text = "";

        }
        

    }
    public static void ShowInfoXp(string info, float xp, float timeTo = 0)
    {
        if (timeTo != null)
        {
            timeToMake = (float)timeTo;
        }

        textInfo = info;
        Xp = xp;
        showInfo = true;
    }
    public void EndDisplay()
    {
        StopAllCoroutines();
        bk.enabled = false;
        bk1.enabled = false;
        textUI.text = "";
        textUIXp.text = "";
    }
}
