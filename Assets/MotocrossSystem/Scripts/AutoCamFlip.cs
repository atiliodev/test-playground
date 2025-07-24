using UnityEngine;
using Unity.Cinemachine;

public class AutoCamFlip : MonoBehaviour
{
    private CinemachineFollow camComponent;
    public Transform target;
    private Rigidbody bikeRb;
    public Vector3 rearOffset = new Vector3(0, 2, -0.17f);
    public Vector3 frontOffset = new Vector3(0, 2, 0.17f);
    public float minToFlip = -0.5f;

    private void Start()
    {
        camComponent = GetComponent<CinemachineFollow>();
        bikeRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool isFlipped = Vector3.Dot(bikeRb.linearVelocity, target.forward) < minToFlip;

        if(bikeRb == null)
        {
            bikeRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        }

        if (camComponent != null)
        {
            camComponent.FollowOffset = Vector3.Lerp(camComponent.FollowOffset, isFlipped ? frontOffset : rearOffset, Time.deltaTime * 5);
        }
        else
        {
            camComponent = GetComponent<CinemachineFollow>();
        }
    }
}
