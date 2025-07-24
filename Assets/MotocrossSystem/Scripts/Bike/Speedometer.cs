using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public float speed;
    public int currentGear; 
    public bool isRear;
    public bool isN;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gearText;

    void Start()
    {
        
    }

    
    void Update()
    {
        speedText.text = "" + speed.ToString("f0");
        if(!isRear && !isN)
        {
            gearText.text = "" + (currentGear + 1).ToString("f0");
        }
        else if (isRear && !isN)
        {
            gearText.text = "N";
        }
        else if (!isRear && isN)
        {
            gearText.text = "N";
        }

    }
}
