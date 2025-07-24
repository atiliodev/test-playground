using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowFlip : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public TextMeshProUGUI textUIXp;
    public float timeOfDisplay = 7f;

    public float currentTime;

    public static string textInfo;
    public static float textXp;

    static bool showInfo;
    static float timeToMake;
    Image bk;
    public Image bk1;

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
            showInfo = false;
        }
    }


    IEnumerator displayInfo()
    {
        yield return new WaitForSeconds(0.2f);
        bk.enabled = true;
        bk1.enabled = true;
        textUI.text = textInfo;
        textUIXp.text = textXp.ToString("f0") + "X" + " +";
        yield return new WaitForSeconds(currentTime);
        bk.enabled = false;
        bk1.enabled = false;
        textUI.text = "";
        textUIXp.text = "";

    }
    public static void ShowInfoXp(string info, float xp, float timeTo = 0)
    {
        if (timeTo != null)
        {
            timeToMake = (float)timeTo;
        }

        textInfo = info;
        textXp = xp;
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