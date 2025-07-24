using System.Collections;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    private BikesControlerSystem bikes;

    public AudioSource screamFall;
    public AudioSource successFreestyle;

    public static bool successF1;

    static bool waitToSuccessAgain;
    bool waitToFall, toRestartScream;

    void Start() => bikes = GetComponent<BikesControlerSystem>();

    void Update()
    {
        if (successF1)
        {
            StartCoroutine(PlaySuccessRoutine());
            successF1 = false;
        }

        if (bikes?.bikeController != null)
        {
            bool crashed = bikes.bikeController.crashed;

            if (crashed && !waitToFall)
            {
                screamFall.Play();
                waitToFall = true;
            }
            else if (!crashed && waitToFall && !toRestartScream)
            {
                StartCoroutine(ResetFallStateRoutine());
                toRestartScream = true;
            }
        }
    }

    IEnumerator PlaySuccessRoutine()
    {
        successFreestyle.Play();
        yield return new WaitForSeconds(0.001f);
        waitToSuccessAgain = true;
        yield return new WaitForSeconds(3f);
        waitToSuccessAgain = false;
    }

    IEnumerator ResetFallStateRoutine()
    {
        yield return new WaitForSeconds(0.7f);
        waitToFall = false;
        toRestartScream = false;
    }

    public static void doneSomethingSFX()
    {
        if (!waitToSuccessAgain)
        {
            successF1 = true;
            waitToSuccessAgain = true;
        }
    }
}
