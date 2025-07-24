using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BikesControlerSystem : MonoBehaviour
{
    public GameObject playBike;
    public Slider slider;
    public Image counterPivot;
    public Speedometer speedometerSystem;
    public GameObject diePanel;
    public Transform target;
    public ResetPlace resetSystem;
    public Transform referenceObject;
    public GameObject atualBike;
    public FreestyleSystem bikeSystem;
    public BikeController bikeController;
    public PlayerHealth healthSystem;
    public FirstCam AtualFirstCam;
    public string refName;

    Transform parentObject;
    Vector3 initialPos;
    Quaternion initialQuat;

    bool wait, waitFc1;

    void Awake() => StartCoroutine(InitializeBike());

    void Start()
    {
        parentObject = transform;
        DeleteHealth();
        StartCoroutine(InitializeBike());
    }

    void Update()
    {
        if (atualBike)
        {
            target.position = atualBike.transform.position;
            if (!AtualFirstCam)
                AtualFirstCam = FindChildRecursively(transform, "FirstCamera")?.GetComponent<FirstCam>();
        }

        if ((!atualBike || !bikeController || !bikeSystem) && !waitFc1)
        {
            StartCoroutine(InitializeBike());
            waitFc1 = true;
        }

        if (ResetSystem.resetPlayer && !wait)
        {
            Destroy(atualBike, 0.2f);
            GameObject newObj = Instantiate(playBike, parentObject);
            newObj.transform.SetPositionAndRotation(
                healthSystem?.die == true ? initialPos : resetSystem.atualPlace,
                healthSystem?.die == true ? initialQuat : resetSystem.atualRot
            );
            StartCoroutine(InitializeBike());
            wait = true;
        }
    }

    public void DeleteHealth() => healthSystem.resetHealth = true;

    Transform FindChildRecursively(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            Transform result = FindChildRecursively(child, childName);
            if (result) return result;
        }
        return null;
    }

    IEnumerator InitializeBike()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj && (obj.name.EndsWith(refName) || obj.name.EndsWith(refName + "(Clone)")))
                atualBike = obj;
        }

        if (atualBike)
        {
            bikeSystem = atualBike.GetComponent<FreestyleSystem>();
            bikeController = atualBike.GetComponent<BikeController>();
            healthSystem = atualBike.GetComponent<PlayerHealth>();
            initialPos = atualBike.transform.position;
            initialQuat = atualBike.transform.rotation;
        }

        yield return new WaitForSeconds(2f);
        wait = false;
        waitFc1 = false;
        yield return new WaitForSeconds(1f);
    }
}
