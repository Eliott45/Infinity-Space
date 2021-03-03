using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Это перечисление всех возможных типов оружия.
/// Также включает тип "shield", что бы дать возможность совершенствовать защиту.
/// </summary>
public enum WeaponType {
    none,       // По умолчанию / нет оружия
    blaster,    // Простой бластер
    spread,     // Веерная пушка, стреляющая несколькими снарядами
    phaser,     // Волновой фазер
    missile,    // Самонаводящиеся ракеты
    laser,      // Наносит повреждения при долговременном воздействии
    shield,      // Увеличивает shieldLevel
    turel   // Турель стреляет в сторону врага
}

/// <summary>
/// Класс WeaponDefinition позволяет настраивать свойства конкретного вида оружия в инспекторе. 
/// Для этого класс Main будет хранить массив элементов типа WeaponDefinition
/// </summary>
[System.Serializable]
public class WeaponDefinition {
    public WeaponType type = WeaponType.none;
    public string letter; // Буква на кубике, изображающем бонус
    public Color color = Color.white; // Цвет свола оружия и кубика бонуса
    public GameObject projectilePrefab; // Шаблон снарядов
    public GameObject projectilePrefabTwin; 
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Разрушительная мощность
    public float continuosDamage = 0; // Степень разрушения в секунду (для Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; // Скорость полета снарядов
}


public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")] [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Время последнего выстрела
    private Renderer collarRend;
    public bool twin = false; // Двойник для фазера
    public LineRenderer line; // Луч лазера

    void Start() {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Луч лазера
        if(transform.parent.gameObject.tag == "Hero"){
            line = gameObject.GetComponent<LineRenderer>();
            line.enabled = false;
        }
        
        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию WeaponType.none
        SetType(_type);
        // Динамически создать точку привязки для всех снарядов
        if(PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }    
        // Найти fireDelegate в корневом игровом объекте
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null) {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
            rootGO.GetComponent<Hero>().rocketDelegate += rocketFire;
        }

        if(rootGO.GetComponent<Enemy_5>() != null) {
            rootGO.GetComponent<Enemy_5>().fireDelegateEnemy += Fire;
        }
    }

    public WeaponType type {
        get { return(_type); }
        set { SetType(value);}
    }

    public void SetType(WeaponType wt) {
        _type = wt;
        if(type == WeaponType.none) {
            this.gameObject.SetActive(false);
            return;
        } else {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // Сразу после установки _type можно выстрелить
    }

    public void Fire() {
        // Если this.gameObject неактивен, выйти
        if(!gameObject.activeInHierarchy) return;
        // Если между выстрелами прошло недостаточно много времени, выйти
        if(Time.time - lastShotTime < def.delayBetweenShots) {
            return;
        }
        Projectile p;
        
        Vector3 vel = Vector3.up * def.velocity;
        if(transform.up.y < 0) {
            vel.y = - vel.y;
        }
        if(transform.parent.gameObject.tag == "Hero"){
            line.enabled = false;
        }
        switch(type) {
            case WeaponType.blaster:
                
                p = MakeProjectile();
                if(transform.parent.gameObject.tag == "Hero"){
                    p.rigid.velocity = vel;
                } else {
                    p.rigid.velocity = Vector3.down * def.velocity;
                    
                }
                break;
            case WeaponType.spread: 
                p = MakeProjectile(); // Снаряд, летящий прямо
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Снаряд, летящий вправо
                p.transform.rotation = Quaternion.AngleAxis(10,Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Снаряд, летящий более право
                p.transform.rotation = Quaternion.AngleAxis(20,Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Снаряд, летящий влево
                p.transform.rotation = Quaternion.AngleAxis(-10,Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Снаряд, летящий более лево
                p.transform.rotation = Quaternion.AngleAxis(-20,Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
            case WeaponType.phaser: 
                twin = true;
                p = MakeProjectile();
                break;
            case WeaponType.laser: 
                StopCoroutine("FireLaser");
                StartCoroutine("FireLaser");
                break;  
            case WeaponType.missile: 
                p = MakeProjectile();
                break;  
            case WeaponType.turel: 
                p = MakeProjectile();
                break; 
        }
    }

   public void rocketFire() {

        // Если this.gameObject неактивен, выйти
        if(!gameObject.activeInHierarchy) return;
        // Если между выстрелами прошло недостаточно много времени, выйти
        if(Time.time - lastShotTime < def.delayBetweenShots) {
            return;
        }
        Projectile p;

        Vector3 vel = Vector3.up * def.velocity;
        if(transform.up.y < 0) {
            vel.y = - vel.y;
        }

        WeaponType tempType = type;
        SetType(WeaponType.missile);
        p = MakeProjectile();
        SetType(tempType);
        
        return;  
   }
    
    public Projectile MakeProjectile() {

        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero") {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        } else {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        
        if(twin) {
            twin = false;
            GameObject goTwin = Instantiate<GameObject>(def.projectilePrefabTwin);
            goTwin.tag = "ProjectileHero";
            goTwin.layer = LayerMask.NameToLayer("ProjectileHero");
            goTwin.GetComponent<MeshRenderer>().material = go.GetComponent<MeshRenderer>().material;
            goTwin.transform.position = collar.transform.position;
           
            goTwin.transform.SetParent(PROJECTILE_ANCHOR, true);
        }

        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return(p);
    }

    // Лазер
    public  IEnumerator FireLaser()
    {
        line.enabled = true;

        while (Input.GetButton("Jump")) { 
            Ray ray = new Ray(transform.position, transform.up);

            RaycastHit hit;

            

            line.SetPosition(0, ray.origin);

            if (Physics.Raycast(ray, out hit, 100))
            {
                line.SetPosition(1, hit.point);
                GameObject hitObject = hit.transform.gameObject; // Получаем объект в который попал луч
                if(hitObject.tag == "Enemy") {
                    Enemy target = hitObject.GetComponent<Enemy>();
                    target.ReactToHit(); 
                }
                
            }
            else
            {
                line.SetPosition(1, ray.GetPoint(100));
            }

            
            yield return null;
        }
        line.enabled = false;
    }
}


