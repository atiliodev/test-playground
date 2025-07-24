using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelCollisionEffect : MonoBehaviour
{
    public AudioSource audioOfLanding;
    public FreestyleSystem freestyleSystem;
    
    bool canMakeShoot;

    private WheelCollider wheelCollider;

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        if(freestyleSystem.alreadyJump && !canMakeShoot)
        {
            canMakeShoot = true;
        }

        WheelHit hit;
        if (wheelCollider.GetGroundHit(out hit))
        {
            if (hit.force > 0.1f && canMakeShoot)
            {
                audioOfLanding.Play();
                canMakeShoot = false;
            }
        }
    }

    void OnCollisionEnter (Collision collision)
    {

        if(collision.impulse.magnitude >= 10 && canMakeShoot)
        {
            audioOfLanding.Play();
            canMakeShoot = false;
        }	
    }
}
