﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Необычное, но удобное применение Vector2. х хранит минимальное значение, а у - максимально значение для метода Random.Range(), который
    // будет вызываться позже
    public Vector2 rotMinMax = new Vector2(15,90);
    public Vector2 driftMinMax = new Vector2(.25f,2);
    public float lifeTime = 6f; // Время в секундах существования PowerUp
    public float fadeTime = 4f; // Секунд до угасания

    [Header("Set Dynamically")]
    public WeaponType type; // Тип бонуса
    public GameObject cube; // Ссылка на вложенный куб
    public TextMesh letter; // Ссылка на TextMesh
    public Vector3 rotPerSecond; // Скорость вращения
    public float birthTime;

    private Rigidbody rigid; 
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake() {
        // Получить ссылку на куб
        cube = transform.Find("Cube").gameObject;
        // Получить ссылки на TextMesh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        // Выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; // Получить случайную скорость XYZ
        // Random.onUnitSphere возвращает вектор, указывающий на случайную точку, находящуюся на поверхности сферы с радиусом 1 м и
        // и с центром в начале координат
        vel.z = 0; // Отобразить vel на плоскость XY
        vel.Normalize(); // Нормализация устанавливает длину Vector3 равной 1 м 
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Установить угол поворота этого игрового объекта равным R:[0,0,0]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity - равноценно остуствию поворота

        // Выбрать случайную скорость вращения для вложенного куба 
        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x,rotMinMax.y),
            Random.Range(rotMinMax.x,rotMinMax.y),
            Random.Range(rotMinMax.x,rotMinMax.y)
        );

        birthTime = Time.time;
    }

    private void Update() {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond*Time.time);

        // Эффект растворения куба PowerUp с течением времени
        float u = (Time.time - (birthTime+lifeTime)) / fadeTime;
        // В течение lifeTime секунд значение u будет <= 0. Затем оно станет положительным и через fadeTime секунд станет больше 1.

        // Если u >= 1, уничтожить бонус
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }

        // Использовать u для определение альфа-значения куба и букву 
        if (u > 0) {
            Color c = cubeRend.material.color;
            c.a = 1f-u;
            cubeRend.material.color = c;
            // Буква тоже должна растворяться, но медленне
            c = letter.color;
            c.a = 1f-(u*0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen) {
            // Если бонус полностью вышел за границу экрана, уничтожить его
            Destroy(this.gameObject);
        }
    }

    public void SetType(WeaponType wt) {
        // Получить WeaponDefinition из Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        // Установить цвет дочернего куба
        cubeRend.material.color = def.color;
        // Установить цвет буквы
        // letter.color = def.color;
        letter.text = def.letter; // Установить отображаемую букву
        type = wt; // В заключение установить фактический тип
    }

    public void AbsorbedBy(GameObject target) {
        // Эта функция вызывается классом Hero, когда игрок подбирает бонус 
        
        Destroy(this.gameObject);
        
    }
}
