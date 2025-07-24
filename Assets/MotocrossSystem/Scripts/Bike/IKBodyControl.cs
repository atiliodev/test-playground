using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBodyControl : MonoBehaviour
{
    float TrembleEffect;
    float GetUpEffect;
    float GetDownEffect;

    float resultEffectX;
    float resultEffectY;
    float resultEffectZ;

    public float timeOfTremble = 6;
    public float timeOfGetUp = 6;
    public float timeOfGetDown = 6;


    [HideInInspector] public bool executeTremble;
    [HideInInspector] public bool executeGetUp;
    [HideInInspector] public bool executeGetDown;
    [Space(20)]

    bool doTremble;
    bool doUp;
    bool doDown;

    bool onDo;



    RiderControler controler;
    FreestyleSystem freestyleSystem;
    bool onTremble;
    float originX;
    Vector3 currrentRot;
    bool haveX;

    bool corretion;
    void Start()
    {
        onDo = true;
        onTremble = true;
        originX = currrentRot.x;
        controler = GetComponent<RiderControler>();
        freestyleSystem = controler.systemFreestyle;
    }

    float l_time;
    float u_time;
    float d_time;

    bool onUp;
    bool onDown;
    bool on_Tremble;
    bool hadDid;
    bool afterWalk;
    void FixedUpdate()
    {

    }
    void Update()
    {

        if (freestyleSystem == null)
        {
            freestyleSystem = controler.systemFreestyle;
        }

        controler.onLock = !onDo;

        if (controler.bike.linearVelocity.magnitude > 7)
        {
            afterWalk = true;
        }



        MakeTremble();

        CorrectioSys();
        Executer();

        IKBase();
        GetUpOrDown();

        if (!freestyleSystem.isGround && !hadDid && !onUp && afterWalk)
        {
            corretion = true;

            //executeGetDown = true;
            hadDid = true;
        }

        if (freestyleSystem.isGround && hadDid)
        {
            executeGetUp = true;
            Debug.LogWarning("Done1@#!");
            StartCoroutine(DoGetUp());
            onUp = true;
            hadDid = false;

        }

    }

    private void IKBase()
    {
        if (onTremble)
        {
            currrentRot = transform.eulerAngles;

            if (resultEffectX != 0)
            {
                currrentRot.x += resultEffectX * Time.deltaTime;
                haveX = true;
            }
            else
            {
                if (haveX)
                {
                    transform.rotation = Quaternion.identity;
                    haveX = false;
                }

            }
            transform.rotation = Quaternion.Euler(currrentRot);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(resultEffectX, 0, resultEffectZ);
        }
    }

    private void Executer()
    {
        if (executeGetUp && !onUp)
        {
            StartCoroutine(DoGetUp());
            executeGetUp = false;
            onUp = true;
        }
        if (executeGetDown && !onDown)
        {
            StartCoroutine(DoGetDown());
            executeGetDown = false;
            onDown = true;
        }

        if (executeTremble && !on_Tremble)
        {
            StartCoroutine(DoTremble());
            executeTremble = false;
            on_Tremble = true;
        }
    }

    private void CorrectioSys()
    {
        if (corretion)
        {
            StartCoroutine(Correct());
            corretion = false;
        }
    }
    IEnumerator DoTremble()
    {
        yield return new WaitForSeconds(0);
        doUp = true;
        yield return new WaitForSeconds(0.5f);
        doUp = false;
        yield return new WaitForSeconds(0.1f);
        doDown = true;
        yield return new WaitForSeconds(0.5f);
        doDown = false;
        on_Tremble = false;
        executeGetUp = true;
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator DoGetUp()
    {
        yield return new WaitForSeconds(0);
        doTremble = true;
        yield return new WaitForSeconds(1f);
        doTremble = false;
        yield return new WaitForSeconds(1f);
        //corretion = true;
        onUp = false;
    }
    IEnumerator DoGetDown()
    {
        yield return new WaitForSeconds(0);
        doDown = true;
        yield return new WaitForSeconds(2);
        doDown = false;
        onDown = false;
    }
    IEnumerator Correct()
    {
        yield return new WaitForSeconds(0);
        onDo = false;
        yield return new WaitForSeconds(0.2f);
        onDo = true;

    }
    private void MakeTremble()
    {
        if (doTremble)
        {
            l_time++;
            if (l_time < timeOfTremble)
            {
                resultEffectX = 50;
            }
            else
            {
                if (l_time < timeOfTremble * 2)
                {
                    resultEffectX = -50;
                }
                else
                {
                    resultEffectX = 0;
                }
            }
        }
        else
        {
            if (l_time > 0)
            {
                l_time = 0;
                resultEffectX = 0;
                //corretion = true;
            }
        }
    }

    private void GetUpOrDown()
    {
        if (doUp)
        {
            if (u_time < timeOfGetUp)
            {
                u_time++;
                resultEffectX = 50;
            }
            else
            {
                resultEffectX = 0;
            }
        }
        else
        {
            if (u_time > 0)
            {
                u_time++;
                if (u_time < timeOfGetUp * 2)
                {
                    resultEffectX = -50;
                }
                else
                {
                    if (u_time >= timeOfGetUp * 2)
                    {
                        resultEffectX = 0;
                        u_time = 0;
                        //corretion = true;
                    }
                }
            }
        }
        if (doDown)
        {
            if (d_time < timeOfGetDown)
            {
                d_time++;
                resultEffectX = -50;
            }
            else
            {
                resultEffectX = 0;
            }
        }
        else
        {
            if (d_time > 0)
            {
                d_time++;
                if (d_time < timeOfGetDown * 2)
                {
                    resultEffectX = 50;
                }
                else
                {
                    if (d_time >= timeOfGetDown * 2)
                    {
                        resultEffectX = 0;
                        d_time = 0;
                        corretion = true;
                    }
                }
            }
        }
    }
}
