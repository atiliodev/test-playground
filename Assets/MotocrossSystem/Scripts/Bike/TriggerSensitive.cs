using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensitive : MonoBehaviour
{
    public Transform raycastOrigin1; 
    public Transform raycastOrigin2; 
    public Transform raycastOrigin3; 
    public float raycastDistance = 5f; 
    public int rayCount = 36; 
    public LayerMask collisionLayers;

    public bool haveToCrash;
    public FreestyleSystem systemT; 
    public bool onSenseAngle;
    private Rigidbody rb;

    public bool onDaleyJump;
    public bool backJump;
    bool wait;
    public float rotationLimit = 50f;

    BikeController bike;
    
    void Start()
    {
        bike = GetComponent<BikeController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PerformRaycast(raycastOrigin1, false);   
        PerformRaycast(raycastOrigin2, true);   
        PerformRaycast(raycastOrigin3, true);   

        CrashDirection();

        if(!systemT.isGround && !wait && !onDaleyJump)
        {
            StartCoroutine("activeJump");
            wait = true;
        }
        if(systemT.isGround && !wait && onDaleyJump)
        {
            StartCoroutine("disableJump");
            backJump = true;
            wait = true;
        }

       
    }

    public void CrashDirection()
    {
        Vector3 currentVelocityDirection = rb.linearVelocity.normalized;

        Vector3 forwardDirection = transform.forward;
        Vector3 backwardDirection = -transform.forward;

     
        float dotProductForward = Vector3.Dot(currentVelocityDirection, forwardDirection);
        float dotProductBackward = Vector3.Dot(currentVelocityDirection, backwardDirection);

        float currentZRotation = transform.eulerAngles.z;

        
        if (currentZRotation > 180)
        {
            currentZRotation -= 360;
        }

        if (dotProductForward > 0.2f)
        {
           onSenseAngle = false;
        }
        else if (dotProductBackward > 0.2f)
        {
           onSenseAngle = true;
        }
        else if (currentZRotation < -rotationLimit || currentZRotation > rotationLimit)
        {
           onSenseAngle = true;
        }
        else if (dotProductForward < -0.2f)
        {
            onSenseAngle = false;
        }
        else
        {
           onSenseAngle = false;
        }
    
    }
    Vector3 direction1;
    Vector3 direction2;
    public void PerformRaycast(Transform origin, bool isSense)
    {
        
        
        float angleStep = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            if(isSense)
            {
                direction1 = origin.TransformDirection(new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)));


                Ray ray = new Ray(origin.position, direction1);
                RaycastHit hit;

                

                
                    if(onSenseAngle)
                    {
                        if(backJump && bike.bikeSpeed > 10)
                        {
                            Debug.Log("Air");
                            //haveTo
                            //
                            //
                            //= true;
                        }
                    }
                
            
                Debug.DrawRay(origin.position, direction1 * raycastDistance, Color.green);
            }
            else 
            {
                
                direction2 = origin.TransformDirection(new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));

    
                Ray ray = new Ray(origin.position, direction2);
                RaycastHit hit;

                
                if (Physics.Raycast(ray, out hit, raycastDistance, collisionLayers) && bike.bikeSpeed > 15)
                {
                    Debug.Log("Head");
                   // haveToCrash = true;
                }
            
                Debug.DrawRay(origin.position, direction2 * raycastDistance, Color.green);
            }
        }
    }

    IEnumerator activeJump()
    {
        yield return new WaitForSeconds(1);
        onDaleyJump = true;
        wait = false;
    }

    IEnumerator disableJump()
    {
        yield return new WaitForSeconds(1);
        onDaleyJump = false;
        backJump = false;
        wait = false;
    }

    public void ResteAll()
    {

    }
}
