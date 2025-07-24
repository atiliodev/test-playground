using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSystem : MonoBehaviour
{
    public WheelCollider coll_frontWheel, coll_rearWheel;
    public float MaxEngineRPM, EngineRedline, MinEngineRPM, EngineRPM;
    public float bikeSpeed, frontBrakePower;
    public float[] GearRatio;
    public bool OnN;
    public int indexMultiply, CurrentGear = 0;
    public Image pivotCounter;
    public Speedometer speedometerSystem;
    public float initialCounterPos = 15, factorQReduce = 150, valueCounterLimit = 245;
    public float counterRotationValue, potenciometerFactor = 150, RPMUnit;
    public BikesControlerSystem bikesControler;
    public string refName = "SetBikes";
    public GameObject atualObj;

    public float OnNRPM;
    bool waitSet;

    BikeController bikeSystem;
    FreestyleSystem fbike;

    void Start()
    {
        bikeSystem = GetComponent<BikeController>();
        fbike = GetComponent<FreestyleSystem>();
    }

    void FixedUpdate()
    {
        RPMUnit = EngineRPM / 1000;

        float rpmSource = OnN && Input.GetKey(KeyCode.Space) ? OnNRPM * 10000 : bikeSystem.EngineRPM;
        counterRotationValue = Mathf.Min(valueCounterLimit, initialCounterPos + rpmSource / factorQReduce);
        if (counterRotationValue >= valueCounterLimit) counterRotationValue -= 0.75f;

        if (pivotCounter) pivotCounter.fillAmount = counterRotationValue / potenciometerFactor;

        indexMultiply = Input.GetKey(KeyCode.Alpha2) ? 2 : 1;

        if (speedometerSystem)
        {
            speedometerSystem.speed = bikeSystem.bikeSpeed;
            speedometerSystem.currentGear = bikeSystem.CurrentGear;
            speedometerSystem.isRear = bikeSystem.isReverseOn;
            speedometerSystem.isN = OnN;
        }

        EngineRPM = coll_rearWheel.rpm * GearRatio[CurrentGear] * indexMultiply;
        if (EngineRPM > EngineRedline) EngineRPM = MaxEngineRPM;

        float targetRPM = OnN && Input.GetKey(KeyCode.Space) ? 1 : bikeSystem.EngineRPM / 9000f;
        OnNRPM = Mathf.Lerp(OnNRPM, targetRPM, 2 * Time.deltaTime);

        if (!OnN) ShiftGears(); else CurrentGear = 0;
    }

    void Update()
    {
        if ((bikesControler == null || pivotCounter == null || speedometerSystem == null) && !waitSet)
        {
            StartCoroutine(CheckApply());
            waitSet = true;
        }

        OnN = Input.GetKey("left ctrl");
    }

    IEnumerator CheckApply()
    {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.EndsWith(refName) || obj.name.EndsWith(refName + "(Clone)"))
                atualObj = obj;
        }

        bikesControler = atualObj.GetComponent<BikesControlerSystem>();
        pivotCounter = bikesControler.counterPivot;
        speedometerSystem = bikesControler.speedometerSystem;
        waitSet = false;
        yield return new WaitForSeconds(2);
    }

    public void ShiftGears()
    {
        if (EngineRPM >= MaxEngineRPM)
        {
            for (int i = 0; i < GearRatio.Length; i++)
            {
                if (coll_rearWheel.rpm * GearRatio[i] < MaxEngineRPM)
                {
                    CurrentGear = i;
                    break;
                }
            }
        }
        else if (EngineRPM <= MinEngineRPM)
        {
            for (int j = GearRatio.Length - 1; j >= 0; j--)
            {
                if (coll_rearWheel.rpm * GearRatio[j] > MinEngineRPM)
                {
                    CurrentGear = j;
                    break;
                }
            }
        }
    }
}
