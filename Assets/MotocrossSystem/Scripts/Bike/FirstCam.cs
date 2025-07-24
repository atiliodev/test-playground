using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCam : MonoBehaviour
{

    public Camera camera;

    public static Camera cam;
    void Update()
    {
        cam = camera;
    }
}
