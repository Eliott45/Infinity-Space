using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTurel : MonoBehaviour
{
    
    public float speed = 1f; 
    [Header("Set Dynamically")] 
    public GameObject[] enemy; // Массив со всеми врагами 
    public GameObject closest; 
    public GameObject target = null;
    public Vector3 tempPos;
    public Vector3 targetPosition;
    

    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    private void Start() {
        enemy = GameObject.FindGameObjectsWithTag("Enemy"); // При создании снаряда, добавляет всех существующих врагов в массив
        target = FindClosesEnemy();
        if(target!=null) {
            targetPosition = target.transform.position;
        }  
    }

    GameObject FindClosesEnemy() {
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in enemy) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }

    private void FixedUpdate() {
        if(target != null) {
            transform.LookAt(target.transform.position);
            this.transform.position = Vector3.MoveTowards(this.transform.position,targetPosition, Time.deltaTime * speed);
            if(transform.position == targetPosition) {
                StartCoroutine(Co_WaitForSeconds(1f));
                Destroy(this.gameObject);
            }
        } else {
            tempPos = this.transform.position;
            tempPos.y += speed * Time.deltaTime;
            
            pos = tempPos;
        }
        
    }

    private IEnumerator Co_WaitForSeconds(float value)
    {
        yield return new WaitForSeconds(value);
        
    }
}
