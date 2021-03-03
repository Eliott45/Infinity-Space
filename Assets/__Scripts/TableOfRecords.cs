using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableOfRecords : MonoBehaviour
{
    [Header("Set Dynamically")]
    public int[] records = new int[5];
    void Awake() {
        records[0] = PlayerPrefs.GetInt("UIRecord1");
        records[1] = PlayerPrefs.GetInt("UIRecord2");
        records[2] = PlayerPrefs.GetInt("UIRecord3");
        records[3] = PlayerPrefs.GetInt("UIRecord4");
        records[4] = PlayerPrefs.GetInt("UIRecord5");
        Text gt = this.GetComponent<Text>();
        gt.text = "1. Score: "+ records[0] + "\n" + "2. Score: "+ records[1] + "\n" + "3. Score: "+ records[2] + "\n" + "4. Score: "+ records[3] + "\n" + "5. Score: "+ records[4] + "\n";
    }
}
