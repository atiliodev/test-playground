using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BikeAirMove
{
    public Vector3 start;
    public Vector3 end;
    public float time;
}


public class FreestyleSystem : MonoBehaviour
{
    public WheelCollider[] wheels;
    public float rotationSpeed = 50f;
    public float rotationFlipSpeed = 500f;
    public float reduceFator = 4f;
    public float wipeRotate = 95f;
    public float returnSpeed = 5f; // Velocidade do retorno, padrão 5
    public float returnThreshold = 0.01f; // Precisão do retorno
    // public float maxRotationAngleX = 45f;
    //public float minRotationAngleX = -45f;
    //public float maxRotationAngleZ = 45f;
    //public float minRotationAngleZ = -45f;
    //blic float returnSpeed = 2f;

    // public float centerOfMassAdjustmentSpeed = 7f;
    //public float maxCenterOfMassOffset = 6f;

    public BoxGroundedCheck secondFactoryCheck;
    public BikeController bkController;
    public Animator anim;
    public RiderControler animCs;
  //  public Animator animBike;
    public GameObject bikeAnim;
    public GameObject bikeTrans;
    public GameObject rider;
///public GameObject animRider;

    private Rigidbody rb;
    private Vector3 initialCenterOfMass;
    public MeshRenderer[] mesh;
    public SkinnedMeshRenderer[] meshRider;
    
    public bool isGround;
    [Space(20)]
    public List<BikeAirMove> airMoves = new List<BikeAirMove>();

    public string[] listOfCentralCommands = { "b", "n", "m", "g" };
    public string[] listOfDerivativeCommands = { "up", "down", "right", "left" };
    public Vector2Int[] listOfMatchCommands = new Vector2Int[4];
    public bool[] blockedFreestyles;

    public int outputIndexValue;
    public bool onFreeStyle;
    public bool showState;
    public float value;

    [Header("Configuração da Aterrissagem")]
    public float maxTiltAngle = 20f; // Inclinação máxima lateral permitida (X e Z)
    public float maxRotationAngle = 15f; // Rotação máxima no eixo Y permitida
    public float maxVerticalTilt = 30f; // Inclinação máxima no eixo Z (pitch) permitida

    // Variáveis de entrada adicionadas
    public float rotationInputHorizontal;
    public float rotationInputVertical;
    public float whipLimit = 30;

    public string bestWhipName = "";
    public float bestWhipScore = 0f;

    private bool isPaused = false;
    private float pauseTime = 0f;
    public string[] animationClipName = { "Test", "Nac Nac" };
    public float[] scoreOfFStyle = new float[20];
    public int[] stateMaxFramePoint = { 30, 20 };
    public bool haveToMork;
    public bool isAnim;
    bool timeFine;

    bool done;

    public TriggerSensitive triggersSense;

    public bool alreadyJump;

    Quaternion quatAnim;

    public bool animatioFall;
    private float accumulatedFrontRotation = 0f;
    private float accumulatedBackRotation = 0f;
    public bool b_onContinuosFlip;
    public bool f_onContinuosFlip;
    public bool r_onContinuosFlip;
    public bool l_onContinuosFlip;

    float initialDataFlip;

    public float accumulatedRotation = 0f; // Acumulador de rotação
    public int flipCount = 0; // Contador de flips
    public float rotationSpeedF = 100f; // Velocidade da rotação ao pressionar as teclas
    public float whipCount;
    public float pointCount;
    public bool onflip;
    public bool onWhip;
    public bool haveWhip;
    public bool haveflip;
    public bool onEvent;

    void Start()
    {


        initialDataFlip = rotationSpeed;
        /* if (!bikeAnim.activeSelf)
         {
             bikeAnim.SetActive(true);
         }
         else if (bikeAnim.activeSelf)
         {
             bikeAnim.SetActive(false);
         }
        */
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            initialCenterOfMass = rb.centerOfMass;
        }
        else
        {
            Debug.LogError("Rigidbody não encontrado.");
        }
    }
    int currentWhipLevel = 0; // 0 = nenhum, 1 = Whip, 2 = Complex, 3 = Pro (adiciona essa variável no topo do script se preferir torná-la persistente entre chamadas)

    void WhipCounter()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pointCount -= 90 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pointCount += 90 * Time.deltaTime;
        }

        float absPoint = Mathf.Abs(pointCount);
        string label = "";
        float scoreRate = 0f;

        // Determinando qual manobra é executada com base na pontuação
        if (absPoint >= 360f)
        {
            label = "360°";
            scoreRate = 45f;
        }
        else if (absPoint >= 200f)
        {
            label = "Best Whip";
            scoreRate = 70f;
        }
        else if (absPoint >= 120f)
        {
            label = "Pro Whip";
            scoreRate = 60f;
        }
        else if (absPoint >= 70f)
        {
            label = "Whip";
            scoreRate = 15f;
        }

        if (!string.IsNullOrEmpty(label))
        {
            whipCount += scoreRate * Time.deltaTime;
            whipCount = Mathf.Min(whipCount, 75f);

            ShowWhip.ShowInfoXp(label, whipCount, 10);
            haveWhip = true;

            // Atualizando a melhor pontuação e nome da manobra
            if (whipCount > bestWhipScore)
            {
                bestWhipScore = whipCount;
                bestWhipName = label;
            }
        }
    }

    // Corrige ângulos para o intervalo -180 ~ 180
    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void FlipCounter()
    {
        // Captura a entrada do jogador
        if (Input.GetKey(KeyCode.UpArrow) || f_onContinuosFlip)
        {
            // Gira para frente
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Rotate(Vector3.right * rotationSpeedF * 2 * Time.deltaTime);
                accumulatedRotation += 160 * Time.deltaTime;
            }
            else
            {
                accumulatedRotation += 60 * Time.deltaTime;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || b_onContinuosFlip)
        {
            // Gira para trás
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(Vector3.left * rotationSpeedF * 2 * Time.deltaTime);
                accumulatedRotation -= 160 * Time.deltaTime; // ⚠️ levemente aumentado
            }
            else
            {
                accumulatedRotation -= 70 * Time.deltaTime; // ⚠️ levemente aumentado
            }
        }

        // Contabiliza um flip quando atinge ±360º
        if (Mathf.Abs(accumulatedRotation) >= 170f)
        {
            haveflip = true;
            flipCount++;
            if (accumulatedRotation > 0)
            {
                ShowFlip.ShowInfoXp("FrontFlip", flipCount);
                accumulatedRotation = 0f;
            }
            else if (accumulatedRotation < 0)
            {
                ShowFlip.ShowInfoXp("BackFlip", flipCount);
                accumulatedRotation = 0f;
            }
            else
            {
                accumulatedRotation = 0f;
            }
        }
    }

    [Header("Advanced Whip Settings")]
    public float whipYawIntensity = 20f;
    public float whipPitchIntensity = 10f;
    public float whipRollIntensity = 30f;

    void Update()
    {
        AirMoveUpdate();
        // Atualiza as variáveis de entrada
        rotationInputHorizontal = Input.GetAxis("Horizontal");
        rotationInputVertical = Input.GetAxis("Vertical");

        isGroundedCheck();
        AnimSystem();
        DoAction();
        TriggerControl();
        



        if (onFreeStyle)
        {
            anim.SetLayerWeight(animCs.freestyleLayer, 1);
            anim.SetLayerWeight(animCs.simpleLayer, 0);
            animCs.currentLayer = animCs.freestyleLayer;
            Physics.gravity = new Vector3(0, -6.81f, 0);
        }
        else
        {
            anim.SetLayerWeight(animCs.freestyleLayer, 0);
            anim.SetLayerWeight(animCs.simpleLayer, 1);
            animCs.currentLayer = animCs.simpleLayer;

            Physics.gravity = new Vector3(0, -9.81f, 0);
        }
    }
    public int IndexMove;
    void AirMoveUpdate()
    {
        bool cond1 = airMoves[IndexMove].start.x > 0 || airMoves[IndexMove].start.y > 0 || airMoves[IndexMove].start.z > 0;
        bool cond2 = airMoves[IndexMove].end.x > 0 || airMoves[IndexMove].end.y > 0 || airMoves[IndexMove].end.z > 0;

       

        if (IndexMove >= 0)
        {
            
            if (!doneIT && ((cond1) || (cond2)) && airMoves[IndexMove].time > 0)
            {
                StartCoroutine(operateMove(IndexMove, airMoves[IndexMove].start, airMoves[IndexMove].end, airMoves[IndexMove].time));
                doneIT = true;
            }
        }
        
    }

    public float fatorShow1;


    private void FixedUpdate()
    {
        if (!isGround)
        {
            RotateBike();
            if (onEvent)
            {
                FlipCounter();

                // ✅ Evita whip durante flips
                if (!haveflip)
                    WhipCounter();

                if (haveflip && onFreeStyle)
                {
                    fatorShow1 = 1;
                }
            }
        }
        else
        {
            if (haveWhip)
            {
                onWhip = true;
                haveWhip = false;
            }
            if (haveflip)
            {
                onflip = true;
                haveflip = false;
            }
            b_onContinuosFlip = false;
            f_onContinuosFlip = false;
            b_FlipImpulse = 0;
            f_FlipImpulse = 0;
            l_FlipImpulse = 0;
            r_FlipImpulse = 0;
            l_onContinuosFlip = false;
            r_onContinuosFlip = false;
            valueReduce = 0;
            timeToDo = 0;
            catchValue = false;
            //Debug.Log("Condição não atendida.");
        }
    }

    void DoAction()
    {


        isAnim = IsAnyAnimationPlaying() || anim.IsInTransition(0);


        for (int i = 0; i < listOfCentralCommands.Length; i++)
        {
            for (int j = 0; j < listOfDerivativeCommands.Length; j++)
            {
                if (haveToMork || (!isGround && !secondFactoryCheck.onGround))
                {
                    //bkController.enabled = false;
                }
                else
                {
                    //bkController.enabled = true;
                }

                if (!done && !isGround && Input.GetKey(listOfDerivativeCommands[j]) && Input.GetKey(listOfCentralCommands[i]))
                {
                    StartCoroutine("Inicialize");
                    done = true;
                }
            }
        }

        if (haveToMork)
        {
            if (timeFine && (!IsAnyAnimationPlaying() || !anim.IsInTransition(animCs.currentLayer)))
            {
                haveToMork = false;
                timeFine = false;
                done = false;
            }

        }

        rider.SetActive(!bkController.crashed);

//animRider.SetActive(!bkController.crashed);
    }

    IEnumerator Inicialize()
    {
        yield return new WaitForSeconds(0);
        haveToMork = true;
        yield return new WaitForSeconds(3);
        timeFine = true;
    }

    bool IsAnyAnimationPlaying()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length > 0 && stateInfo.normalizedTime < 1;
    }

    public float f_FlipImpulse;
    float b_FlipImpulse;
    float l_FlipImpulse;
    float r_FlipImpulse;

    void impulseControl()
    {
        // Controle de impulso para o backflip
        if (b_onContinuosFlip)
        {
            b_FlipImpulse = Input.GetKey(KeyCode.Space) ? -0.85f : -0.65f; // Impulso maior ao acelerar
                                                                           //NotificationSystem.Notify("Backflip");
        }
        else
        {
            b_FlipImpulse = 0; // Reset do backflip quando desativado
        }

        // Controle de impulso para o frontflip
        if (f_onContinuosFlip)
        {
            f_FlipImpulse = Input.GetKey("up") ? 0.75f : 0.70f; // Ajuste para frontflip
                                                                // NotificationSystem.Notify("Frontflip");
        }
        else
        {
            f_FlipImpulse = 0; // Reset do frontflip quando desativado
        }


        if (r_onContinuosFlip)
        {
            if (Input.GetKey("left ctrl"))
            {
                r_FlipImpulse = 0.85f;
            }
            else
            {
                r_FlipImpulse = 0.70f;
            }
        }
        else
        {
            r_FlipImpulse = 0;
        }


        if (l_onContinuosFlip)
        {
            if (Input.GetKey("left ctrl"))
            {
                l_FlipImpulse = -0.85f;
            }
            else
            {
                l_FlipImpulse = -0.70f;
            }
        }
        else
        {
            l_FlipImpulse = 0;
        }
    }
    bool waitCmd;
    public float timeToDo;
    float valueReduce;
    Quaternion initQuat;
    Quaternion finishQuat;
    public float rotThreshold = 0.01f;


    float rotationAmountZ;
    private float whipSmooth = 0f;
    private float currentWhipTorque = 0f;
    public float whipBuildUpSpeed = 3f; // quão rápido o torque cresce
    public float whipReleaseSpeed = 3f;   // quão rápido ele volta ao centro
    bool waitAnother;
    bool catchValue;

    void RotateBike()
    {
        impulseControl();

        // 🟩 AJUSTE DA ROTAÇÃO VERTICAL (FLIP)
        float adjustedRotationSpeed = rotationFlipSpeed;
        if ((haveWhip || onWhip) && !f_onContinuosFlip && !b_onContinuosFlip)
        {
            adjustedRotationSpeed *= 0.10f;
        }

        float rotationAmountX = (rotationInputVertical + f_FlipImpulse + b_FlipImpulse) * adjustedRotationSpeed * Time.deltaTime;

        // 🟨 WHIP AVANÇADO: ROTAÇÕES EM 3 EIXOS (Roll, Yaw, Pitch)
        float yaw = 0f, pitch = 0f, roll = 0f;

        // Valores acumuladores
        float targetRoll = 0f;
        float targetPitch = 0f;
        float targetYaw = 0f;

        if (Input.GetKey(KeyCode.A) || inputKeyX) targetRoll = 1f;
        if (Input.GetKey(KeyCode.D) || inputKeyZ) targetRoll = -1f;
        if (Input.GetKey(KeyCode.S)) targetPitch = 1f;
        if (Input.GetKey(KeyCode.W) || inputKeyY) targetPitch = -1f;
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) targetYaw = -1f;
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) targetYaw = 1f;

        // Suavização
        roll = Mathf.Lerp(roll, targetRoll, Time.deltaTime * 5f);
        pitch = Mathf.Lerp(pitch, targetPitch, Time.deltaTime * 5f);
        yaw = Mathf.Lerp(yaw, targetYaw, Time.deltaTime * 5f);

        // Previne conflito com flips contínuos
        if (f_onContinuosFlip || b_onContinuosFlip)
        {
            pitch = 0f; // Desativa o pitch do whip durante flips
        }

        Vector3 whipTorque = new Vector3(
            pitch * whipPitchIntensity,
            yaw * whipYawIntensity,
            roll * whipRollIntensity
        );

        if (Input.GetKey(KeyCode.Space))
            whipTorque *= 0.3f;

        // ➕ Mostra o nome da manobra de acordo com o input
        if (Mathf.Abs(yaw) > 0.5f && pitch > 0.5f)
        {
            ShowWhip.ShowInfoXp("Turndown", whipCount, 10);
            haveWhip = true;
        }
        else if (Mathf.Abs(yaw) > 0.5f && pitch < -0.5f)
        {
            ShowWhip.ShowInfoXp("Lookback", whipCount, 10);
            haveWhip = true;
        }
        else if (Mathf.Abs(roll) > 0.5f && Mathf.Abs(yaw) < 0.1f)
        {
            ShowWhip.ShowInfoXp("Whip", whipCount, 10);
            haveWhip = true;
        }
        else if (Mathf.Abs(yaw) > 0.5f && Mathf.Abs(pitch) > 0.5f && roll < -0.5f)
        {
            ShowWhip.ShowInfoXp("Oppo Whip", whipCount, 10);
            haveWhip = true;
        }

        // 🟦 RETORNO AUTOMÁTICO AO CENTRO QUANDO SEM INPUT
        bool noWhipInput =
            !(Input.GetKey(KeyCode.A) || inputKeyX) &&
            !(Input.GetKey(KeyCode.D) || inputKeyZ) &&
            !Input.GetKey(KeyCode.S) &&
            !(Input.GetKey(KeyCode.W) || inputKeyY) &&
            !Input.GetKey(KeyCode.LeftArrow) &&
            !Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.UpArrow) &&
            !Input.GetKey(KeyCode.DownArrow);

        // Desativa retorno automático durante qualquer manobra de giro
        bool isFlippingOrRotating =
        b_onContinuosFlip ||
        f_onContinuosFlip ||
        l_onContinuosFlip ||
        r_onContinuosFlip;

        if (noWhipInput && !isGround && !isFlippingOrRotating)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, returnSpeed * Time.deltaTime);
        }

        // 🟦 AJUSTE DA ROTAÇÃO LATERAL (360)
        float rotationAmountY = 0f;

        if (l_onContinuosFlip)
        {
            rotationAmountY = -rotationFlipSpeed * 0.6f * Time.deltaTime;
        }
        else if (r_onContinuosFlip)
        {
            rotationAmountY = rotationFlipSpeed * 0.6f * Time.deltaTime;
        }
        else
        {
            rotationAmountY = (rotationInputHorizontal + l_FlipImpulse + r_FlipImpulse) * rotationSpeed * Time.deltaTime;
        }

        // WHIP ROTATION HANDLING
        if (timeToDo <= whipLimit && !waitAnother && rotationInputHorizontal != 0)
        {
            if (!catchValue)
            {
                initQuat = transform.rotation;
                catchValue = true;
            }

            if (timeToDo >= whipLimit - 0.2f)
            {
                finishQuat = transform.rotation;
            }
        }
        else
        {
            rotationAmountZ = 0;
        }

        if (rotationAmountZ == 0 && timeToDo > 0 && !onFreeStyle && rotationInputHorizontal == 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initQuat, returnSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, initQuat) < returnThreshold)
            {
                transform.rotation = initQuat;
                timeToDo = 0;
            }
        }
        else
        {
            if (onFreeStyle)
            {
                transform.Rotate(rotationAmountX, rotationAmountY, 0);
                timeToDo = 0;
                valueReduce = 0;
                catchValue = false;
            }
            else
            {
                transform.Rotate(rotationAmountX + whipTorque.x, rotationAmountY + whipTorque.y, -rotationAmountZ + whipTorque.z);
            }

            if (timeToDo == 0 && rotationAmountZ == 0)
            {
                waitAnother = false;
                catchValue = false;
            }
        }

        // CONFIGURA A VELOCIDADE BASE DA ROTAÇÃO LATERAL
        if (l_onContinuosFlip || r_onContinuosFlip)
        {
            rotationSpeed = initialDataFlip;
        }
        else
        {
            rotationSpeed = initialDataFlip / reduceFator;
        }

        // FLIP DETECÇÃO
        if (Input.GetKey("up") && Input.GetKey("left ctrl") && !f_onContinuosFlip)
        {
            f_onContinuosFlip = true;
            b_onContinuosFlip = false;
        }
        if (Input.GetKey("down") && Input.GetKey("left ctrl") && !b_onContinuosFlip)
        {
            b_onContinuosFlip = true;
            f_onContinuosFlip = false;
        }

        // 360s laterais
        if (Input.GetKeyUp("right") && Input.GetKeyUp("left ctrl") && !waitCmd)
        {
            r_onContinuosFlip = !r_onContinuosFlip;
            l_onContinuosFlip = false;
            waitCmd = false;
        }
        if (Input.GetKeyUp("left") && Input.GetKeyUp("left ctrl") && !waitCmd)
        {
            l_onContinuosFlip = !l_onContinuosFlip;
            r_onContinuosFlip = false;
            waitCmd = false;
        }
    }

    void isGroundedCheck()
    {
        if (wheels != null && wheels.Length > 0)
        {
            for (int x = 0; x < wheels.Length; x++)
            {
                if (wheels[x].isGrounded || secondFactoryCheck.onGround)
                {
                    isGround = true;

                }
                else
                {
                    isGround = false;
                }
            }
        }
    }
    int valueOfInput;
    bool onDo;

    public int pauseValue = 1;

    public float pichtToPause;

    bool catched;

    float timeValue;
    float etD;
    public bool parameterIsWork;

    private Coroutine inputCoroutine; // Controle de input


    private int lastValidIndexValue = -1; // Último valor válido do Index
    [HideInInspector] public int currentIndex = 0; // Armazena o Index atual

    bool waitFall;

    void AnimSystem()
    {
        if (!onFreeStyle)
        {
            // bikeAnim.transform.rotation = transform.rotation;
        }
        else
        {
            etD++;
            if (etD > 40)
            {
                // bikeAnim.transform.rotation = Quaternion.Euler(bikeAnim.transform.rotation.x, transform.rotation.y, bikeAnim.transform.rotation.z);
            }
        }

        if (!isGround && haveToMork)
        {
            onFreeStyle = true;
            if (!catched)
            {
                animatioFall = false;
                catched = true;
            }
        }
        else if (onFreeStyle)
        {
            catched = false;
            StartCoroutine("turnOff");
        }

       // bikeAnim.SetActive(onFreeStyle);
        anim.speed = pauseValue;

        if (onFreeStyle)
        {
           // rb.MoveRotation(bikeAnim.transform.rotation);
        }

       /* foreach (var meshItem in mesh)
        {
            meshItem.enabled = !onFreeStyle;
        }
        foreach (var riderMesh in meshRider)
        {
            riderMesh.enabled = !onFreeStyle;
        }*/

        

        if (!isGround)
        {
            HandleFreestyleInputs();
            waitFall = true;
        }
        else
        {
            if(waitFall)
            {
                StartCoroutine(FallMove());
                waitFall = false;
            }
        }

        if (/*bikeAnim.activeSelf*/ true)

        {
            anim.SetBool("Freestyle", onFreeStyle);
        }
    }

    IEnumerator FallMove()
    {
        yield return new WaitForSeconds(0);
        anim.SetBool("Fall", true);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Fall", false);
    }

    bool doneIT;

    private void HandleFreestyleInputs()
    {
        bool keyCombinationFound = false;

        for (int i = 0; i < listOfCentralCommands.Length; i++)
        {
            for (int k = 0; k < listOfMatchCommands.Length; k++)
            {
                for (int j = 0; j < listOfDerivativeCommands.Length; j++)
                {
                    bool conditional1 = (Input.GetKey(listOfCentralCommands[listOfMatchCommands[k].x]) && Input.GetKey(listOfCentralCommands[listOfMatchCommands[k].y]));
                    bool conditional2 = (Input.GetKey(listOfCentralCommands[listOfMatchCommands[k].y]) && Input.GetKey(listOfCentralCommands[listOfMatchCommands[k].x]));
                    bool isBlockedConditional2 = blockedFreestyles[i * 4 + (j + 1)];
                    
                   

                    if ((conditional1 || conditional2) && Input.GetKey(listOfDerivativeCommands[j]) && !blockedFreestyles[(k * 4 + (j + 1)) + 16])
                    {
                        onFreeStyle = true;
                        keyCombinationFound = ProcessFreestyleInput(k, j, i);
                        break;
                    }
                    else if (Input.GetKey(listOfCentralCommands[i]) && !parameterIsWork && !isBlockedConditional2 && Input.GetKey(listOfDerivativeCommands[j]))
                    {
                        onFreeStyle = true;
                        keyCombinationFound = ProcessFreestyleInput(-1, j, i);
                        break;
                    }
                }
            }
        }

        // Se nenhuma tecla for pressionada, mantém o último Index válido
        if (!keyCombinationFound && currentIndex != 0)
        {
            anim.SetInteger("Index", currentIndex);
            if (!set)
            {
                StartCoroutine(Change());
                set = true;
            }
        }
    }

    bool set;

    bool inputKeyX;
    bool inputKeyY;
    bool inputKeyZ;

    IEnumerator operateMove(int indexValue, Vector3 start, Vector3 end, float time)
    {
        yield return new WaitForSeconds(0);
        transform.Rotate(start * 2 * Time.deltaTime);
        if(start.x == 1)
        {
            inputKeyX = true;
        }
        else
        {
            inputKeyX = false;
        }
        if (start.y == 1)
        {
            inputKeyY = true;
        }
        else
        {
            inputKeyY = false;
        }
        if (start.z == 1)
        {
            inputKeyZ = true;
        }
        else
        {
            inputKeyZ = false;
        }
        yield return new WaitForSeconds(time);
        inputKeyX = false;
        inputKeyY = false;
        inputKeyZ = false;
        doneIT = false;
    }

    IEnumerator Change()
    {
        yield return new WaitForSeconds(1);
        currentIndex = 0;
        yield return new WaitForSeconds(0);
        anim.SetInteger("Index", currentIndex);
        set = false;
    }
    private bool ProcessFreestyleInput(int matchCommandIndex, int derivativeIndex, int inputIndex)
    {
        int newIndex;
        if (matchCommandIndex != -1)
        {
            newIndex = (matchCommandIndex * 4 + (derivativeIndex + 1)) + 16;
        }
        else
        {
            newIndex = inputIndex * 4 + (derivativeIndex + 1);
        }

        // Evita valores menores que 1
        if (newIndex < 1)
        {
            newIndex = 1;
        }

        // Atualiza o Index imediatamente
        if (newIndex != currentIndex)
        {
            IndexMove = newIndex - 1;
            AirMoveUpdate();
            outputIndexValue = newIndex;
            StartCoroutine(turnOff());
            currentIndex = newIndex;
            anim.SetInteger("Index", currentIndex);
        }

        return true;
    }

    private void ResetFreestyle()
    {
        pauseValue = 1;
        onFreeStyle = false;
        timeValue++;

        if (timeValue >= 4)
        {
            pichtToPause = 0;
        }

        alreadyJump = false;
        StartCoroutine("stopOperation");
    }



    IEnumerator stopOperation()
    {
        yield return new WaitForSeconds(4);
        parameterIsWork = false;
    }
    IEnumerator turnOff()
    {
        yield return new WaitForSeconds(4f);
        IndexMove = 0;
        onFreeStyle = false;
        outputIndexValue = 0;
    }

    public bool isImpactAboveThreshold = false;

    public float impactThreshold = 10f;

    void TriggerControl()
    {
        if(triggersSense.haveToCrash)
        {
            animatioFall = true;
            bkController.crashed = true;
        }




        if(bkController.crashed)
        {
            triggersSense.ResteAll();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
    Vector3 relativeVelocity = collision.relativeVelocity;
    float impactForce = relativeVelocity.magnitude;
    float collisionForce = collision.impulse.magnitude / (impactForce * 3);
    Collider collider = collision.collider;

    // Obtém os ângulos atuais de inclinação e rotação
    float currentTiltX = Mathf.Abs(transform.eulerAngles.x); // Inclinação lateral (X)
    float currentTiltZ = Mathf.Abs(transform.eulerAngles.z); // Inclinação lateral (Z)
    float currentRotationY = Mathf.Abs(transform.eulerAngles.y); // Rotação no eixo Y
    float currentVerticalTilt = Mathf.Abs(transform.eulerAngles.x); // Inclinação frontal/traseira (pitch)

    // Corrige ângulos acima de 180° para evitar problemas
    if (currentTiltX > 180) currentTiltX = 360 - currentTiltX;
    if (currentTiltZ > 180) currentTiltZ = 360 - currentTiltZ;
    if (currentRotationY > 180) currentRotationY = 360 - currentRotationY;
    if (currentVerticalTilt > 180) currentVerticalTilt = 360 - currentVerticalTilt;

        c1 = collisionForce >= impactThreshold || onFreeStyle;
        c2 = !collider is CapsuleCollider;
        c3 = isGround && (currentTiltX > maxTiltAngle || currentTiltZ > maxTiltAngle || currentRotationY > maxRotationAngle || currentVerticalTilt > maxVerticalTilt);
    // Verifica se a moto aterrissou com inclinação, rotação ou pitch excessivo
    if (c1 || c3)
    {
        animatioFall = true;
        bkController.crashed = true;
        isImpactAboveThreshold = true;
            Debug.Log("Here Collision");
    }
}
    public bool c1;
    public bool c2;
    public bool c3;
   
    private string GetCurrentAnimationName(AnimatorStateInfo stateInfo)
    {
        foreach (string animationName in animationClipName)
        {
            if (stateInfo.IsName(animationName))
            {
                Debug.Log("Animação" + animationName);
                return animationName;
            }
        }
        return string.Empty;
    }

    void PauseAnim(int indexInput)
    {
        if (Input.GetKeyDown(listOfCentralCommands[indexInput]))
        {
            PauseAnimationAtFrame(stateMaxFramePoint[indexInput], indexInput);
        }

        if (Input.GetKeyUp(listOfCentralCommands[indexInput]))
        {
            ResumeAnimation();
        }
    }

    public void PauseAnimationAtFrame(int frame, int i)
    {
        if (anim != null && !isPaused)
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            float clipLength = currentState.length;
            float frameTime = (float)frame / currentState.length;
            pauseTime = frameTime;

            anim.Play(animationClipName[i], 0, pauseTime);
            anim.speed = 0f;
            isPaused = true;
        }
    }

    public void ResumeAnimation()
    {
        if (anim != null && isPaused)
        {
            anim.speed = 1f; // Retoma a animação
            isPaused = false;
            onDo = false;
        }
    }
}