using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class MotocrossAudioController : MonoBehaviour
{
    // Áudios dos diferentes estágios do motor
    public AudioClip idleClip;
    public AudioClip lowRPMClip;
    public AudioClip midRPMClip;
    public AudioClip highRPMClip;
    public AudioClip limiterClip;

    // Curvas de volume para cada estágio
    public AnimationCurve idleVolumeCurve;
    public AnimationCurve lowVolumeCurve;
    public AnimationCurve midVolumeCurve;
    public AnimationCurve highVolumeCurve;
    public AnimationCurve stateVolumeCurve;
    public AnimationCurve limiterVolumeCurve;

    // Curvas de pitch para cada estágio
    public AnimationCurve idlePitchCurve;
    public AnimationCurve lowPitchCurve;
    public AnimationCurve midPitchCurve;
    public AnimationCurve highPitchCurve;
    public AnimationCurve limiterPitchCurve;

    // RPM atual e RPM máximo do motor
    private float currentRPM = 0f;
    private float maxRPM = 10000f;

    // Configurações do som 3D
    public float dopplerLevel = 1f;
    public float minDistance = 2f;
    public float maxDistance = 150f;

    public AudioMixerGroup mixer;

    // Agressividade separada para cada estado (privado agora)
    private float neutralAccelerationAggressiveness = 450f;
    private float airAccelerationAggressiveness = 3.7f;

    // Fontes de áudio para cada faixa de som
    private AudioSource idleSource;
    private AudioSource lowSource;
    private AudioSource midSource;
    private AudioSource highSource;
    private AudioSource limiterSource;

    // Controle de interpolação do som do motor
    private float displayedRPM = 0f;
    private float previousRPM = 0f;
    private float rpmProgressive = 0f;

    // Agressividade da aceleração e desaceleração
    private float accelerationSpeed = 1f;
    private float decelerationSpeed = 0.25f;

    // Tecla usada para acelerar
    private KeyCode accelerateKey = KeyCode.Space;

    // Controle de embreagem
    private bool clutch = false;

    // Referências aos outros scripts do veículo
    private BikeController bike;
    private FreestyleSystem fbike;
    private GearSystem gearSystem;

    void Start()
    {
        // Pega os componentes necessários
        gearSystem = GetComponent<GearSystem>();
        bike = GetComponent<BikeController>();
        fbike = GetComponent<FreestyleSystem>();

        // Cria as fontes de áudio para cada som
        idleSource = CreateAudioSource(idleClip);
        lowSource = CreateAudioSource(lowRPMClip);
        midSource = CreateAudioSource(midRPMClip);
        highSource = CreateAudioSource(highRPMClip);
        limiterSource = CreateAudioSource(limiterClip);


        highSource.outputAudioMixerGroup = mixer;
        lowSource.outputAudioMixerGroup = mixer;
        midSource.outputAudioMixerGroup = mixer;
        idleSource.outputAudioMixerGroup = mixer;
        limiterSource.outputAudioMixerGroup = mixer;
    }

    void Update()
    {
        // Se estiver no neutro ou no ar, usa um RPM simulado para o som
        if (gearSystem.OnN || !fbike.isGround)
        {
            currentRPM = gearSystem.OnNRPM * 8500f;
        }
        else
        {
            // Caso contrário, usa o RPM real do motor
            currentRPM = bike.EngineRPM;
            maxRPM = bike.MaxEngineRPM;
            previousRPM = currentRPM;
        }

        // Atualiza os sons com base no RPM
        UpdateEngineSound();

        // Verifica se está trocando de marcha (embreagem ativa)
        clutch = bike.onShift;
    }

    void UpdateEngineSound()
{
    // Calcula o RPM relativo (entre 0 e 1)
    float target = Mathf.Clamp01(currentRPM / maxRPM);

    bool inNeutral = gearSystem.OnN;
    bool inAir = !fbike.isGround;
    bool noFriction = inNeutral || inAir;

    float accel = accelerationSpeed;    // Velocidade de aumento de RPM
    float decel = decelerationSpeed;    // Velocidade de queda de RPM
    float rpmBoost = 0f;

    // Ajustes específicos para o neutro
    if (inNeutral)
    {
        accel = neutralAccelerationAggressiveness * 5.5f; // ← Aumente para acelerar mais rápido no neutro
        decel = decelerationSpeed * 3f;                   // ← Aumente para desacelerar mais rápido no neutro
        if (Input.GetKey(accelerateKey)) rpmBoost = 0.45f; // ← Controla o "impulso" de RPM quando acelera no neutro
    }
    else if (inAir || Input.GetKey(KeyCode.Alpha2)) // Em voo ou com tecla específica
    {
        accel = airAccelerationAggressiveness; // ← Aumente para RPM subir mais rápido no ar
        decel = decelerationSpeed * 3f;        // ← Aumente para RPM cair mais rápido no ar
        if (Input.GetKey(accelerateKey)) rpmBoost = 0.10f;
    }

    float boostedTarget = Mathf.Clamp01(target + rpmBoost);

    // Se estiver acelerando (exceto com embreagem apertada)
    if (((Input.GetKey(accelerateKey) || Input.GetKey(KeyCode.Alpha2)) && !clutch) || ((Input.GetKey(accelerateKey) || Input.GetKey(KeyCode.Alpha2) && gearSystem.CurrentGear >= gearSystem.GearRatio.Length - 1)))
    {
        rpmProgressive = Mathf.MoveTowards(rpmProgressive, boostedTarget, accel * Time.deltaTime);
    }
    else
    {
        // RPM cai mais para zero se estiver parado ou sem atrito (no ar ou neutro)
        if (bike.m_body.linearVelocity.magnitude <= 0.01f || noFriction)
            rpmProgressive = Mathf.MoveTowards(rpmProgressive, 0f, decel * Time.deltaTime);
        else
            rpmProgressive = Mathf.MoveTowards(rpmProgressive, 0.2f, decel * Time.deltaTime); // ← RPM mínimo ao soltar o acelerador no chão
    }

    // Volume de cada camada sonora de acordo com RPM
    idleSource.volume = idleVolumeCurve.Evaluate(rpmProgressive);
    lowSource.volume = lowVolumeCurve.Evaluate(rpmProgressive);
    midSource.volume = gearSystem.CurrentGear >= gearSystem.GearRatio.Length - 1
        ? stateVolumeCurve.Evaluate(rpmProgressive)
        : midVolumeCurve.Evaluate(rpmProgressive);
    highSource.volume = Mathf.Lerp(highSource.volume, highVolumeCurve.Evaluate(rpmProgressive), 35f * Time.deltaTime); // ← Aumente o número para resposta mais rápida no HIGH

    // Pitch de cada camada
    idleSource.pitch = idlePitchCurve.Evaluate(rpmProgressive);
    lowSource.pitch = lowPitchCurve.Evaluate(rpmProgressive);
    midSource.pitch = midPitchCurve.Evaluate(rpmProgressive);

    // Afeta pitch do HIGH de forma diferente para cada marcha
    int currentGear = gearSystem.CurrentGear;
    float gearFactor = Mathf.InverseLerp(1, gearSystem.GearRatio.Length - 1, currentGear); // ← 0 na 1ª, 1 na última marcha

    float highPitchBase = Mathf.Lerp(2.2f, 1.8f, gearFactor);    // ← Pitch base mais alto nas marchas baixas
    float highPitchRange = Mathf.Lerp(0.97f, 0.4f, gearFactor);  // ← Maior variação nas marchas baixas

    float highTargetPitch = highPitchBase + (rpmProgressive * highPitchRange);
    highSource.pitch = Mathf.Lerp(highSource.pitch, highTargetPitch, 20f * Time.deltaTime); // ← Resposta de pitch (aumente p/ mais rápido)

    // Se está no ar ou neutro e acelerando, reduz o volume das camadas base
    if (noFriction && Input.GetKey(accelerateKey))
    {
        float fadeSpeed = 35f; // ← Velocidade para esconder as camadas quando está no ar
        idleSource.volume = Mathf.Lerp(idleSource.volume, 0f, fadeSpeed * Time.deltaTime);
        lowSource.volume = Mathf.Lerp(lowSource.volume, 0f, fadeSpeed * Time.deltaTime);
        midSource.volume = Mathf.Lerp(midSource.volume, 0f, fadeSpeed * Time.deltaTime);
    }

    // Se chegou perto do RPM máximo e está no ar/neutro, ativa o limiter
    if (rpmProgressive > 0.9f && noFriction)
    {
        float fadeSpeed = 25f;
        float limiterPitchStart = 0.8f;
        float limiterPitchEnd = 1.05f;

        float blend = Mathf.InverseLerp(0.88f, 1f, rpmProgressive); // ← Onde o limiter começa a se misturar (ajuste aqui!)

        limiterSource.volume = Mathf.Lerp(limiterSource.volume, 1f, fadeSpeed * Time.deltaTime);
        limiterSource.pitch = Mathf.Lerp(limiterPitchStart, limiterPitchEnd, blend);

        float highSuppression = Mathf.InverseLerp(0.99f, 1f, rpmProgressive); // ← Quanto abafa o HIGH no final do limiter
        float highTargetVolume = Mathf.Lerp(highVolumeCurve.Evaluate(rpmProgressive), 0f, highSuppression);
        highSource.volume = Mathf.Lerp(highSource.volume, highTargetVolume, fadeSpeed * Time.deltaTime);

        // Desliga completamente as outras fontes
        idleSource.volume = 0f;
        lowSource.volume = 0f;
        midSource.volume = 0f;
    }
    else
    {
        // Quando limiter não está ativo, garante que as fontes estão normais
        float fadeSpeed = 25f;
        limiterSource.volume = Mathf.Lerp(limiterSource.volume, 0f, fadeSpeed * Time.deltaTime);

        idleSource.volume = idleVolumeCurve.Evaluate(rpmProgressive);
        lowSource.volume = lowVolumeCurve.Evaluate(rpmProgressive);
        midSource.volume = gearSystem.CurrentGear >= gearSystem.GearRatio.Length - 1
            ? stateVolumeCurve.Evaluate(rpmProgressive)
            : midVolumeCurve.Evaluate(rpmProgressive);

        float highTarget = highVolumeCurve.Evaluate(rpmProgressive);
        highSource.volume = Mathf.Lerp(highSource.volume, highTarget, 25f * Time.deltaTime);

        // Pitch do HIGH fora do limiter
        float pitchFadeSpeed = 50f;
        highSource.pitch = Mathf.Lerp(highSource.pitch, Mathf.Lerp(1.2f, 2.1f, rpmProgressive), pitchFadeSpeed * Time.deltaTime);
    }
}

    // Cria um AudioSource configurado para um determinado AudioClip
    AudioSource CreateAudioSource(AudioClip clip)
    {
        GameObject audioObj = new GameObject("AudioSource_" + clip.name);
        audioObj.transform.parent = this.transform;
        audioObj.transform.localPosition = Vector3.zero;

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.dopplerLevel = dopplerLevel;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.Play();
        return source;
    }
}