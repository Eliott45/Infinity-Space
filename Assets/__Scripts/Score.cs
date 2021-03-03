using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [Header("Set Dynamically")]
    static public int score = 0;

    void Awake() {
        
        // Если уже существуют рекорды в PlayerPrefs
        if (PlayerPrefs.HasKey("UIRecord1")) {
            score = PlayerPrefs.GetInt("UIRecord1");
        }
        // Сохранить рекорд в хранилище 
        PlayerPrefs.SetInt("UIRecord1",score);
        Text gt = this.GetComponent<Text>();
        gt.text = "Record: " + score;
    }

    void Update() {
        Text gt = this.GetComponent<Text>();
        

        // Обновить рекорд
        if (score > PlayerPrefs.GetInt("UIRecord1")){
            PlayerPrefs.SetInt("UIRecord1",score);
            gt.text = "Record: " + score;
        } else if (score > PlayerPrefs.GetInt("UIRecord2") && score < PlayerPrefs.GetInt("UIRecord1")) {
            PlayerPrefs.SetInt("UIRecord2",score);
        } else if (score > PlayerPrefs.GetInt("UIRecord3") && score < PlayerPrefs.GetInt("UIRecord2")) {
            PlayerPrefs.SetInt("UIRecord3",score);
        } else if (score > PlayerPrefs.GetInt("UIRecord4") && score < PlayerPrefs.GetInt("UIRecord3")) {
            PlayerPrefs.SetInt("UIRecord4",score);
        } else if (score > PlayerPrefs.GetInt("UIRecord5") && score < PlayerPrefs.GetInt("UIRecord4")) {
            PlayerPrefs.SetInt("UIRecord5",score);
        }
    }
}
