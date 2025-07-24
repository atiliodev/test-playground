using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlace : MonoBehaviour
{
    public BikesControlerSystem bikesControler;
    public Transform[] places;
    public Transform player;
    public float maxDistanceToReset = 10f;

    public float actualDistance;
    public float toleranceValue = 5;

    public Vector3 atualPlace;
    public Quaternion atualRot;
    public int atualPs;

    public static Vector3 placeToReset;
    void Update()
    {
        placeToReset = atualPlace;
        if (player == null && bikesControler.atualBike != null)
        {
            player = bikesControler.atualBike.transform;
        }
        for (int i = 0; i < places.Length; i++)
        {
            if(player != null)
            {
                actualDistance = Vector3.Distance(places[i].position, player.position);
                if (Vector3.Distance(places[i].position, player.position) < maxDistanceToReset /*|| (Mathf.Abs(actualDistance - maxDistanceToReset) <= toleranceValue)*/)
                {
                    atualPlace = places[i].position;
                    atualRot = player.rotation;
                    atualPs = i;
                }
            }
        }
    }
}
