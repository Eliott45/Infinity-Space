using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePhaser : MonoBehaviour
{   
   
    [Header("Set in Inspector:")]
    // Число секунд полного цикла синусоиды
    public float waveFrequency = 2;
    // Ширина синусоиды в метрах
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; // Начальное значение координаты Х
    private float birthTime;

    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    void Start() {
        x0 = pos.x;

        birthTime = Time.time;
    }
    

    void FixedUpdate(){
        // Так как pos - это свойство, нельзя напрямую изменить pos.x поэтому получим pos в виде вектора Vector3, доступного для изменения
        Vector3 tempPos = pos;
        // Значение theta изменяется с течением времени 
        float age  = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0+waveWidth*sin;
        
        pos = tempPos;

        tempPos = pos;
        tempPos.y += 35 * Time.deltaTime;
        pos = tempPos;
    }
}