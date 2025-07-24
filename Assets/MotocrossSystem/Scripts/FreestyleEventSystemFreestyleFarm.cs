using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class FreestyleEventSystemFreestyleFarm : MonoBehaviour
{
    public TricksScore tricksScoreScript;
    public GameObject startPanel;
    public GameObject leaderboardPanel;

    // Textos nos painéis
    public TextMeshProUGUI startPanelPlayerNameText; // Exibe a lista de competidores no Start Panel
    public TextMeshProUGUI competitorNamesText; // Nomes no Leaderboard
    public TextMeshProUGUI scoresText; // Pontuações no Leaderboard

    public ProfileSystem profileSystem; // Referência ao sistema de perfil
    public string[] opponentNames; // Nomes dos oponentes já definidos

    // Bônus fixo para os oponentes
    public float[] opponentScoreBonuses;

    private bool eventStarted = false;
    private bool eventEnded = false;
    private string playerName;

    void Start()
    {
        InitializeEvent();
    }

    void Update()
    {
        if (!eventStarted)
        {
            UpdatePlayerName();

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(StartEventCountdown());
            }
        }
        else if (tricksScoreScript.CountdownTimer <= 0 || 
                 (tricksScoreScript.playerHealth != null && tricksScoreScript.playerHealth.currentHealth <= 0))
        {
            if (!eventEnded)
            {
                if (tricksScoreScript.playerHealth != null && tricksScoreScript.playerHealth.currentHealth <= 0)
                {
                    tricksScoreScript.scoreAmount *= 0.5f; // Penalidade ao morrer
                }
                StartCoroutine(ShowLeaderboard());
                eventEnded = true;
            }
        }

        // Esconde o Leaderboard ao pressionar H
        if (leaderboardPanel.activeSelf && Input.GetKeyDown(KeyCode.H))
        {
            leaderboardPanel.SetActive(false);
            eventStarted = false;
            eventEnded = false;
            InitializeEvent();
        }
    }

    void InitializeEvent()
    {
        startPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        UpdatePlayerName();
    }

    void UpdatePlayerName()
    {
        if (profileSystem != null)
        {
            playerName = profileSystem.name;

            string startPanelNames = "";
            for (int i = 0; i < opponentNames.Length; i++)
            {
                startPanelNames += $"{i + 1}. {opponentNames[i]}\n";
            }

            startPanelNames += $"{opponentNames.Length + 1}. {playerName}";
            startPanelPlayerNameText.text = startPanelNames;
        }
    }

    IEnumerator StartEventCountdown()
    {
        startPanel.SetActive(true);
        startPanelPlayerNameText.text += "\n\nIniciando em:";
        
        for (int i = 5; i > 0; i--)
        {
            startPanelPlayerNameText.text = $"Iniciando em: {i}";
            yield return new WaitForSeconds(1f);
        }

        startPanel.SetActive(false);
        eventStarted = true;
        tricksScoreScript.ResetScoreAndTime();
    }

    IEnumerator ShowLeaderboard()
    {
        float playerScore = tricksScoreScript.scoreAmount;
        leaderboardPanel.SetActive(true);

        var competitors = new List<string> { playerName };
        competitors.AddRange(opponentNames);

        var scores = new Dictionary<string, float>();

        for (int i = 0; i < competitors.Count; i++)
        {
            string competitor = competitors[i];
            float bonus = (i == 0) ? 0f : opponentScoreBonuses[i - 1];
            float score = Random.Range(72f, 75f) + bonus;
            scores[competitor] = Mathf.Clamp(score, 76f, 87f);
        }

        scores[playerName] = playerScore;

        var sortedScores = scores.OrderByDescending(s => s.Value).ToList();

        string namesContent = "";
        string scoresContent = "";
        for (int i = 0; i < sortedScores.Count; i++)
        {
            string name = sortedScores[i].Key;
            string score = sortedScores[i].Value.ToString("F2");

            namesContent += $"{i + 1}. {name}\n";
            scoresContent += $"{score}\n";
        }

        competitorNamesText.text = namesContent;
        scoresText.text = scoresContent;

        yield return null;
    }
}