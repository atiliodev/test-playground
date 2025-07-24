using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TricksScore tricksScore;

    public TMP_Text currentTrickText;
    public TMP_Text currentScoreText;
    public TMP_Text bestTrickText;
    public TMP_Text bestScoreText;
    public TMP_Text totalScoreText;
    public TMP_Text playerNameText;
    public TMP_Text countdownText; // Referência para a contagem regressiva

    private bool showTotalScore = false;

    void Update()
    {
        if (tricksScore != null)
        {
            currentTrickText.text = tricksScore.currentFreestyleName;
            currentScoreText.text = tricksScore.currentScore.ToString("F2");
            bestTrickText.text = tricksScore.bestTrickName;
            bestScoreText.text = tricksScore.bestTrickScore.ToString("F2");

            // Atualizar o texto da contagem regressiva
            if (countdownText != null)
            {
               //countdownText.text = tricksScore.countdownText.text;
            }

            // Exibir ou esconder a pontuação total com base na contagem regressiva
            if (showTotalScore)
            {
                totalScoreText.text = tricksScore.scoreAmount.ToString("F2");
            }
            else
            {
                totalScoreText.text = "";
            }
        }
    }

    public void ShowTotalScore()
    {
        showTotalScore = true;
    }

    public void HideTotalScore()
    {
        showTotalScore = false;
    }
}