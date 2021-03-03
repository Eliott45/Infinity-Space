using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_6 : Enemy{
    public GameObject HeroTrigger;
    public Vector3 targetPosition;
    public Vector3 tempPos;

    private void Start() {
        HeroTrigger = GameObject.FindGameObjectWithTag("Hero");
        if(HeroTrigger != null) {
            targetPosition = HeroTrigger.transform.position;
        }
        
    } 

    public override void Move(){
        if(HeroTrigger != null) {

            this.transform.position = Vector3.MoveTowards(this.transform.position,targetPosition, Time.deltaTime * speed);
            if(transform.position == targetPosition) {
                StartCoroutine(Co_WaitForSeconds(1f));
                HeroTrigger = null;
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
