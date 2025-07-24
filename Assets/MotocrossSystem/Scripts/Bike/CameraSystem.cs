using System.Collections;
using System.Collections.Generic;
//using Unity.Cinemachine;
using UnityEngine;



public class CameraSystem : MonoBehaviour
{
    public GameObject[] cameras; // Array para câmeras normais
   // public CinemachineVirtualCamera[] cinemachineCameras; // Array para câmeras Cinemachine

    public BikesControlerSystem bikesControler;

    public int currentCameraIndex = 0;
    public bool 
        
        ;
    private int currentCinemachineIndex = 0;

    private FreestyleSystem fSystem;
    private BikeController bController;
    private GameObject rider;

    public static int CamIndex;
    void Start()
    {
        bController = GameObject.FindGameObjectWithTag("Player").GetComponent<BikeController>();
        fSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<FreestyleSystem>();
       // rider = GameObject.FindGameObjectWithTag("Rider");
        // Inicializa a primeira câmera e desativa as outras
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        cameras[0].gameObject.SetActive(true);
    }

   
    void Update()
    {
        CamIndex = currentCameraIndex;
        Camera activeCamera = Camera.main;

        if(bController == null)
        {
            bController = GameObject.FindGameObjectWithTag("Player").GetComponent<BikeController>();
        }

       /* if (cameras[4] == null && bikesControler.AtualFirstCam != null)
        {
          //  cameras[4] = bikesControler.AtualFirstCam.camera;
        }
       */

        // Verifica se o jogador pressiona a tecla 'G' para alternar entre as câmeras normais
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentCameraIndex >= cameras.Length - 1)
            {
                currentCameraIndex = 0;
            }
            else
            {
                currentCameraIndex++;
            }
        }
        if (bController != null)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (!bController.
                    
                    
                    ed)
                {
                    if (currentCameraIndex == i)
                    {
                        cameras[currentCameraIndex].gameObject.SetActive(true);
                    }
                    else
                    {
                     if(cameras[i] != null)
                        cameras[i].gameObject.SetActive(false);
                    }
                }
                else
                {

                    if (cameras.Length - 1 == i)
                    {
                        cameras[cameras.Length - 1].gameObject.SetActive(true);
                    }
                    else
                    {
                        cameras[i].gameObject.SetActive(false);
                    }

                }
            }

        }


        
        // Verifica se o jogador pressiona a tecla 'H' para alternar entre as câmeras Cinemachine
        if (Input.GetKeyDown(KeyCode.H))
        {
           // SwitchCinemachineCamera();
        }
        
    }

    public void DeactivateCurrentCamera()
    {
        // Desativa a câmera normal atualmente ativa
        if (cameras.Length > currentCameraIndex)
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);
        }
    }

    public void ActivateMainCamera()
    {
        // Ativa a câmera normal atualmente ativa
        if (cameras.Length > currentCameraIndex)
        {
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }

    void SwitchNormalCamera()
    {
        if(cameras[currentCameraIndex] == null)
        {
            currentCameraIndex = 0;
        }
        cameras[currentCameraIndex].gameObject.SetActive(false);

        currentCameraIndex++;

        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = 0;
        }

        cameras[currentCameraIndex].gameObject.SetActive(true);
    }

   /*
    void SwitchCinemachineCamera()
    {
        if (cinemachineCameras.Length > 0)
        {
            cinemachineCameras[currentCinemachineIndex].gameObject.SetActive(false);

            currentCinemachineIndex++;
            if (currentCinemachineIndex >= cinemachineCameras.Length)
            {
                currentCinemachineIndex = 0;
            }

            cinemachineCameras[currentCinemachineIndex].gameObject.SetActive(true);
        }
    }*/
}