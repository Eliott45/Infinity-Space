using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;

public class HotKeys : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject UITips;

    private GameObject music;
    private bool switchTips = true;

    void Update()
    {
        if (Input.GetKeyDown("escape"))  
        {
            music = GameObject.Find("Audio Source");
            Destroy(music);
            SceneManager.LoadScene("Menu");  
        }

        if (Input.GetKeyDown("h"))
        {   
            if(switchTips) {
                HideTips();   
            } else {
                ShowTips();
            }  
        }
    }

    public void ShowTips() {
        UITips.SetActive(true);
        switchTips =  true;
    }

    public void HideTips() {
        UITips.SetActive(false);
        switchTips = false;
    }
}
