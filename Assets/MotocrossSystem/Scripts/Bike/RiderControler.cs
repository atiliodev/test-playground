using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RiderControler : MonoBehaviour
{
    private Animator anim;
    public Rigidbody bike;
    public BikeController bikeSystem;
    public FreestyleSystem systemFreestyle;
    public GearSystem gearControl;
    public Vector3 initialOffset;
    public Vector3 offsetFixer;

    public TwoBoneIKConstraint leftHandIk;
    public TwoBoneIKConstraint rightHandIk;
    public Rig HipsRig;
    public Rig SpineRig;
    public Rig LeftLegRig;
    public string leftState;
    public Rig RightLegRig;
    public string rightState;
    public Transform proceduralLeanTarget; // arrasta no Inspector
    [HideInInspector] public float riderBodyLeanFactor; // -1 (pra trás) até 1 (pra frente)
    private float targetWeight;

   [Header("Reação visual do corpo ao peso")]
   public Transform bodyRoot; // geralmente é o osso da pélvis ou "root"
   public float bodyLeanAmount = 0.15f; // quanto o corpo inclina (pra frente/trás)
   public float bodyLeanSpeed = 4f;     // velocidade da transição
   private int bodyLeanLayer;
   public int freestyleLayer;
   public int simpleLayer;

   

   // Guarda o ângulo atual do corpo
   private Vector3 currentBodyAngles = Vector3.zero;
   // Velocidade usada pelo SmoothDamp
   private Vector3 bodyAngleVelocity = Vector3.zero;
    
    public int currentLayer;

    [HideInInspector] public bool onLock;


    public Vector2[] handControl;
    
    // Variável persistente para suavizar o lean
    private float leanVelocity = 0f; // Declare isso no topo da classe
    private void OnEnable()
{
    BikeInstantiate.OnBikeInstantiated += AtualizarReferencias;
}

private void OnDisable()
{
    BikeInstantiate.OnBikeInstantiated -= AtualizarReferencias;
}

void AtualizarReferencias(GameObject novaBike)
{
    if (novaBike == null) return;

    bike = novaBike.GetComponent<Rigidbody>();
    bikeSystem = novaBike.GetComponent<BikeController>();
    systemFreestyle = novaBike.GetComponent<FreestyleSystem>();
    gearControl = novaBike.GetComponent<GearSystem>();
}

    
    void Start()
    {
        anim = GetComponent<Animator>();
        initialOffset = transform.localPosition;
        bodyLeanLayer = anim.GetLayerIndex("BodyLeanLayer");
        
    }

    Vector3 currentOffset;
    public float set23;
    void Update()
{
       
    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    if (this == null || transform == null || !gameObject.activeInHierarchy) return;
    currentOffset = initialOffset - offsetFixer;

    freestyleLayer = anim.GetLayerIndex("Freestyles");
    simpleLayer = anim.GetLayerIndex("RiderControl");
    if (bike == null || bikeSystem == null || systemFreestyle == null || gearControl == null)
    return;

    if (Input.GetKey(KeyCode.UpArrow))
    {
    Debug.Log("UP pressionado!");
    }

    if (systemFreestyle.onFreeStyle)
    {
        for (int x = 0; x < handControl.Length; x++)
        {
            if (handControl[x].x == 1 && handControl[x].y == 1 && x + 1 == systemFreestyle.anim.GetInteger("Index"))
            {
                leftHandIk.weight = 0;
                rightHandIk.weight = 0;
            }
            else if (handControl[x].x == 0.5f && handControl[x].y == 0.5f && x + 1 == systemFreestyle.anim.GetInteger("Index"))
            {
                leftHandIk.weight = 0.58f;
                rightHandIk.weight = 0.58f;
            }
            else if (handControl[x].x == 1 && handControl[x].y == 0 && x + 1 == systemFreestyle.anim.GetInteger("Index"))
            {
                leftHandIk.weight = 0;
                rightHandIk.weight = 1;
            }
            else if (handControl[x].x == 0 && handControl[x].y == 1 && x + 1 == systemFreestyle.anim.GetInteger("Index"))
            {
                leftHandIk.weight = 1;
                rightHandIk.weight = 0;
            }
            else if (handControl[x].x == 0 && handControl[x].y == 0 && x + 1 == systemFreestyle.anim.GetInteger("Index"))
            {
                leftHandIk.weight = 1;
                rightHandIk.weight = 1;
            }
            
        }

        currentLayer = freestyleLayer;
        anim.SetLayerWeight(freestyleLayer, 1);
        anim.SetLayerWeight(simpleLayer, 0);
        anim.SetLayerWeight(bodyLeanLayer, Mathf.Abs(Input.GetAxis("VInput")));
    }
    else
    {
        // 👇 Aqui está a correção essencial:
        leftHandIk.weight = 1;
        rightHandIk.weight = 1;

        currentLayer = simpleLayer;
        anim.SetLayerWeight(freestyleLayer, 0);
        anim.SetLayerWeight(simpleLayer, 1);
    }

        if (onLock)
        {
            transform.localRotation = Quaternion.identity;
        }

        float VInput = Input.GetAxis("VInput");
         // Define a inclinação do corpo com base no VInput (pra frente/tras)
        riderBodyLeanFactor = Mathf.Clamp(VInput, -1f, 1f);
        float HInput = Input.GetAxis("Horizontal");



    float hInput = Input.GetAxis("Horizontal");
float bikeSpeed = bike.linearVelocity.magnitude;  // declarada só aqui

// Define mínima velocidade pra começar a abrir a perna (ex: 3 km/h)
float speedThreshold = 3f;

// Calcula o quanto queremos abrir a perna baseado na curva + velocidade
float curveFactor = Mathf.Abs(hInput) * Mathf.InverseLerp(speedThreshold, 20f, bikeSpeed);

// Se estiver virando pra direita:
if (hInput > 0)
{
    RightLegRig.weight = Mathf.Lerp(RightLegRig.weight, curveFactor, Time.deltaTime * 5f);
    LeftLegRig.weight = Mathf.Lerp(LeftLegRig.weight, 0, Time.deltaTime * 5f);
    if (stateInfo.IsName(rightState))
    {
        anim.speed = Mathf.Lerp(anim.speed, 1 + RightLegRig.weight * 0.5f, Time.deltaTime * 2f);
    }
}
else if (hInput < 0)
{
    LeftLegRig.weight = Mathf.Lerp(LeftLegRig.weight, curveFactor, Time.deltaTime * 5f);
    RightLegRig.weight = Mathf.Lerp(RightLegRig.weight, 0, Time.deltaTime * 5f);
    if (stateInfo.IsName(leftState))
    {
        anim.speed = Mathf.Lerp(anim.speed, 1 + LeftLegRig.weight * 0.5f, Time.deltaTime * 2f);
    }
}
else
{
    // se estiver reto, fecha as pernas suavemente
    LeftLegRig.weight = Mathf.Lerp(LeftLegRig.weight, 0, Time.deltaTime * 5f);
    RightLegRig.weight = Mathf.Lerp(RightLegRig.weight, 0, Time.deltaTime * 5f);
    anim.speed = Mathf.Lerp(anim.speed, 1f, Time.deltaTime * 2f);
}


if (!gearControl.OnN)
{
    anim.SetFloat("VInput", VInput + whenWhilee());
}
else
{
    if (VInput > 0 || bike.linearVelocity.magnitude * 100 > 0)
    {

        anim.SetFloat("VInput", 1);
    }
    else
    {
        anim.SetFloat("VInput", 0);
    }
}

    if (bike.linearVelocity.magnitude > 0.5f )
    {
        anim.SetFloat("VInput", 1);
    }
    else
    {
        anim.SetFloat("VInput", 0);
    }

        anim.SetFloat("HInput", HInput);
anim.SetBool("Stopped", IsStopped());
anim.SetBool("OnAir", !systemFreestyle.isGround);
anim.SetBool("onReverse", bikeSystem.isReverseOn);

if (Input.GetKey("up"))
{
    anim.SetFloat("flipFront", 1);
}
else
{
    anim.SetFloat("flipFront", 0);
}

// REMOVE ISSO AQUI (é o que estava dando erro):
// float bikeSpeed = 0f;

if (bikeSystem != null)
{
    float rearCompression = bikeSystem.coll_rearWheel.suspensionSpring.targetPosition;
    float frontCompression = bikeSystem.coll_frontWheel.suspensionSpring.targetPosition;

    float rawLean = (rearCompression - frontCompression) * 2f;
    float lean = Mathf.Clamp(rawLean, -1f, 1f);

    // 🔁 TRAVAMENTO DE TROCA BRUSCA (passa pelo neutro)
    float currentLeanValue = anim.GetFloat("LeanAmount");
    if (Mathf.Sign(currentLeanValue) != Mathf.Sign(lean) && Mathf.Abs(currentLeanValue) > 0.6f)
    {
        lean = 0f; // força passar pela animação neutra
    }

    // ✅ Suaviza a movimentação do corpo do piloto (Z)
    Vector3 targetLocalPos = initialOffset + new Vector3(0f, 0f, lean * bodyLeanAmount);
    transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, bodyLeanSpeed * Time.deltaTime);

    // ✅ Suaviza o parâmetro do Animator para o Blend Tree (LeanAmount)
    float smoothedLean = Mathf.SmoothDamp(currentLeanValue, lean, ref leanVelocity, 0.15f);
    anim.SetFloat("LeanAmount", smoothedLean);

    // 👇 ANIMAÇÕES BASEADAS NA COMPRESSÃO
    bool isAccelerating = Input.GetKey(KeyCode.Space) && systemFreestyle.isGround;

    // Agora considera freio dianteiro ou seta UP como freio da frente
    bool isBrakingFront = (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.UpArrow)) && systemFreestyle.isGround;

    // 🚫 BLOQUEIA LeanDown se estiver em reverse
   // anim.SetBool("LeanDown", isAccelerating && rearCompression > 0.55f && !bikeSystem.isReverseOn);
   // anim.SetBool("LeanUp", isBrakingFront && frontCompression > 0.45f);

    // ✅ SUSPENSÃO TRASEIRA → ao pressionar DOWN
    if (systemFreestyle.isGround && Input.GetKey(KeyCode.DownArrow))
    {
        JointSpring rearSpring = bikeSystem.coll_rearWheel.suspensionSpring;
        rearSpring.targetPosition = Mathf.Lerp(rearSpring.targetPosition, 0.75f, 5f * Time.deltaTime); // afunda traseira
        bikeSystem.coll_rearWheel.suspensionSpring = rearSpring;
    }
    else
    {
        JointSpring rearSpring = bikeSystem.coll_rearWheel.suspensionSpring;
        rearSpring.targetPosition = Mathf.Lerp(rearSpring.targetPosition, 0.5f, 5f * Time.deltaTime); // volta ao normal
        bikeSystem.coll_rearWheel.suspensionSpring = rearSpring;
    }

    // ✅ SUSPENSÃO DIANTEIRA → afunda suavemente ao frear (RightShift) ou pressionar UP
    if (systemFreestyle.isGround)
    {
        JointSpring frontSpring = bikeSystem.coll_frontWheel.suspensionSpring;

        float frontTarget = 0.5f; // posição normal da suspensão dianteira

        if (isBrakingFront)
        {
            frontTarget = 0.95f; // valor máximo permitido para afundar a suspensão dianteira
        }

        frontSpring.targetPosition = Mathf.Lerp(frontSpring.targetPosition, frontTarget, 5f * Time.deltaTime);
        bikeSystem.coll_frontWheel.suspensionSpring = frontSpring;
    }
    else
    {
        JointSpring frontSpring = bikeSystem.coll_frontWheel.suspensionSpring;
        frontSpring.targetPosition = Mathf.Lerp(frontSpring.targetPosition, 0.5f, 5f * Time.deltaTime);
        bikeSystem.coll_frontWheel.suspensionSpring = frontSpring;
    }

    // Atualiza velocidade
    bikeSpeed = bike.linearVelocity.magnitude;
}
// --- CONTRABALANÇO lateral + inclinação por rampas em UM SÓ target ---
float bikeRoll = bike.transform.localEulerAngles.z;
if (bikeRoll > 180f) bikeRoll -= 360f;
float targetCounterLean = Mathf.Clamp(-bikeRoll * 0.2f, -5f, 5f);

float bikePitch = bike.transform.localEulerAngles.x;
if (bikePitch > 180f) bikePitch -= 360f;
float targetPitchLean = Mathf.Clamp(-bikePitch * 0.2f, -5f, 5f);

// Combina contrabalanço (roll) + inclinação (pitch) num único quaternion
Quaternion targetLeanRot = Quaternion.Euler(targetPitchLean, 0f, targetCounterLean);

// Aplica suavemente em proceduralLeanTarget
//proceduralLeanTarget.localRotation = Quaternion.Slerp(proceduralLeanTarget.localRotation, targetLeanRot, Time.deltaTime * 3f);
        
// --- Input do jogador + micro noise + força G lateral ---
// Input
float h = Input.GetAxis("Horizontal");
float v = Input.GetAxis("VInput");
float targetYaw = 0f;
float targetRoll = h * 5f;
float targetPitch = v * 3f;

// Noise
float noiseX = (Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f) * 1f;
float noiseZ = (Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f) * 1f;

// Força G lateral
Vector3 localVel = bike.transform.InverseTransformDirection(bike.linearVelocity);
float lateralForce = Mathf.Clamp(localVel.x * 0.1f, -2f, 2f);

// Combina tudo em ângulos do corpo
Vector3 targetBodyAngles = new Vector3(
    targetPitch + noiseX, 
    targetYaw, 
    -targetRoll + noiseZ - lateralForce
);

// Suaviza
currentBodyAngles = Vector3.SmoothDamp(
    currentBodyAngles, 
    targetBodyAngles, 
    ref bodyAngleVelocity, 
    0.2f
);

// Aplica no bodyRoot (apenas visual)
bodyRoot.localRotation = Quaternion.Euler(currentBodyAngles);

        float horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput > 0)
        {
            anim.SetBool("Mirrored", true);
        }
        else
        {
            anim.SetBool("Mirrored", false);
        }

        if (Mathf.Abs(horizontalInput) > 0)
        {
            targetWeight -= 1 * Time.deltaTime;
        }
        else
        {
            targetWeight += 1 * Time.deltaTime;
        }


        targetWeight = Mathf.Clamp(targetWeight, 0, 1);


        if (Input.GetKey("down") || Input.GetKey(KeyCode.Alpha2))
        {
            anim.SetBool("RearAprox", true);
        }
        else
        {
            anim.SetBool("RearAprox", false);
        }

        if (ResetSystem.resetPlayer)
        {
            transform.localPosition = currentOffset;
        }

        if (systemFreestyle.isGround && Input.GetKey("up"))
        {
        anim.SetBool("UpForward", true);
        }
        else
        {
        anim.SetBool("UpForward", false);
        }


        if (!systemFreestyle.isGround && Input.GetKey("up") && Input.GetKey(KeyCode.RightShift))
        {
     //   anim.SetBool("FrontFlip", true);
        }
        else
        {
      //  anim.SetBool("FrontFlip", false);
        }

        if (ResetSystem.resetPlayer)
        {
            transform.localPosition = currentOffset;
        }
        
        /*if (leftHandIk != null && rightHandIk != null) 
                {
                    leftHandIk.data.targetPositionWeight = targetWeight;
                 leftHandIk.data.targetPositionWeight = targetWeight;
                }
        */
    }

    int whenWhilee()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    void LateUpdate()
    {
        transform.localPosition = currentOffset;
    }

    bool IsStopped()
    {
        return bike.linearVelocity.magnitude < 0.1f;
    }


}
