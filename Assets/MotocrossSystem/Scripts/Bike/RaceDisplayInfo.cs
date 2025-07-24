using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceDisplayInfo : MonoBehaviour
{
    public string[] namesOfRides = new string[10]; 
    public float[] score = new float[10];          
    public TextMeshProUGUI[] nameText;            
    public TextMeshProUGUI[] scoreText;          

    public float playerValue;                     
    public string playerName = "None";          

    public string[] orderRiders;                
    public float[] orderScore;

    public ScoreDataDisplay dataDisplay;

    public bool thePlayerIsTheFirst;

    public ProfileSystem profile;

    void Start()
    {
        profile = GameObject.FindObjectOfType<ProfileSystem>();
        GenerateRandomScores(); 
    }

    void Update()
    {
        SorterSystem(); 
        ShowText();

       
        playerName = profile.name;

        if (orderRiders[0] != null)
        {
            thePlayerIsTheFirst = orderRiders[0] == playerName;
        }
    }

    void GenerateRandomScores()
    {
        for (int i = 0; i < score.Length; i++)
        {
            score[i] = Mathf.Round(Random.Range(500f, 800f) * 100f) / 100f;
        }
    }

    void SorterSystem()
    {
        List<string> allNames = new List<string>(namesOfRides);
        List<float> allScores = new List<float>(score);

        allNames.Add(playerName);
        allScores.Add(playerValue);

        playerValue = dataDisplay.totalValue;

        for (int i = 0; i < allScores.Count - 1; i++)
        {
            for (int j = i + 1; j < allScores.Count; j++)
            {
                if (allScores[j] + Random.Range(0f, 0.01f) > allScores[i])
                {
                    float tempScore = allScores[i];
                    allScores[i] = allScores[j];
                    allScores[j] = tempScore;

                    string tempName = allNames[i];
                    allNames[i] = allNames[j];
                    allNames[j] = tempName;
                }
            }
        }

        orderRiders = allNames.ToArray();
        orderScore = allScores.ToArray();
    }

    void ShowText()
    {
        for (int i = 0; i < nameText.Length; i++)
        {
            if (i < orderRiders.Length)
            {
                nameText[i].text = orderRiders[i];
                scoreText[i].text = orderScore[i].ToString("F2");
            }
            else
            {
                nameText[i].text = "";
                scoreText[i].text = "";
            }
        }
    }
}