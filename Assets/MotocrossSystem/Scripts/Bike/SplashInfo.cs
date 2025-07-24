using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SplashInfo : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public float timeOfDisplay = 7f;

    public float currentTime;

    public static string textInfo;

    static bool showInfo;
    static float timeToMake;
    Image bk;

    void Start()
    {
        bk = GetComponent<Image>();
        bk.enabled = false;
        textUI.text = " ";
    }

    void Update()
    {
        if (showInfo)
        {
            if(timeToMake > 0)
            {
                currentTime = timeToMake;
            }
            else
            {
                currentTime = timeOfDisplay;
            }
            StartCoroutine("displayInfo");
            showInfo = false;
        }
    }


    IEnumerator displayInfo()
    {
        yield return new WaitForSeconds(0.2f);
        bk.enabled = true;
        textUI.text = textInfo;
        yield return new WaitForSeconds(currentTime);
        bk.enabled = false;
        textUI.text = "";

    }
    public static void ShowInfo(string info, float timeTo = 0)
    {
        if(timeTo != null)
        {
            timeToMake = (float) timeTo;
        }

        textInfo = info;
        showInfo = true;
    }

    public void EndDisplay()
    {
        StopAllCoroutines();
        bk.enabled = false;
        textUI.text = "";
    }
}
