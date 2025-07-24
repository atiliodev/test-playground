using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockDisplay : MonoBehaviour
{
   
    public TextMeshProUGUI clockText;
    public TricksScore systemT;
    
    public float totalSeconds = 0f;

    public bool timeEnd;

    bool haveTime;
    bool done;

    public static bool restart;
    private void Start()
    {
        if (!done)
        {
            StartCoroutine(startTime());
            done = true;
        }
    }

    IEnumerator startTime()
    {
        yield return new WaitForSeconds(0.1f);
        totalSeconds = systemT.initialTime * 3;
    }

    void Update()
    {
       

        if(systemT.haveResetTheGame && !done)
        {
            StartCoroutine(startTime());
            systemT.haveResetTheGame = false;
            done = true;
        }

        if (totalSeconds >= 0.1f)
        {
            haveTime = true;
            totalSeconds -= Time.deltaTime * 25;
            done = false;
        }
        else
        {
            totalSeconds = 0;
            if (haveTime)
            {
                timeEnd = true;
                haveTime = false;
            }
        }
        
        int hours = Mathf.FloorToInt(totalSeconds / 3600);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);

      
        string timeFormatted = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

       
        clockText.text = timeFormatted;
    }

    public void RestartTime()
    {
        StartCoroutine(startTime());
        timeEnd = false;
    }
}
