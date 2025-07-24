using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll_Instant : MonoBehaviour
{
    public BikeController bikeLink;
    public GameObject ragdoll;
    public GameObject set;
    public Transform target;

    public GameObject[] toActive;
    public WheelCollider[] toDesactive;

    GameObject ragdollS;
    [HideInInspector] public bool done;

    void Update()
    {
        if (bikeLink.
            
            
            
            
            ed && !done)
        {
            bikeLink.normalCoM = 0;
            bikeLink.m_body.mass = 100;
            Instantiate(ragdoll, target.position, target.rotation);
            set.SetActive(false);
            done = true;
        }
        else if (!bikeLink.
            ed && done)
        {
            RagdollDriver.DestroRagdoll();
            done = false;
        }

        /*
        if (Input.GetKey("r"))
        {
            set.SetActive(true);
            bikeLink.enabled = true;
        }

        if (!bikeLink.enabled)
        {
            for (int x = 0; x < toActive.Length; x++)
            {
                float wait = 0;
                wait++;
                if (wait > 35)
                {
                    toActive[x].SetActive(true);
                    wait = 0;
                }
                toDesactive[x].enabled = false;
            }
        }
        else
        {
            for (int x = 0; x < toActive.Length; x++)
            {
                toActive[x].SetActive(false);
                toDesactive[x].enabled = true;
            }
        }
        */
    }
}
