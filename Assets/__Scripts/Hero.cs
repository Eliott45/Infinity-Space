using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    static public Hero S; // Одиночка

    [Header("Set in Inspector")]
    // Поля, управляющие движением корабля
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public int amRocket = 0; // Кол-во ракет
    private GameObject amRocketUI;
    

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    // Эта переменная хранит ссылку на последний столкнувшийся игровой объект
    private GameObject lastTriggerGo = null;

    // Обьявление нового делегата типа WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Создать поле типа WeaponFireDelegate с именем fireDelegate
    public WeaponFireDelegate fireDelegate;
    public WeaponFireDelegate rocketDelegate;
    private GameObject music;
    public Text rc;
    private bool finishShot = true;
    void Start() {
        if (S == null) {
            S = this; // Сохранить ссылку на одиночку
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        // fireDelegate += TempFire;

        amRocketUI = GameObject.Find("UIRockets");
        rc = amRocketUI.GetComponent<Text>();
        rc.text = "Rockets left : " + amRocket;

        // Очистить массив weapons и начать игру с 1 бластером
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    
    void Update()
    {
        // Извлечь информацию из класса Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Изменить transform.position, опираясь на информацию по осям
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Повернуть корабль, чтобы придать ощущение динамизма
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);

        /* Позволить кораблю выстрелить
            if(Input.GetKeyDown(KeyCode.Space)) {
                TempFire();
            }
        */

        // Произвести выстрел из всех видов оружия вызовом fireDelegate, сначала проверить нажатие клавиши: Axis("Jump)
        // затем убедиться, что значение fireDelegate не равно null, что бы избежать ошибки
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null && finishShot) {
            fireDelegate();
        }

        // Выстрельнуть из ракеты
        if(Input.GetKeyDown("r") ) {
            if(amRocket > 0) {
                amRocket--;
                finishShot = false;
                StartCoroutine(Co_WaitForSeconds(0.3f)); 
            }
        }
    }

    void TempFire() {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        // rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    void OnTriggerEnter(Collider other) {   
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // print("Triggered: " + go.name);

        // Гарантировать невозможность повторного столкновения с тем же объектом
        if (go == lastTriggerGo) {
            return;
        }
        lastTriggerGo = go;

        // Если защитное поле столкнулось с вражеским кораблем уменьшить уровень защиты на 1 и уничтожить врага
        if (go.tag == "Enemy") {
            shieldLevel--;
            Destroy(go);
        } else if (go.tag == "PowerUp") {
            // Если защитное поле столкнулось с бонусом
            AbsorbPowerUp(go);
        } else if (other.tag == "ProjectileEnemy") {
            shieldLevel--;
            Destroy(go);
        }
        else {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch(pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            case WeaponType.missile:
                amRocket++;
                rc.text = "Rockets left : " + amRocket;
                break;
            default:
                if(pu.type == weapons[0].type) { // Если оружие того же типа
                    Weapon w = GetEmptyWeaponSlot();
                    if(w != null) {
                        // Установить в pu.type
                        w.SetType(pu.type);
                    }
                } else { // Если оружие другого типа
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel {
        get {
            return(_shieldLevel);
        } 
        set {
            _shieldLevel = Mathf.Min(value, 4);
            // Если уровень поля упал до нуля или ниже
            if(value < 0){
                Destroy(this.gameObject);
                // Сообщить объекту Main.S о необходимости перезапустить игру
                Main.S.DelayedRestart(gameRestartDelay);
                music = GameObject.Find("Audio Source");
                Destroy(music);
            }
        }
    }

    Weapon GetEmptyWeaponSlot() {
        for (int i=0; i<weapons.Length; i++) {
            if(weapons[i].type == WeaponType.none) {
                return(weapons[i]);
            }
        }
        return(null);
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }

    private IEnumerator Co_WaitForSeconds(float value){
        yield return new WaitForSeconds(value);
        rocketDelegate();
        rc.text = "Rockets left : " + amRocket;
        finishShot = true;
    }
}
