/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System;
using System.Collections;

public class BikeController : MonoBehaviour{
    ///////////////////////////////////////////////////////// wheels ///////////////////////////////////////////////////////////
    // defeine wheel colliders
    public WheelCollider coll_frontWheel;
    public WheelCollider coll_rearWheel;
    // visual for wheels
    public GameObject meshFrontWheel;
    public GameObject meshRearWheel;
    // check isn't front wheel in air for front braking possibility
    bool isFrontWheelInAir = true;
    
    //////////////////////////////////////// Stifness, CoM(center of mass), crashed /////////////////////////////////////////////////////////////
    //for stiffness counting when rear brake is on. Need that to lose real wheel's stiffness during time
    float stiffPowerGain = 0.0f;
    //for CoM moving along and across bike. Pilot's CoM.
    float tmpMassShift = 0.0f;
    // crashed status. To know when we need to desable controls because bike is too leaned.
    public bool crashed = false;
    // there is angles when bike takes status crashed(too much lean, or too much stoppie/wheelie)
    //public float crashAngle01;//crashed status is on if bike have more Z(side fall) angle than this												// 70 sport, 55 chopper
    //public float crashAngle02;//crashed status is on if bike have less Z(side fall) angle than this 												// 290 sport, 305 chopper
   // public float crashAngle03;//crashed status is on if bike have more X(front fall) angle than this 												// 70 sport, 70 chopper
    //public float crashAngle04;//crashed status is on if bike have more X(back fall) angle than this												// 280 sport, 70 chopper
    											
    // define CoM of bike
    public Transform CoM; //CoM object
    public float normalCoM; //normalCoM is for situation when script need to return CoM in starting position										// -0.77 sport, -0.38 chopper
    public float CoMWhenCrahsed; //we beed lift CoM for funny bike turning around when crashed													// -0.2 sport, 0.2 chopper
    public GameObject charLink;
   
    private Rigidbody rb;
    [Header("Curva de inclinação por velocidade")]
    public AnimationCurve leanBySpeedCurve = AnimationCurve.Linear(0, 0.2f, 100, 1.8f);

    [Header("Reação de impacto")]
    public float landImpactThreshold = -8f;     // Velocidade mínima pra considerar impacto
    public float rearBounceForce = 3000f;       // Força do quique na traseira
    public Transform rearWheelTransform;        // Ponto da força
    
    [Header("Suspensão - Peso Virtual do Piloto")]
    public RiderControler riderController;
    public float maxRearBias = 0.1f;  // Inclinação máxima pra trás
    public float maxFrontBias = 0.1f; // Inclinação máxima pra frente
    
    [Header("Influência do Freio na Suspensão Dianteira")]
    public float frontBrakeBias = 0.1f; // quanto afunda com o freio dianteiro

    [Header("Impulso na arrancada")]
    public float launchCompression = 0.3f; // o quanto afunda extra na saída
    public float accelerationSensitivity = 5f; // sensibilidade à aceleração inicial
    private float launchEffect = 0f; // valor dinâmico
    //////////////////// "beauties" of visuals - some meshes for display visual parts of bike ////////////////////////////////////////////
    public float fatorWipeAngle = 2f;
	public Transform rearPendulumn; //rear pendulumn
    public Transform steeringWheel; //wheel bar
    public Transform suspensionFront_down; //lower part of front forge
    private int normalFrontSuspSpring; // we need to declare it to know what is normal front spring state is
    private int normalRearSuspSpring; // we need to declare it to know what is normal rear spring state is
    bool forgeBlocked = true; // variable to lock front forge for front braking
    //why we need forgeBlocked ?
    //There is little bug in PhysX 3.3 wheelCollider - it works well only with car mass of 1600kg and 4 wheels.
    //If your vehicle is not 4 wheels or mass is not 1600 but 400 - you are in troube.
    //Problem is epic force created by suspension spring when it's full compressed, stretched or wheel comes underground between frames(most catastrophic situation)
    //In all 3 cases your spring will generate crazy force and push rigidbody to the sky.
    //so, my solution is find this moment and make spring weaker for a while then return to it's normal condition.
    
    private float baseDistance; // need to know distance between wheels - base. It's for wheelie compensate(dont want wheelie for long bikes)
    
    // we need to clamp wheelbar angle according the speed. it means - the faster bike rides the less angle you can rotate wheel bar
    public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f), new Keyframe(100f, 1f));//first number in Keyframe is speed, second is max wheelbar degree
    
    // temporary variable to restrict wheel angle according speed
    float tempMaxWheelAngle;
    
    //variable for cut off wheel bar rotation angle at high speed
    //float wheelPossibleAngle = 0.0f;
    
    //for wheels vusials match up the wheelColliders
    private Vector3 wheelCCenter;
    private RaycastHit hit;
    
    /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
    public float bikeSpeed; //to know bike speed km/h
    public bool isReverseOn = false; //to turn On and Off reverse speed
    // Engine
    public float frontBrakePower; //brake power absract - 100 is good brakes																		// 100 sport, 70 chopper
    public float EngineTorque; //engine power(abstract - not in HP or so)																		// 85 sport, 100 chopper
    // airRes is for wind resistance to large bikes more than small ones
    public float airRes; //Air resistant 																										// 1 is neutral
    /// GearBox
    public float MaxEngineRPM; //engine maximum rotation per minute(RPM) when gearbox should switch to higher gear 								// 12000 sport, 7000 chopper
    public float EngineRedline; 																													// 12200 sport, 7200 chopper
    public float MinEngineRPM; //lowest RPM when gear need to be switched down																	// 6000 sport, 2500 chopper
    public float EngineRPM; // engine current rotation per minute(RPM)
    // gear ratios(abstract)
    public float[] GearRatio;//array of gears                                                                                                 	// 6 sport, 5 chopper
    
    public int CurrentGear = 0; // current gear
    
    GameObject ctrlHub;// gameobject with script control variables 
    controlHub outsideControls;// making a link to corresponding bike's script

    //newDefinition of rigidBody
    public Rigidbody m_body;
		public Vector3 crashPos;
		public Quaternion crashRot;
	
    ///////////////////////////////////////////////////  ESP system ////////////////////////////////////////////////////////
    bool ESP = false;//ESP turned off by default

    ////////////////////////////////////////////////  ON SCREEN INFO ///////////////////////////////////////////////////////
    public void OnGUI()
    {
    	GUIStyle biggerText = new GUIStyle((GUIStyle)"label");
      	biggerText.fontSize = 40;
      	GUIStyle middleText = new GUIStyle((GUIStyle)"label");
      	middleText.fontSize = 22;
      	GUIStyle smallerText = new GUIStyle((GUIStyle)"label");
      	smallerText.fontSize = 14;
      	
      	//to show in on display interface: speed, gear, ESP status
    	GUI.color = UnityEngine.Color.black;
        //GUI.Label(new Rect(Screen.width*0.875f,Screen.height*0.9f, 120.0f, 80.0f), String.Format(""+ "{0:0.}", bikeSpeed), biggerText);
       // GUI.Label (new Rect (Screen.width*0.76f,Screen.height*0.88f, 60.0f, 80.0f), "" + (CurrentGear+1),biggerText);
        if (!ESP){
    		GUI.color = UnityEngine.Color.grey;
    		//GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.86f,60.0f,40.0f), "ESP", middleText);
    	} else {
    		GUI.color = UnityEngine.Color.green;
    		//GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.86f,60.0f,40.0f), "ESP", middleText);
    	}
    	 if (!isReverseOn){
    		GUI.color = UnityEngine.Color.grey;
    		//GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.96f,60.0f,40.0f), "REAR", smallerText);
    	} else {
    		GUI.color = UnityEngine.Color.red;
    		//GUI.Label (new Rect (Screen.width*0.885f, Screen.height*0.96f,60.0f,40.0f), "REAR", smallerText);
    	}
        
        // user info help lines
       /* GUI.color = UnityEngine.Color.white;
        GUI.Box (new Rect (10.0f,10.0f,180.0f,20.0f), "A,W,S,D - main control", smallerText);
        GUI.Box (new Rect (10.0f,25.0f,220.0f,20.0f), "2 - more power to accelerate", smallerText);
        GUI.Box (new Rect (10.0f,40.0f,120.0f,20.0f), "X - rear brake", smallerText);
        GUI.Box (new Rect (10.0f,55.0f,320.0f,20.0f), "Q,E,F,V - shift center of mass of biker", smallerText);
        GUI.Box (new Rect (10.0f,70.0f,320.0f,20.0f), "R - restart / RightShift+R - full restart", smallerText);
        GUI.Box (new Rect (10.0f,85.0f,180.0f,20.0f), "RMB - rotate camera around", smallerText);
      	GUI.Box (new Rect (10.0f,100.0f,120.0f,20.0f), "Z - turn on/off ESP", smallerText);
      	GUI.Box (new Rect (10.0f,115.0f,320.0f,20.0f), "C - toggle reverse", smallerText);
      	GUI.Box (new Rect (10.0f,130.0f,320.0f,20.0f), "Esc - return to main menu", smallerText);
		*/
      	GUI.color = UnityEngine.Color.black; 
      	
      	
    }
    
		float initialTorque;
    FreestyleSystem freestyleSys;
    public float center;
    public void Start() 
    {
        rb = GetComponent<Rigidbody>();
        center = normalCoM;
        ctrlHub = GameObject.Find("gameScene");//link to GameObject with script "controlHub"
    	outsideControls = ctrlHub.GetComponent<controlHub>();//to connect mulicontrol script to this one
			freestyleSys = GetComponent<FreestyleSystem>();
    	Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3 with new PhysX feature when Tensor decoupled from center of mass
    	GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
    	GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3 with new PhysX feature when Tensor decoupled from center of mass
        
        m_body = GetComponent<Rigidbody>();//seems strange, I know but three strings above strange too but necessary for PhysX ;( 

        // wheel colors for understanding of accelerate, idle, brake(white is idle status)
        meshFrontWheel.GetComponent<Renderer>().material.color = UnityEngine.Color.black;
    	meshRearWheel.GetComponent<Renderer>().material.color = UnityEngine.Color.black;
    	
    	//for better physics of fast moving bodies
    	m_body.interpolation = RigidbodyInterpolation.Interpolate;
    	
    	// too keep EngineTorque variable like "real" horse powers
    	EngineTorque = EngineTorque * 20;

			initialTorque = EngineTorque;
    	
    	//*30 is for good braking to keep frontBrakePower = 100 for good brakes. So, 100 is like sportsbike's Brembo
    	frontBrakePower = frontBrakePower * 30;//30 is abstract but necessary for Unity5
    	
    	//technical variables
    	normalRearSuspSpring = (int)coll_rearWheel.suspensionSpring.spring;
    	normalFrontSuspSpring = (int)coll_frontWheel.suspensionSpring.spring;
    	
    	baseDistance = coll_frontWheel.transform.localPosition.z - coll_rearWheel.transform.localPosition.z;// now we know distance between two wheels

        // Definição fixa e realista da suspensão
        // Suspensão dianteira
        JointSpring frontSpring = coll_frontWheel.suspensionSpring;
        frontSpring.spring = 15000f;
        frontSpring.damper = 1500f;
        frontSpring.targetPosition = 0.5f;
        coll_frontWheel.suspensionSpring = frontSpring;
        coll_frontWheel.suspensionDistance = 0.3f;

        // Suspensão traseira
        JointSpring rearSpring = coll_rearWheel.suspensionSpring;
        rearSpring.spring = 15000f;
        rearSpring.damper = 1500f;
        rearSpring.targetPosition = 0.5f;
        coll_rearWheel.suspensionSpring = rearSpring;
        coll_rearWheel.suspensionDistance = 0.3f;
    }
    
	float impulseWipe;
	void Update()
	{
			if(!freestyleSys.isGround)
			{
				impulseWipe = 4;
			}
			else
			{
				impulseWipe = 1;
			}

        

        if (ResetSystem.resetPlayer)
			{
				charLink.SetActive(true);
			}

			if (crashed)
{
    normalCoM = 0;
    m_body.centerOfMass = new Vector3(0, CoMWhenCrahsed, 0);
          
    EngineTorque = 0f;                 // zera torque ao cair
    coll_rearWheel.brakeTorque = 0f;   // não trava roda
    m_body.linearDamping = 0.1f;             // menos atrito linear
    m_body.angularDamping = 0.05f;     // quase nada de travamento rotacional

    // aplica torque baseado na velocidade pra girar de forma mais natural
    //Vector3 crashTorque = Vector3.Cross(Vector3.up, m_body.linearVelocity.normalized) * 200f;
   // m_body.AddTorque(crashTorque, ForceMode.Impulse);

    // restaurar suspensão
    JointSpring rearSpring = coll_rearWheel.suspensionSpring;
    rearSpring.spring = normalRearSuspSpring;
    rearSpring.damper = 1500f;
    rearSpring.targetPosition = 0.5f;
    coll_rearWheel.suspensionSpring = rearSpring;

    JointSpring frontSpring = coll_frontWheel.suspensionSpring;
    frontSpring.spring = normalFrontSuspSpring;
    frontSpring.damper = 1500f;
    frontSpring.targetPosition = 0.5f;
    coll_frontWheel.suspensionSpring = frontSpring;

    // resetar crashed quando parar
    if (m_body.linearVelocity.magnitude < 1f)
    {
        crashed = false;
    }
}

else
{
    if ((Input.GetAxis("VInput") < 1 && m_body.linearVelocity.magnitude > 4) || (Input.GetAxis("HInput") == 0))
    {
        normalCoM = center + -0.08f;
    }
    else
    {
        normalCoM = center;
    }
}

    }
    public GearSystem gearControl;
    public void FixedUpdate(){

		if (this == null || !gameObject.activeInHierarchy) return;
    	
    	// if RPM is more than engine can hold we should shift gear up
    	EngineRPM = coll_rearWheel.rpm * GearRatio[CurrentGear];
    	if (EngineRPM > EngineRedline){
    		EngineRPM = MaxEngineRPM;
    	}
        if (gearControl.OnN)
        {
           // CurrentGear = 0;
        }
        else
        {
            ShiftGears();
        }
        // turn ESP on (no stoppie, no wheelie, no falls when brake is on when leaning)
    	ESP = outsideControls.ESPMode;
    
    	ApplyLocalPositionToVisuals(coll_frontWheel);
    	ApplyLocalPositionToVisuals(coll_rearWheel);
     	
     	
     	//////////////////////////////////// part where rear pendulum, wheelbar and wheels meshes matched to wheelsColliers and so on
     	//beauty - rear pendulumn is looking at rear wheel
     	var tmp_cs1 = rearPendulumn.transform.localRotation;
         var tmp_cs2 = tmp_cs1.eulerAngles;
         tmp_cs2.x = 0-8+(meshRearWheel.transform.localPosition.y*100);
         tmp_cs1.eulerAngles = tmp_cs2;
         rearPendulumn.transform.localRotation = tmp_cs1;
     	//beauty - wheel bar rotating by front wheel
    	var tmp_cs3 = suspensionFront_down.transform.localPosition;
        tmp_cs3.y = (meshFrontWheel.transform.localPosition.y - 0.15f);
        suspensionFront_down.transform.localPosition = tmp_cs3;
    	var tmp_cs4 = meshFrontWheel.transform.localPosition;
        tmp_cs4.z = meshFrontWheel.transform.localPosition.z - (suspensionFront_down.transform.localPosition.y + 0.4f)/5;
        meshFrontWheel.transform.localPosition = tmp_cs4;
    
        // debug - all wheels are white in idle(no accelerate, no brake)
    	meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
    	meshRearWheel.GetComponent<Renderer>().material.color = Color.black;
    	
        // drag and angular drag for emulate air resistance
    	if (!crashed){ 
    		m_body.linearDamping = m_body.linearVelocity.magnitude / 210  * airRes; // when 250 bike can easy beat 200km/h // ~55 m/s
    		m_body.angularDamping = 7 + m_body.linearVelocity.magnitude/20;
    	}
    	// 🧨 Detecta aterrissagem brusca e aplica impulso pra levantar traseira
        bool isGroundedRear = coll_rearWheel.isGrounded;
        float verticalSpeed = m_body.linearVelocity.y;

        if (wasAirborne && isGroundedRear && verticalSpeed < landImpactThreshold)
        {
        m_body.AddForceAtPosition(Vector3.up * rearBounceForce, rearWheelTransform.position, ForceMode.Impulse);
        }

        wasAirborne = !isGroundedRear;
        //determinate the bike speed in km/h
    	bikeSpeed = Mathf.Round((m_body.linearVelocity.magnitude * 3.6f)*10f) * 0.1f; //from m/s to km/h

        // Detecta impulso na saída com base na força da aceleração
float accelerationInput = outsideControls.Vertical;
float isAccelerating = (accelerationInput > 0.1f && !crashed && !isReverseOn) ? 1f : 0f;

// Aplica impulso mais forte só no início e relaxa conforme ganha velocidade
float speed01 = Mathf.InverseLerp(0f, 50f, bikeSpeed);
float launchTarget = isAccelerating * launchCompression * (1f - speed01);

// Suaviza a entrada/saída do impulso
launchEffect = Mathf.Lerp(launchEffect, launchTarget, accelerationSensitivity * Time.fixedDeltaTime);

        // 🎯 Influência do corpo do piloto na suspensão traseira (peso virtual)
        if (riderController != null)
{
    float leanFactor = Mathf.Clamp01((riderController.riderBodyLeanFactor + 1f) * 0.5f);
    float riderWeightInfluence = Mathf.Lerp(-maxRearBias, maxFrontBias, leanFactor);

    // Relaxa o peso do corpo conforme a velocidade
    float bodyRelax01 = Mathf.InverseLerp(5f, 80f, bikeSpeed);
    float bodyRelaxFactor = Mathf.Lerp(1f, 0f, bodyRelax01);

    JointSpring rearSpring = coll_rearWheel.suspensionSpring;

    // Soma o efeito do corpo + impulso de aceleração inicial
    float totalCompression = riderWeightInfluence * bodyRelaxFactor + launchEffect;

    float finalTarget = Mathf.Clamp(0.5f + totalCompression, 0.3f, 0.85f);
    rearSpring.targetPosition = Mathf.Lerp(rearSpring.targetPosition, finalTarget, 5f * Time.fixedDeltaTime);

    coll_rearWheel.suspensionSpring = rearSpring;
}

// 🛑 Afundamento da suspensão dianteira com freio da frente
bool isFrontGrounded = !isFrontWheelInAir;
bool isFrontBrakePressed = Input.GetKey(KeyCode.RightShift);

JointSpring frontSpring = coll_frontWheel.suspensionSpring;
if (isFrontGrounded && isFrontBrakePressed)
{
    frontSpring.targetPosition = Mathf.Clamp(0.4f + frontBrakeBias, 0.3f, 0.7f);
}
else
{
    frontSpring.targetPosition = 0.4f; // valor padrão
}
coll_frontWheel.suspensionSpring = frontSpring;

    //////////////////////////////////// acceleration & brake /////////////////////////////////////////////////////////////
    //////////////////////////////////// ACCELERATE /////////////////////////////////////////////////////////////
    		if (!crashed && outsideControls.Vertical >0 && !isReverseOn){//case with acceleration from 0.0 to 0.9 throttle
    			coll_frontWheel.brakeTorque = 0.0f;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
            if (!gearControl.OnN)
            {
                coll_rearWheel.motorTorque = EngineTorque * outsideControls.Vertical;
            }
            else
            {
                coll_rearWheel.motorTorque = EngineTorque * 0;
            }
            // debug - rear wheel is green when accelerate
            //meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

            // when normal accelerating CoM z is averaged
            var tmp_cs5 = CoM.localPosition;
                tmp_cs5.z = 0.0f + tmpMassShift;
                tmp_cs5.y = normalCoM;
                CoM.localPosition = tmp_cs5;
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		//case for reverse
    		if (!crashed && outsideControls.Vertical >0 && isReverseOn){
            if (!gearControl.OnN)
            {
                coll_rearWheel.motorTorque = EngineTorque * -outsideControls.Vertical / 10 + (bikeSpeed * 50);//need to make reverse really slow
            }
            else
            {
                coll_rearWheel.motorTorque = EngineTorque * 0 / 10 + (bikeSpeed * 50);//need to make reverse really slow
            }
            // debug - rear wheel is green when accelerate
            //meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

            // when normal accelerating CoM z is averaged
            var tmp_cs6 = CoM.localPosition;
                tmp_cs6.z = 0.0f + tmpMassShift;
                tmp_cs6.y = normalCoM;
                CoM.localPosition = tmp_cs6;
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    //////////////////////////////////// ACCELERATE full throttle ///////////////////////////////////////////////////////
    		if (!crashed && outsideControls.Vertical >0.9f && !isReverseOn && !gearControl.OnN)
				{// acceleration >0.9 throttle for wheelie	
    			coll_frontWheel.brakeTorque = 0.0f;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
    			//coll_rearWheel.motorTorque = EngineTorque  * outsideControls.Vertical;//1.2 mean it's full throttle

               // meshRearWheel.GetComponent<Renderer>().material.color = Color.green;
    			m_body.angularDamping  = 31;//for wheelie stability
    			m_body.linearDamping  = 1;
    		
					
    			if (!ESP && !gearControl.OnN)
            {// when ESP we turn off wheelie

                CoM.localPosition = new Vector3(CoM.localPosition.z, CoM.localPosition.y, -(2 - baseDistance / 1.4f) + tmpMassShift);// we got 1 meter in case of sportbike: 2-1.4/1.4 = 1; When we got chopper we'll get ~0.8 as result
                                                                                                                                     //still working on best wheelie code
                float stoppieEmpower = (bikeSpeed/3)/100;
    				// need to supress wheelie when leaning because it's always fall and it't not fun at all
    				float angleLeanCompensate = 0.0f;
                    if (this.transform.localEulerAngles.z < 90){	
    					angleLeanCompensate = this.transform.localEulerAngles.z/30;
    						if (angleLeanCompensate > 0.5f){
    							angleLeanCompensate = 0.5f;
    						}
    				}
    				if (this.transform.localEulerAngles.z > 290){
    					angleLeanCompensate = (360-this.transform.localEulerAngles.z)/30;
    						if (angleLeanCompensate > 0.5f){
    							angleLeanCompensate = 0.5f;
    						}
    				}
    					
    				if (stoppieEmpower + angleLeanCompensate > 0.5f){
    					stoppieEmpower = 0.5f;
    				}
                CoM.localPosition = new Vector3(CoM.localPosition.x, -(1 - baseDistance / 2.8f) - stoppieEmpower, CoM.localPosition.z);
            
						 
            	m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
					
					}
    			//this is attenuation for rear suspension targetPosition
    			//I've made it to prevent very strange launch to sky when wheelie in new Phys3
    			if (this.transform.localEulerAngles.x > 200.0f){
    				var tmp_cs9 = coll_rearWheel.suspensionSpring;
                    tmp_cs9.spring = normalRearSuspSpring + (360-this.transform.localEulerAngles.x)*500;
                    coll_rearWheel.suspensionSpring = tmp_cs9;
    				if (coll_rearWheel.suspensionSpring.spring >= normalRearSuspSpring + 20000) {
                            var tmp_cs10 = coll_rearWheel.suspensionSpring;
                            tmp_cs10.spring = (float)(normalRearSuspSpring + 20000);
                            coll_rearWheel.suspensionSpring = tmp_cs10;
                        }
    			}
    		} else RearSuspensionRestoration();
    		
    		
    //////////////////////////////////// BRAKING /////////////////////////////////////////////////////////////
    //////////////////////////////////// front brake /////////////////////////////////////////////////////////
    		int springWeakness = 0;
            if (!crashed && outsideControls.Vertical <0 && !isFrontWheelInAir){
    
    			coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical;
    			coll_rearWheel.motorTorque = 0.0f; // you can't do accelerate and braking same time.
    			
    			//more user firendly gomeotric progession braking. But less stoppie and fun :( Boring...
    			//coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical-(1 - -outsideControls.Vertical)*-outsideControls.Vertical;
    			
    			if (!ESP)
    			{ // no stoppie when ESP is on
    			    if (bikeSpeed >1)
    			    {// no CoM pull up when speed is zero
    					
    					//when rear brake is used it helps a little to prevent stoppie. Because in real life bike "stretch" a little when you using rear brake just moment before front.
    				    float rearBrakeAddon = 0.0f;
                        if(outsideControls.rearBrakeOn)
    				    {
    						rearBrakeAddon = 0.0025f;
    				    }
    			        //@TODO uncomment has double equals?
    					var tmp_cs11 = CoM.localPosition;
                        tmp_cs11.y += (frontBrakePower/200000) + tmpMassShift / 50f - rearBrakeAddon;
                        tmp_cs11.z += 0.05f;
                        CoM.localPosition = tmp_cs11;
    				} 	
    				else if (bikeSpeed <=1 && !crashed && this.transform.localEulerAngles.z < 45 || bikeSpeed <=1 && !crashed && this.transform.localEulerAngles.z >315)
    				{
    				    if (this.transform.localEulerAngles.x < 5 || this.transform.localEulerAngles.x > 355)
    				    {
    						var tmp_cs12 = CoM.localPosition;
                            tmp_cs12.y = normalCoM;
                            CoM.localPosition = tmp_cs12;
    					}
    				}
    		
    				if (CoM.localPosition.y >= -0.1f) {
                            var tmp_cs13 = CoM.localPosition;
                            tmp_cs13.y = -0.1f;
                            CoM.localPosition = tmp_cs13;
                        }

                if (CoM.localPosition.z >= 0.2f + (m_body.mass / 1100))
                {
                    CoM.localPosition = new Vector3( CoM.localPosition.x,  0.2f + (m_body.mass / 1100), CoM.localPosition.z);
                }
    				//////////// 
    				//this is attenuation for front suspension when forge spring is compressed
    				//I've made it to prevent very strange launch to sky when wheelie in new Phys3
    				//problem is launch bike to sky when spring must expand from compressed state. In real life front forge can't create such force.
    				float maxFrontSuspConstrain = 0.0f;//temporary variable to make constrain for attenuation ususpension(need to make it always ~15% of initial force) 
    				maxFrontSuspConstrain = CoM.localPosition.z;
    				if (maxFrontSuspConstrain >= 0.5f) maxFrontSuspConstrain = 0.5f;
    				springWeakness  = (int)(normalFrontSuspSpring-(normalFrontSuspSpring*1.5f) * maxFrontSuspConstrain);
    				
    			}
    		    
          m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
						
    			// debug - wheel is red when braking
    			//meshFrontWheel.GetComponent<Renderer>().material.color = Color.red;
    			
    			//we need to mark suspension as very compressed to make it weaker
    			forgeBlocked = true;
    		} else FrontSuspensionRestoration(springWeakness);//here is function for weak front spring and return it's force slowly
    			
    		
    //////////////////////////////////// rear brake /////////////////////////////////////////////////////////
    		// rear brake - it's all about lose side stiffness more and more till rear brake is pressed
				if(!coll_rearWheel.isGrounded)
				{
					coll_rearWheel.brakeTorque = frontBrakePower;
				}

    		if (!crashed && outsideControls.rearBrakeOn){
    			coll_rearWheel.brakeTorque = frontBrakePower / 2;// rear brake is not so good as front brake
    			
    			if (this.transform.localEulerAngles.x > 180 && this.transform.localEulerAngles.x < 350){
    				var tmp_cs14 = CoM.localPosition;
                    tmp_cs14.z = 0.0f + tmpMassShift;
                    CoM.localPosition = tmp_cs14;
    			}
    			
    			var tmp_cs15 = coll_frontWheel.sidewaysFriction;
                tmp_cs15.stiffness = 1.0f - ((stiffPowerGain/2)-tmpMassShift*3);
                coll_frontWheel.sidewaysFriction = tmp_cs15;
    			
    		    //@TODO weirdness
    			stiffPowerGain += 0.025f - (bikeSpeed/10000);
    				if (stiffPowerGain > 0.9f - bikeSpeed/300){ //orig 0.90
    					stiffPowerGain = 0.9f - bikeSpeed/300;
    				}
    				var tmp_cs16 = coll_rearWheel.sidewaysFriction;
                    tmp_cs16.stiffness = 1.0f - stiffPowerGain;
                    coll_rearWheel.sidewaysFriction = tmp_cs16;

    			meshRearWheel.GetComponent<Renderer>().material.color = Color.red;
    			
    		} else{
    
    			coll_rearWheel.brakeTorque = 0.0f;
    		    //@TODO weirdness
    			stiffPowerGain -= 0.05f;
    				if (stiffPowerGain < 0){
    					stiffPowerGain = 0.0f;
    				}
    			var tmp_cs17 = coll_rearWheel.sidewaysFriction;
                tmp_cs17.stiffness = 1.0f - stiffPowerGain;
                coll_rearWheel.sidewaysFriction = tmp_cs17;// side stiffness is back to 2
    			var tmp_cs18 = coll_frontWheel.sidewaysFriction;
                tmp_cs18.stiffness = 1.0f - stiffPowerGain;
                coll_frontWheel.sidewaysFriction = tmp_cs18;// side stiffness is back to 1
    			
    		}
    		
    //////////////////////////////////// reverse /////////////////////////////////////////////////////////
    		if (!crashed && outsideControls.reverse && bikeSpeed <=0){
    				outsideControls.reverse = false;
    				if(isReverseOn == false){
    				isReverseOn = true;
    				} else isReverseOn = false;
    		}
    			
    		
    //////////////////////////////////// turnning /////////////////////////////////////////////////////////////			
    			// there is MOST trick in the code
    			// the Unity physics isn't like real life. Wheel collider isn't round as real bike tyre.
    			// so, face it - you can't reach accurate and physics correct countersteering effect on wheelCollider
    			// For that and many other reasons we restrict front wheel turn angle when when speed is growing
    			//(honestly, there was a time when MotoGP bikes has restricted wheel bar rotation angle by 1.5 degree ! as we got here :)			
    			tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeed);//associate speed with curve which you've tuned in Editor
    		
    		if (!crashed && outsideControls.Horizontal !=0){	
    		//if (!crashed && Input.GetAxis("Horizontal") !=0){//DEL OLD
    			// while speed is high, wheelbar is restricted 
    			if(Input.GetKey(KeyCode.Alpha2))
					{
						coll_frontWheel.steerAngle = tempMaxWheelAngle * outsideControls.Horizontal * 2;
						//coll_frontWheel.steerAngle = tempMaxWheelAngle * Input.GetAxis("Horizontal");//DEL OLD
						steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler (0.0f, coll_frontWheel.steerAngle * 1.2f, coll_frontWheel.transform.rotation.z);
					}
					else
					{
    			coll_frontWheel.steerAngle = tempMaxWheelAngle * impulseWipe * outsideControls.Horizontal;
    			//coll_frontWheel.steerAngle = tempMaxWheelAngle * Input.GetAxis("Horizontal");//DEL OLD
    			steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler (0.0f, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
					}
				} else
				{
					coll_frontWheel.steerAngle = 0.0f;
		
    			steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler (0.0f, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);

				} 
    		

     //////////////////////////////////// LEAN TORQUE BOOST /////////////////////////////////////////////////////////////

     // Torque base e multiplicador mais agressivo
float baseLeanTorque = 200f; // mais forte já na baixa velocidade
float speedFactor = Mathf.InverseLerp(50f, 150f, bikeSpeed); // normaliza entre 0 e 1 de 50km/h a 150km/h
float leanMultiplier = Mathf.Lerp(1f, 4f, speedFactor); // aumenta o efeito até 4x

float finalLeanTorque = baseLeanTorque * leanMultiplier;

if (!crashed && outsideControls.Horizontal != 0f)
{
    Vector3 leanDirection = transform.forward * -outsideControls.Horizontal;
    rb.AddTorque(leanDirection * finalLeanTorque * Time.fixedDeltaTime, ForceMode.Acceleration);
}

    /////////////////////////////////////////////////// PILOT'S MASS //////////////////////////////////////////////////////////
    // it's part about moving of pilot's center of mass. It can be used for wheelie or stoppie control and for motocross section in future
    		//not polished yet. For mobile version it should back pilot's mass smooth not in one tick
    		if (outsideControls.VerticalMassShift >0){
    			tmpMassShift = outsideControls.VerticalMassShift/12.5f;//12.5 to get 0.08m at final
    			var tmp_cs19 = CoM.localPosition;
                tmp_cs19.z = tmpMassShift;
                CoM.localPosition = tmp_cs19;	
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		if (outsideControls.VerticalMassShift <0){
    			tmpMassShift = outsideControls.VerticalMassShift/12.5f;//12.5 to get 0.08m at final
    			var tmp_cs20 = CoM.localPosition;
                tmp_cs20.z = tmpMassShift;
                CoM.localPosition = tmp_cs20;
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		if (outsideControls.HorizontalMassShift <0){
    		    var tmp_cs21 = CoM.localPosition;
                tmp_cs21.x = outsideControls.HorizontalMassShift/40;
                CoM.localPosition = tmp_cs21;//40 to get 0.025m at final
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    			
    		}
    		if (outsideControls.HorizontalMassShift >0){
    		    var tmp_cs22 = CoM.localPosition;
                tmp_cs22.x = outsideControls.HorizontalMassShift/40;
                CoM.localPosition = tmp_cs22;//40 to get 0.025m at final
    		    m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    		
    		//auto back CoM when any key not pressed
    		if (!crashed && outsideControls.Vertical == 0 && !outsideControls.rearBrakeOn || (outsideControls.Vertical < 0 && isFrontWheelInAir)){
    			var tmp_cs23 = CoM.localPosition;
                tmp_cs23.y = normalCoM;
                tmp_cs23.z = 0.0f + tmpMassShift;
                CoM.localPosition = tmp_cs23;
    			coll_frontWheel.motorTorque = 0.0f;
    			coll_frontWheel.brakeTorque = 0.0f;
    			coll_rearWheel.motorTorque = 0.0f;
    			coll_rearWheel.brakeTorque = 0.0f;
					
    		  m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
					
				}
    		//autoback pilot's CoM along
    		if (outsideControls.VerticalMassShift == 0){
    			var tmp_cs24 = CoM.localPosition;
                tmp_cs24.z = 0.0f;
                CoM.localPosition = tmp_cs24;
    			tmpMassShift = 0.0f;
    		}
    		//autoback pilot's CoM across
    		if (outsideControls.HorizontalMassShift == 0){
    			var tmp_cs25 = CoM.localPosition;
                tmp_cs25.x = 0.0f;
                CoM.localPosition = tmp_cs25;
    		}
    		
    /////////////////////////////////////////////////////// RESTART KEY ///////////////////////////////////////////////////////////
				
				// Restart key - recreate bike few meters above current place
    		if (ResetSystem.resetPlayer){
    			/*if (ResetSystem.resetPlayer){
    				transform.position = new Vector3(0.0f,1.0f,-11.0f);
    				transform.rotation=Quaternion.Euler( 0.0f, 0.0f, 0.0f );
    			}*/
    			crashed = false;
                EngineTorque = initialTorque;
    			//transform.position += new Vector3(0.0f,0.1f,0.0f);
    			transform.rotation = Quaternion.Euler( 0.0f, transform.localEulerAngles.y, 0.0f );
    		    m_body.linearVelocity = Vector3.zero;
    			m_body.angularVelocity = Vector3.zero;
    			var tmp_cs26 = CoM.localPosition;
                tmp_cs26.x = 0.0f;
                tmp_cs26.y = normalCoM;
                tmp_cs26.z = 0.0f;
                CoM.localPosition = tmp_cs26;
    			//for fix bug when front wheel IN ground after restart(sorry, I really don't understand why it happens);
    			coll_frontWheel.motorTorque = 0.0f;
    			coll_frontWheel.brakeTorque = 0.0f;
    			coll_rearWheel.motorTorque = 0.0f;
    			coll_rearWheel.brakeTorque = 0.0f;
    		    
    			m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    		}
    		
    		
    		
    ///////////////////////////////////////// CRASH happens /////////////////////////////////////////////////////////
/*if(freestyleSys.isGround)
{
    if ((this.transform.localEulerAngles.z >= crashAngle01 && this.transform.localEulerAngles.z <= crashAngle02) ||
        (this.transform.localEulerAngles.x >= crashAngle03 && this.transform.localEulerAngles.x <= crashAngle04))
    {
        normalCoM = 0;
        m_body.linearDamping = 0.1f;
        m_body.angularDamping = 0.01f;
        crashed = true;
                
                var tmp_cs27 = CoM.localPosition;
        tmp_cs27.x = 0.0f;
        tmp_cs27.y = CoMWhenCrahsed;
        tmp_cs27.z = 0.0f;
        CoM.localPosition = tmp_cs27;

        m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
    }
}*/
    	
    	if(crashed){
          coll_rearWheel.motorTorque = 0.0f;
          //m_body.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
          RearSuspensionRestoration();
          FrontSuspensionRestoration(normalFrontSuspSpring);
        }
    }
    
    //function Update () {
    	//not use that because everything here is about physics
    //}
    ///////////////////////////////////////////// FUNCTIONS /////////////////////////////////////////////////////////
    public void ShiftGears() {
    	int AppropriateGear = 0;
        if ( EngineRPM >= MaxEngineRPM ) {
    		AppropriateGear = CurrentGear;
    		
    		for(int i = 0; i < GearRatio.Length; i ++ ) {
    			if (coll_rearWheel.rpm * GearRatio[i] < MaxEngineRPM ) {
    				AppropriateGear = i;
    				break;
    			}
    		}
            StartCoroutine(ShiftingGear());
    		CurrentGear = AppropriateGear;
    	}
    	
    	if ( EngineRPM <= MinEngineRPM ) {
    		AppropriateGear = CurrentGear;
    		
    		for(int j = GearRatio.Length-1; j >= 0; j -- ) {
    			if (coll_rearWheel.rpm * GearRatio[j] > MinEngineRPM ) {
    				AppropriateGear = j;
    				break;
    			}
    		}
    		CurrentGear = AppropriateGear;
    	}
    }
    public bool onShift;


    IEnumerator ShiftingGear()
    {
        onShift = true;
        yield return new WaitForSeconds(0.5f);
        onShift = false;
    }
    
    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
    		if (collider.transform.childCount == 0) {
    			return;
    		}
    		
    		Transform visualWheel = collider.transform.GetChild (0);
    		wheelCCenter = collider.transform.TransformPoint (collider.center);	
    		if (Physics.Raycast (wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius)) {
    			visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
    			if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;
    			
    		} else {
    			visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
    			if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
    		}
    		Vector3 position = Vector3.zero;
    		Quaternion rotation = Quaternion.identity;
    		collider.GetWorldPose (out position, out rotation);
    
    		visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x, collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
    		visualWheel.Rotate (collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);
    
    }
    //need to restore spring power for rear suspension after make it harder for wheelie
    public void RearSuspensionRestoration(){
    if (coll_rearWheel.suspensionSpring.spring > normalRearSuspSpring)
    {
        var tmp_cs28 = coll_rearWheel.suspensionSpring;
        tmp_cs28.spring -= 500.0f;
        coll_rearWheel.suspensionSpring = tmp_cs28;
    }
}

// Recupera a força da suspensão dianteira após stoppie
public void FrontSuspensionRestoration(int sprWeakness){
    if (forgeBlocked && m_body.linearVelocity.magnitude < 5f) {
        forgeBlocked = false;
    }

    var tmp = coll_frontWheel.suspensionSpring;

    if (forgeBlocked) {
        tmp.spring = Mathf.Max(sprWeakness, normalFrontSuspSpring * 0.5f);
    }
    else if (tmp.spring < normalFrontSuspSpring) {
        tmp.spring += 5000.0f * Time.fixedDeltaTime;
    }

    // garante que não fique travado baixo
    if (tmp.spring < normalFrontSuspSpring * 0.5f) {
        tmp.spring = normalFrontSuspSpring * 0.5f;
    }

    coll_frontWheel.suspensionSpring = tmp;
}


   [SerializeField] private float alignDuration = 1f; // duração do alinhamento suave
   [SerializeField] private float alignSpeed = 2.5f;    // velocidade da interpolação
   private bool wasAirborne = false;
   private float alignTimer = 0f;
   private Quaternion targetRotation;

  private void LateUpdate()
{
    if (freestyleSys == null || m_body == null) return;

    bool isNowGrounded = freestyleSys.isGround;

    if (isNowGrounded && wasAirborne)
    {
        Vector3 horizontalVelocity = m_body.linearVelocity;
        horizontalVelocity.y = 0;

        if (horizontalVelocity.sqrMagnitude > 1f)
        {
            targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized, Vector3.up);
            alignTimer = alignDuration;
        }
    }

    wasAirborne = !isNowGrounded;

    if (alignTimer > 0f)
    {
        Quaternion currentRot = m_body.rotation;
        Quaternion newRot = Quaternion.Slerp(currentRot, targetRotation, Time.deltaTime * alignSpeed);
        m_body.MoveRotation(newRot);
        alignTimer -= Time.deltaTime;

        // Detecta aterrissagem brusca
        bool isGroundedRear = coll_rearWheel.isGrounded;
        float verticalSpeed = m_body.linearVelocity.y;

        if (wasAirborne && isGroundedRear && verticalSpeed < landImpactThreshold)
        {
        Debug.Log("🧨 Impacto detectado!");
        Vector3 bounceDirection = (transform.up + transform.forward * 0.5f).normalized;
        m_body.AddForceAtPosition(bounceDirection * rearBounceForce, rearWheelTransform.position, ForceMode.Impulse);
        }

    // Atualiza o estado para saber se estava no ar no próximo frame
    wasAirborne = !isGroundedRear;
    }
}

// ⚙️ Método extra para manter a suspensão travada com valores fixos
private void LockSuspension()
{
    JointSpring rearSpring = coll_rearWheel.suspensionSpring;
    rearSpring.spring = 10000f;
    rearSpring.damper = 1500f;
    coll_rearWheel.suspensionSpring = rearSpring;

    JointSpring frontSpring = coll_frontWheel.suspensionSpring;
    frontSpring.spring = 10000f;
    frontSpring.damper = 1500f;

    // 🌱 Recupera targetPosition suavemente pro padrão
    float desiredTarget = 0.5f; // valor neutro
    frontSpring.targetPosition = Mathf.Lerp(frontSpring.targetPosition, desiredTarget, 2f * Time.fixedDeltaTime);

    coll_frontWheel.suspensionSpring = frontSpring;

    // ✅ Se estiver muito devagar, libera forgeBlocked
    if (forgeBlocked && m_body.linearVelocity.magnitude < 1f) {
        forgeBlocked = false;
        }
    }
}