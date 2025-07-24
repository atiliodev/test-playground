using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDataDisplay : MonoBehaviour
{
    public TricksScore scoreSystem;
    public TextMeshProUGUI textTotalScore;
    public TextMeshProUGUI textBestScore;

    public float totalValue;
   
    void Update()
    {
        textBestScore.text = "" + scoreSystem.bestTrickScore.ToString("F2");
        textTotalScore.text = "" + scoreSystem.totalScore.ToString("F2");

        totalValue = scoreSystem.totalScore;
    }
}
