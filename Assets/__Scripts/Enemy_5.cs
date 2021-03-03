using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_5 : Enemy
{
    [Header("Set in Inspector: Enemy_5")]
    public float waveFrequency = 2;
    // Ширина синусоиды в метрах
    public float waveWidth = 4;
    

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegateEnemy;

    private float x0;
    private float birthTime;

    void Start() {
        // Установить начальную координату Х объекта Enemy_1
        x0 = pos.x;
        StartCoroutine(Co_WaitForSeconds(0.25f));
        birthTime = Time.time;
    }
    
    void FixedUpdate() {
        
        
    }
    public override void Move(){
        Vector3 tempPos = pos;
        
        float age  = Time.time - birthTime;
        float theta = Mathf.PI * 0.5f * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0+waveWidth*sin;

        tempPos.x += speed * Time.deltaTime;
        pos = tempPos;
        // base.Move() обрабатывает движение вниз, вдоль оси Y
        base.Move();
    }

    private IEnumerator Co_WaitForSeconds(float value)
    {
        fireDelegateEnemy();
        yield return new WaitForSeconds(value);
        StartCoroutine(Co_WaitForSeconds(0.25f));
    }
}
