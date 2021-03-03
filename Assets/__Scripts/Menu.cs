using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject UIMenu;
    public GameObject UIGameMode;
    public GameObject UICredits;
    public GameObject UIRecords;
    public AudioSource audioSource;
    public Text UIMusicText;

    public void StartGame() {
        DontDestroyOnLoad(audioSource);
        SceneManager.LoadScene("InfinityMode");
    }

    public void StartGameHard() {
        DontDestroyOnLoad(audioSource);
        SceneManager.LoadScene("InfinityModeHard");
    }

    public void ShowGameMode(){
        UIMenu.SetActive(false);
        UIGameMode.SetActive(true);
    }
    
    public void HideGameMode() {
        UIMenu.SetActive(true);
        UIGameMode.SetActive(false);
    }

    public void Update() {
        if (Input.GetKeyDown("escape"))  
        {
            Application.Quit();       
        }
    }

    public void Exit() {
        Application.Quit();   
    }

    public void ShowCredits() {
        UIMenu.SetActive(false);
        UICredits.SetActive(true);
    }

    public void HideCredits() {
        UIMenu.SetActive(true);
        UICredits.SetActive(false);
    }

    public void ControlMusic() {
        audioSource.mute = !audioSource.mute;
        if(audioSource.mute) {
            UIMusicText.text = "Enable music";
        } else {
            UIMusicText.text = "Disable music";
        }
    }

    public void ShowRecords() {
        UIMenu.SetActive(false);
        UIRecords.SetActive(true);
    }

    public void HideRecords() {
        UIMenu.SetActive(true);
        UIRecords.SetActive(false);
    }

}
