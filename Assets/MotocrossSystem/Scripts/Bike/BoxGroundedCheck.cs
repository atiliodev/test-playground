using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGroundedCheck : MonoBehaviour
{
    public bool onGround;
    public LayerMask LayerOfCollision;

    void OnTriggerEnter(Collider col)
    {
        if(((1 << col.gameObject.layer) & LayerOfCollision.value) != 0)
        {
            onGround = true;
        }
    }

    void OnTriggerExit()
    {
        onGround = false;
    }
}
