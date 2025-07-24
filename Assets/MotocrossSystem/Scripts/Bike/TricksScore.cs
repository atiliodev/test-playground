using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TricksScore : MonoBehaviour
{
    public enum typeOfRace
    {
        missionFoamPit,
        missionFreestyle,
        missionRacer
    };
    [Header("Select The Type Of Competition")]
    public typeOfRace typeOfCompetition;
    [Space(12)]

    public BikesControlerSystem bikesControler;
    public FreestyleSystem freeStyle;
    public PlayerHealth playerHealth;

    public float[] maxTrickScores = { 100, 200, 300, 500, 600 }; // Adicione as pontuações máximas
    public string[] trickNames = { "Trick1", "Trick2", "Backflip", "Frontflip", "AnotherTrick" }; // Adicione os flips

    public float currentScore;
    public float totalScore;
    public string currentFreestyleName;
    public string bestTrickName;
    public float bestTrickScore;
    public float scoreAmount;
    public string bestWhipName;
    public float bestWhipScore;

    private bool nextValidation;
    private bool wasOnFStyle;

    private int animationCount;
    private int[] animationExecutionCount;

    private float idleTimer;

    public float countdownTimer = 75f;
    public bool isCountingDown = true;
    public TextMeshProUGUI freestylePanelText;

    public EventController eventController;
    public EventControlFirst eventControl;

    [TextArea(4, 10)] public string missionFoamPitInfo;
    [TextArea(4, 10)] public string missionFreestyleInfo;
    [TextArea(4, 10)] public string missionRacerInfo;

    private float lastTrickTime;
    private int sequenceCount;
    private float currentBonus;
    private bool isFirstTrick = true;
    private string currentTrickName = "";
    private float currentTrickScore = 0f;
    private string trickName = "";
    private string lastTrickName = "";
    
    bool isFoamPit;
    bool isFreestyle;
    bool isRacer;

    public bool cantGiveScore;
    public bool await;
    // Adicione no topo da classe:
    public string lastFlipName;
    public float lastFlipScore;

    public float initialTime;
    
    public TextMeshProUGUI currentTrickNameText;
    public TextMeshProUGUI currentTrickScoreText;
    public TextMeshProUGUI bestTrickNameText;
    public TextMeshProUGUI bestTrickScoreText;
    public TextMeshProUGUI bestWhipNameText;
    public TextMeshProUGUI bestWhipScoreText;
    public TextMeshProUGUI totalScoreText;
    
    // ⏱️ Timer para rodada de freestyle
    private float timeLeft = 75f;        // Tempo padrão de 1min15s
    private bool timerActive = false;    // Ativado no início da rodada
    public bool haveResetTheGame;

    public float flipPointWas;

    [HideInInspector]public float timeEvent;

    private void Awake()
    {
        bikesControler = GameObject.FindObjectOfType<BikesControlerSystem>();
         
        if (countdownTimer > 0)
        {
            timeEvent = countdownTimer;
            initialTime = countdownTimer;
            StartCoroutine(setTime(timeEvent));
        }
        else
        {
            StartCoroutine(setTime(PlayerPrefs.GetFloat("TimeEvent")));
        }
    }

    public float CountdownTimer // Propriedade pública para acessar countdownTimer
    {
        get { return countdownTimer; }
    }

   public void ResetScoreAndTime()
{
    currentScore = 0;
    totalScore = 0;
    animationCount = 0;

    bestTrickScore = 0;
    bestTrickName = "";

    bestWhipScore = 0;         // ⬅️ RESET do Best Whip
    bestWhipName = "";         // (opcional, se ainda usar)

    timeLeft = initialTime;
    timerActive = true;

    flipPointWas = 0;
    lastFlipName = "";
    currentFreestyleName = "";
    currentTrickName = "";
    currentTrickScore = 0;
    trickName = "";
    lastTrickName = "";

    UpdateCanvas(); // Atualiza HUD com os resets
}

    float beforeTime;
    void Start()
    {
        if (!haveSet)
        {
           // StartCoroutine(setTime(countdownTimer));
        }
        playerHealth = bikesControler.healthSystem;
        freeStyle = bikesControler.bikeSystem;
        animationExecutionCount = new int[freeStyle.scoreOfFStyle.Length];
        lastTrickTime = -10f;
        sequenceCount = 0;
        currentBonus = 0;
        isFirstTrick = true;
        // 🧼 Limpa tudo no início do jogo
        ResetScoreAndTime();
       
        switch (typeOfCompetition)
        {
            case typeOfRace.missionFoamPit:
                isFoamPit = true;
                break;
            case typeOfRace.missionFreestyle:
                isFreestyle = true;
                break;
            case typeOfRace.missionRacer:
                isRacer = true;
                break;
        }

        freeStyle.onEvent = true;

    }

    IEnumerator setTime(float time)
    {
        haveSet = true;
        Debug.Log("Have set the time in: " + time);
        yield return new WaitForSeconds(1);
        PlayerPrefs.SetFloat("TimeEvent", time);
    }

    bool nextTime;
    int countOp1 = 0;
    int countOp2 = 0;

    public bool call1;
    public bool call2;

    public bool haveGet;
    public bool haveSet;

    public float FlipValue;
    public string FlipName;
    public float TrickValue;
    public string TrickName;
    void Update()
    {
        if (canLookAfter)
        {
            if (playerHealth.onDamage || bikesControler.bikeController.crashed)
            {
                dontGiviScore = true;
            }
        }
        if (resultOn1 >= 1 && resultOn2 >= 1)
        {
            finishTheTask = true;
            resultOn1 = 0;
            resultOn2 = 0;
        }

        if (finishTheTask)
        {
            freeStyle.fatorShow1 = 0;
            finishTheTask = false;
        }

        if (!haveGet)
        {
            if (PlayerPrefs.HasKey("TimeEvent"))
            {
                countdownTimer = PlayerPrefs.GetFloat("TimeEvent") * 3;
                Debug.Log("Get With: " + PlayerPrefs.GetFloat("TimeEvent"));
                initialTime = PlayerPrefs.GetFloat("TimeEvent");
                haveGet = true;
            }
        }
        else
        {
            if (countdownTimer <= 0)
            {
                haveGet = true;
            }
        }

        if (!freeStyle.onEvent)
        {
            freeStyle.onEvent = true;
        }

        if (freeStyle.onflip && !call1)
        {
            cancelInOther = false;
            StartCoroutine(validateFlip());
            call1 = true;
        }

        if (freeStyle.onWhip && !call2)
        {
            StartCoroutine(validateWhip());
            call2 = true;
        }

        if (freeStyle.onFreeStyle && currentScore > 0 && !nextTime)
        {
            ShowXpOnLive.ShowInfoXp(currentFreestyleName, currentScore);
            StartCoroutine(WaitAnotherTime());
            nextTime = true;
        }

        if (playerHealth == null || freeStyle == null)
        {
            playerHealth = bikesControler.healthSystem;
            freeStyle = bikesControler.bikeSystem;
        }

        if (playerHealth != null && (playerHealth.currentHealth <= 0 || bikesControler.bikeController.crashed))
        {
            //ResetScoreAndTime();

            if (!await)
            {
                StartCoroutine(EnableLock());
                await = true;
            }
            return;
        }

        if (isCountingDown)
        {
            countdownTimer -= Time.deltaTime;
            UpdateCountdownDisplay();

            if (countdownTimer <= 0)
            {
                StartCoroutine(EndRun());
                isCountingDown = false;
            }
        }

        if (freeStyle != null && freeStyle.outputIndexValue - 1 >= 0)
        {
            CountScore(freeStyle.onFreeStyle, freeStyle.outputIndexValue - 1);
            NameOfFreestyle(freeStyle.outputIndexValue - 1);
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        if (idleTimer >= 30f)
        {
            idleTimer = 0f;
            if (totalScore > 0)
            {
                //  totalScore -= totalScore * 0.10f;
                scoreAmount = animationCount > 0 ? totalScore / animationCount : 0;
                Debug.Log("Jogador inativo! Novo totalScore: " + totalScore);
            }
        }

        if (freeStyle.onFreeStyle)
        {
            wasOnFStyle = true;
        }
        else
        {
            if (wasOnFStyle)
            {
                StartCoroutine(validateFall());
                StartCoroutine(ValidateProcess());
                wasOnFStyle = false;
            }
        }
        if (isFoamPit)
        {
            if (!freeStyle.isGround && !waitTheProcess)
            {
                StartCoroutine("freestyleT");
                waitTheProcess = true;
            }
        }


    }
    bool doneTo;

    public bool dontGiviScore;
    bool canLookAfter;
    IEnumerator validateFall()
    {
        bool beforeDemage = false;
        dontGiviScore = false;
        yield return new WaitForSeconds(0);
        canLookAfter = true;
        yield return new WaitForSeconds(7);
        canLookAfter = false;
        Debug.Log("Finish Look!");
    }

    IEnumerator validateFinal()
    {
        yield return new WaitForSeconds(4);
        //SplashInfoWithXp.ShowInfoXp("TotalScore:", fator1 + fator2);


    }
    float valueWhip;
    IEnumerator validateWhip()
    {
        bool beforeDemage = false;
        if (playerHealth.onDamage || bikesControler.bikeController.crashed)
        {
            beforeDemage = true;
            setK = true;
        }
        else
        {
            if (!setK)
            {
                beforeDemage = false;
            }
        }
        valueWhip = 0;
        yield return new WaitForSeconds(3.2f);
        if (!bikesControler.bikeController.crashed && !beforeDemage)
        {
            AddPointsToWhip();

        }
        else if (bikesControler.bikeController.crashed || beforeDemage)
        {
            freeStyle.whipCount = 0;
            freeStyle.pointCount = 0;
            countOp2 = 0;
            freeStyle.onWhip = false;

            SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
        }
        yield return new WaitForSeconds(1f);
        call2 = false;
    }

   private void AddPointsToWhip()
{
    SFXController.doneSomethingSFX();
    valueWhip = freeStyle.whipCount;
   
    if (!isCountingDown)
    return;
   
    if (valueWhip > 0)
    {
        totalScore += valueWhip;

        // Atualiza Best Trick (qualquer manobra)
        if (valueWhip > bestTrickScore)
        {
            bestTrickScore = valueWhip;
            bestTrickName = "Whip";
        }

        // Atualiza Best Whip (somente whip)
        if (valueWhip > bestWhipScore)
        {
            bestWhipScore = valueWhip;
            bestWhipName = "Whip";
        }

        // Limpa o estado do whip
        freeStyle.whipCount = 0;
        freeStyle.pointCount = 0;
        countOp2 = 0;
        freeStyle.onWhip = false;

        // ATUALIZA A TELA AQUI
        UpdateCanvas();

        Debug.Log($"✅ Novo Best Whip: {bestWhipScore} pontos");
    }
}

    float valueToApply;

    public bool cancelInOther;

    IEnumerator validateFlip()
    {
        bool beforeDemage = false;
        haveGiveScore2 = false;
        if (playerHealth.onDamage || bikesControler.bikeController.crashed)
        {
            cancelInOther = true;
            beforeDemage = true;
            setK = true;
        }
        else
        {
            if (!setK)
            {
                beforeDemage = false;
            }
        }
        waitNext = false;
        yield return new WaitForSeconds(6.2f);
        if (!bikesControler.bikeController.crashed && !beforeDemage)
        {
            AddPointOnFlip();
        }
        else if (bikesControler.bikeController.crashed)
        {
            freeStyle.flipCount = 0;
            freeStyle.accumulatedRotation = 0;
            countOp1 = 0;
            freeStyle.onflip = false;

            SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
        }
        yield return new WaitForSeconds(0);
        call1 = false;
    }

    bool waitNext;

   private void AddPointOnFlip()
{
    bool isBackflip = freeStyle.accumulatedRotation < 0;
    bool isFrontflip = freeStyle.accumulatedRotation > 0;

    if (!isCountingDown)
    return;

    if ((isBackflip || isFrontflip) && !waitNext && !haveGiveScore1 && !haveGiveScore2)
    {
        int flipCount = Mathf.Max(1, freeStyle.flipCount);
        float comboScore = Mathf.Clamp(currentScore, 0f, 150f);
        string flipType = isBackflip ? "Backflip" : "Frontflip";
        string baseName = GetFlipName(flipType, flipCount);

        if (!cancelInOther && !dontGiviScore)
        {
            SFXController.doneSomethingSFX();
            Debug.Log($"🌀 Flip detectado: {flipType} | FlipCount: {flipCount} | ComboScore: {comboScore} | FatorShow1: {freeStyle.fatorShow1}");

            float backBase = 76f;
            float frontBase = 82f;

            float backComboBonus1 = 10f;
            float backComboBonus2 = 25f;
            float backComboBonusN = 17f;

            float frontComboBonus1 = 33f;
            float frontComboBonus2 = 36f;
            float frontComboBonusN = 20f;

            float backNoComboBonus2 = 20f;
            float backNoComboBonus3 = 60f;

            float frontNoComboBonus2 = 24f;
            float frontNoComboBonus3 = 52f;

            if (freeStyle.fatorShow1 >= 1 && comboScore >= 20f)
            {
                if (isBackflip)
                {
                    if (flipCount == 1)
                        flipPointWas = (backBase + comboScore + backComboBonus1) / 2f;
                    else if (flipCount == 2)
                        flipPointWas = (backBase * 2 + comboScore + backComboBonus2) / 2.65f;
                    else
                        flipPointWas = (backBase * flipCount + comboScore + backComboBonusN * flipCount) / (flipCount + 1f);
                }
                else if (isFrontflip)
                {
                    if (flipCount == 1)
                        flipPointWas = (frontBase + comboScore + frontComboBonus1) / 1.8f;
                    else if (flipCount == 2)
                        flipPointWas = (frontBase * 2 + comboScore + frontComboBonus2) / 3f;
                    else
                        flipPointWas = (frontBase * flipCount + comboScore + frontComboBonusN * flipCount) / (flipCount + 1f);
                }

                flipPointWas = Mathf.Min(flipPointWas, 100f);
                lastFlipName = string.IsNullOrEmpty(currentTrickName) ? baseName : baseName + " " + currentTrickName;
            }
            else
            {
                if (isBackflip)
                {
                    if (flipCount == 1)
                        flipPointWas = backBase;
                    else if (flipCount == 2)
                        flipPointWas = (backBase * 2 + backNoComboBonus2) / 2f;
                    else if (flipCount == 3)
                        flipPointWas = (backBase * 3 + backNoComboBonus3) / 3f;
                    else
                        flipPointWas = backBase * flipCount;
                }
                else if (isFrontflip)
                {
                    if (flipCount == 1)
                        flipPointWas = frontBase;
                    else if (flipCount == 2)
                        flipPointWas = (frontBase * 2 + frontNoComboBonus2) / 2f;
                    else if (flipCount == 3)
                        flipPointWas = (frontBase * 3 + frontNoComboBonus3) / 3f;
                    else
                        flipPointWas = frontBase * flipCount;
                }

                flipPointWas = Mathf.Min(flipPointWas, 100f);
                lastFlipName = baseName;
            }

            // 🟢 Registra o flip para combo
            lastFlipScore = flipPointWas;
            scoreAmount += flipPointWas;
            trickName = lastFlipName;

            // ✅ Se não há combo, já mostra e valida agora
            if (currentScore <= 0 && string.IsNullOrEmpty(currentFreestyleName))
            {
                SplashInfoWithXp.ShowInfoXp(lastFlipName, flipPointWas);
                SFXController.doneSomethingSFX();

                totalScore += (int)flipPointWas;
                animationCount++;

                if (flipPointWas > bestTrickScore)
                {
                    bestTrickScore = (int)flipPointWas;
                    bestTrickName = lastFlipName;
                }

                Debug.Log($"✅ Flip ISOLADO VALIDADO: {lastFlipName} | Pts: {flipPointWas} | Total: {totalScore}");
                flipPointWas = 0f;
                lastFlipName = "";
            }
        }

        // 🧹 Reset
        freeStyle.flipCount = 0;
        freeStyle.accumulatedRotation = 0;
        freeStyle.onflip = false;
        countOp1 = 0;
        waitNext = true;
        haveGiveScore2 = true;

        Debug.Log($"✅ Flip salvo: {lastFlipName} | Pontuação: {flipPointWas}");
    }
}


private string GetFlipName(string baseName, int flipCount)
{
    if (flipCount == 2) return "Double " + baseName;
    if (flipCount == 3) return "Triple " + baseName;
    if (flipCount > 3) return flipCount + "x " + baseName;
    return baseName;
}
    IEnumerator WaitAnotherTime()
    {
        yield return new WaitForSeconds(2f);
        nextTime = false;
    }

    bool waitTheProcess;
    void UpdateCountdownDisplay()
    {
        countdownTimer = Mathf.Max(0, countdownTimer);
        int minutes = Mathf.FloorToInt(countdownTimer / 60);
        int seconds = Mathf.FloorToInt(countdownTimer % 60);
    }


    IEnumerator EnableLock()
    {
        yield return new WaitForSeconds(0);
        cantGiveScore = true;
        yield return new WaitForSeconds(5.3f);
        cantGiveScore = false;
        await = false;

    }


    IEnumerator freestyleT()
    {
        yield return new WaitForSeconds(0);
        bool canGive = false;
        if (wasOnFStyle)
        {
            canGive = true;
        }
        yield return new WaitForSeconds(5f);
        if (playerHealth.onDamage || cantGiveScore)
        {
            if (!await)
            {
                StartCoroutine(EnableLock());
                await = true;
            }

            SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
            //ResetScoreAndTime();
            eventController.FinishLose();
            Debug.Log("From Here 1");
        }
        if (setK)
        {
            if (!await)
            {
                StartCoroutine(EnableLock());
                await = true;
            }

            // ResetScoreAndTime();
            eventController.FinishLose();
            SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
            Debug.Log("From Here 2");
        }
        yield return new WaitForSeconds(5f);
    }




    IEnumerator EndRun()
    {
        yield return new WaitForSeconds(0.9f);
        scoreAmount = animationCount > 0 ? totalScore / animationCount : 0;
        FindObjectOfType<ScoreDisplay>().ShowTotalScore();
        Debug.Log("Média final: " + scoreAmount);
        yield return new WaitForSeconds(5f);
        ResetScoreAndTime();
        FindObjectOfType<ScoreDisplay>().HideTotalScore();
    }

    bool Done;
    bool setK;
    IEnumerator ValidateProcess()
    {
        haveGiveScore1 = false;
        if (isFoamPit)
        {
            yield return new WaitForSeconds(0f);
            nextValidation = false;
            bool beforeDemage = false;
            if (playerHealth.onDamage || cantGiveScore || bikesControler.bikeController.crashed)
            {
                beforeDemage = true;
                setK = true;
            }
            else
            {
                if (!setK)
                {
                    beforeDemage = false;
                }
            }
            yield return new WaitForSeconds(7.2f);

            if (!playerHealth.onDamage && !beforeDemage && !cantGiveScore && !bikesControler.bikeController.crashed)
            {

                ValidateScore();
                Done = true;
            }
            else
            {
                if (!await)
                {
                    StartCoroutine(EnableLock());
                    await = true;
                }
                //  totalScore -= currentScore;
                currentScore = 0;

                SplashInfoWithXp.ShowInfoXp("Try Again", 0);
                Debug.Log("From Here 3");
            }

            setK = false;
            yield return new WaitForSeconds(1f);
        }
        else if (isFreestyle)
        {
            yield return new WaitForSeconds(0f);
            bool beforeDemage = false;
            yield return new WaitForSeconds(0f);
            nextValidation = false;
            if (playerHealth.onDamage || cantGiveScore || bikesControler.bikeController.crashed)
            {
                beforeDemage = true;
                setK = true;
            }
            else
            {
                if (!setK)
                {
                    beforeDemage = false;
                }
            }
            if ((playerHealth.onDamage || bikesControler.bikeController.crashed) && !cantGiveScore)
            {
                StartCoroutine(EnableLock());

            }
            yield return new WaitForSeconds(7.2f);

            if (!playerHealth.onDamage && freeStyle.isGround && !cancelInOther && !beforeDemage && !cantGiveScore && !bikesControler.bikeController.crashed)
            {

                ValidateScoreFreestyle();
                Done = true;
            }
            else
            {
                //  totalScore -= currentScore;
                if (!await)
                {
                    StartCoroutine(EnableLock());
                    await = true;
                }

                currentScore = 0;

                SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
                Debug.Log("From Here 3");
            }

            setK = false;
            yield return new WaitForSeconds(1f);
        }
    }

    void CountScore(bool onMakeFStyle, int index)
    {
        if (onMakeFStyle)
        {
            if (index >= 0 && index < maxTrickScores.Length)
            {
                float fStyleValue = freeStyle.scoreOfFStyle[index]; // Valor do FreestyleSystem
                float maxScoreForTrick = maxTrickScores[index]; // Pontuação máxima para o truque
                float targetScore = Mathf.Min(fStyleValue, maxScoreForTrick); // Respeita o limite máximo
                currentScore = targetScore;

                // Debug para verificar se o flip está sendo tratado
                Debug.Log($"Manobra detectada: {trickNames[index]} | Pontuação: {currentScore}");

                animationExecutionCount[index]++;
            }
            else
            {
                Debug.LogWarning($"Índice {index} fora dos limites do array!");
            }
        }
        else
        {

        }
    }

    void NameOfFreestyle(int index)
    {
        if (index >= 0 && index < trickNames.Length)
        {
            currentFreestyleName = trickNames[index];
        }
    }

    bool finishTheTask;

    float resultOn1;
    float resultOn2;
    private void ValidateScore()
{
    if (scoreAmount > 0)
    {
        totalScore += Mathf.RoundToInt(scoreAmount);
        currentTrickName = trickName;
        currentTrickScore = scoreAmount;

        if (scoreAmount > bestTrickScore)
        {
            bestTrickName = trickName;
            bestTrickScore = Mathf.RoundToInt(scoreAmount);
        }
    }
    else if (flipPointWas > 0f)
    {
        totalScore += Mathf.RoundToInt(flipPointWas);
        currentTrickName = lastTrickName;
        currentTrickScore = flipPointWas;

        if (flipPointWas > bestTrickScore)
        {
            bestTrickName = lastTrickName;
            bestTrickScore = Mathf.RoundToInt(flipPointWas);
        }

        flipPointWas = 0f;
    }

    UpdateCanvas();
}

    IEnumerator GiveScore(int value)
    {
        //value = 1 Only Flip
        //value = 2 Only Trick
        //value = 3 Combo
        int made = 0;
        float doub = 0;
        yield return new WaitForSeconds(0);
        if(value == 1 && made == 0)
        {
            SplashInfoWithXp.ShowInfoXp(FlipName, FlipValue);
            totalScore += FlipValue;
            SFXController.doneSomethingSFX();
            made = 1;
        }
        if (value == 2 && made == 0)
        {
            SplashInfoWithXp.ShowInfoXp(TrickName, TrickValue);
            totalScore += TrickValue;
            SFXController.doneSomethingSFX();
            made = 1;
        }
        if (value == 3 && made == 0)
        {
            made = 1;
            SplashInfoWithXp.ShowInfoXp(FlipName +" + "+ TrickName + " Combo", FlipValue + TrickValue);
            totalScore += FlipValue + TrickValue;
            SFXController.doneSomethingSFX();
        }
        yield return new WaitForSeconds(3);
        FlipName = " ";
        TrickValue = 0;
        FlipValue = 0;
        TrickName = " ";

    }

   private void UpdateCanvas()
{
    if (currentTrickNameText != null)
        currentTrickNameText.text = currentTrickName;

    if (currentTrickScoreText != null)
        currentTrickScoreText.text = currentTrickScore.ToString("F0");

    if (bestTrickNameText != null)
        bestTrickNameText.text = bestTrickName;

    if (bestTrickScoreText != null)
        bestTrickScoreText.text = bestTrickScore.ToString("F0");

    
    if (bestWhipScoreText != null)
    bestWhipScoreText.text = bestWhipScore.ToString("F2");

    if (totalScoreText != null)
        totalScoreText.text = totalScore.ToString("F0");
}


    bool haveGiveScore1, haveGiveScore2;
    void ValidateScoreFreestyle()
{
    bool hasFlip = flipPointWas > 0f;
    string flipName = string.IsNullOrEmpty(lastFlipName) ? "Flip" : lastFlipName;
    bool hasAnimation = !string.IsNullOrEmpty(currentFreestyleName);

    if (!isCountingDown)
    return;

    if (!cantGiveScore && !bikesControler.bikeController.crashed)
    {
        if (!hasFlip && !hasAnimation)
        {
            SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
            return;
        }
    }
    else
    {
        SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
        return;
    }

    int finalScore = 0;
    string combinedTrickName = "";

    int trickBase = (int)currentScore;
    int flipBase = (int)flipPointWas;

    // 🎯 Flip + animação (combo)
    if (hasFlip && hasAnimation)
    {
        combinedTrickName = $"{flipName} + {currentFreestyleName}";
        finalScore = (int)((flipBase + trickBase + 15f) / 2f);
    }
    // 🎭 Apenas animação
    else if (hasAnimation)
    {
        combinedTrickName = currentFreestyleName;
        finalScore = trickBase;
    }
    else
    {
        return; // já foi validado antes pelo AddPointOnFlip
    }

    if (finalScore > bestTrickScore)
    {
        bestTrickScore = finalScore;
        bestTrickName = combinedTrickName;
        Debug.Log($"🏆 Novo Best Trick: {bestTrickName} com {bestTrickScore} pontos.");
    }

    SplashInfoWithXp.ShowInfoXp(combinedTrickName, finalScore);
    SFXController.doneSomethingSFX();

    totalScore += finalScore;
    animationCount++;
    currentScore = 0;

    flipPointWas = 0;
    lastFlipName = "";
    currentFreestyleName = "";

    Debug.Log($"✅ Score Atualizado! Total: {totalScore} | Trick: {combinedTrickName} | Pts: {finalScore}");
    UpdateCanvas();
    }
    
    public void FinishWin()
    {
        scoreAmount = animationCount > 0 ? totalScore / animationCount : 0;
        SplashInfoWithXp.ShowInfoXp(missionFoamPitInfo, totalScore);
        eventController.FinishWin();
        ResetScoreAndTime();
        haveGet = false;
        freeStyle.onEvent = false;
        haveResetTheGame = true;
    }

    public void FinishLost()
    {
        haveResetTheGame = true;
        SplashInfoWithXp.ShowInfoXp("Try Again!", 0);
        eventController.FinishLose();
        ResetScoreAndTime();
        haveGet = false;
        freeStyle.onEvent = false;
    }
}