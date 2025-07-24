using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileSystem : MonoBehaviour
{
    public string name;
    public int typeCareer;
    public TextMeshProUGUI textName;
    public TMP_InputField inputField;
   
  
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            name = PlayerPrefs.GetString("PlayerName");
            inputField.text = name;
        }
        
    }

    void Update()
    {
        textName.text = name;
    }

    public void SetNameProfile(string nameSet)
    {
        name = nameSet;
        PlayerPrefs.SetString("PlayerName", nameSet);
    }
}
