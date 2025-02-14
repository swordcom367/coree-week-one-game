﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class EnemyControler : MonoBehaviour
{
    //------------------
    //detection
 public Collider sight;
 public bool PlayerInSight;
 public GameObject player;
public bool playerSeen;
public float maxWaitTime;
public float waitTime;
private Vector3 dire;
//---------------------
//UI
public Image inZone;
public Image alerted;
public Image healthBar;
public Image stuned;
//---------------------
//AI 
public NavMeshAgent agent;
public Transform[] wayPoints;
int m_curentwaypointIndex;
public float AIwaitTime;
public float AImaxWaitTime;
    public GameObject soundmanier;
//---------------------
// enemy health and dying
public float starthealth;
public float health;
public float stunTime;
public float stunthresh;
public float hurtThresh;
//---------------------
//damageing the player
public Transform shootPos;
public GameObject bullet;
public float bulletSpeed;
public float despawnTime;
public float timebtwshots;
public float maxtimeBtwshots;
    // Start is called before the first frame update
    void Start()
    {
       player=GameObject.FindGameObjectWithTag("Player");
       sight=gameObject.GetComponent<SphereCollider>();
       inZone.enabled=false;
       agent.SetDestination(wayPoints[0].position);
       health=starthealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount=health/starthealth;
        if(stunTime>0) {
            stuned.enabled=true;
            agent.isStopped=true;
            stunTime-=Time.deltaTime;
        }
        if(stunTime<=0) {
        DetectionManinger();
        stuned.enabled=false;
        if(playerSeen==true) {
            agent.isStopped=true;
            gameObject.transform.LookAt(player.transform.position,Vector3.up);
            if(timebtwshots>=maxWaitTime) {
            shoot();
            timebtwshots=0;
            } else {
                timebtwshots+=Time.deltaTime;
            }
        } else {
            agent.isStopped=false;
        }
            if (soundmanier.GetComponent<SoundAi>().B_Investagate == true)
            {
                investigate(soundmanier.GetComponent<SoundAi>().suspisiospos);
            }
        }
    if(health<= 0) {
        die();
    }
        if (health > 0)
        {
            if (soundmanier.GetComponent<SoundAi>().B_Investagate == false)
            {
                patroling();
            }
        }
    }

private void DetectionManinger() {
    if(PlayerInSight==true) {
          RaycastHit hit;
          if(player.GetComponent<PlayerControler>().crouchState==false) {
            dire =player.transform.position -transform.position +Vector3.up;
          } else { 
             dire =player.transform.position -transform.position;
          }
           Ray ray= new Ray(transform.position,dire);
          if(Physics.Raycast(ray,out hit)) {
              if(hit.collider.CompareTag("Player")) {
              if(waitTime>maxWaitTime) {
                  playerSeen=true;
              } else {
                  waitTime+=Time.deltaTime;
                  alerted.fillAmount= waitTime/maxWaitTime;
              }
              } else {
                  waitTime=0;
                  playerSeen=false;
              }
          }
      }
    }
    private void OnTriggerEnter(Collider other) {
    if(other.CompareTag("Player")) {
        inZone.enabled=true;
        PlayerInSight=true;
    }
    }
 private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            PlayerInSight = false;
            playerSeen = false;
            inZone.enabled = false;
            alerted.fillAmount = 0;
            waitTime = 0;
        }
    }
    public void patroling() {
if(agent.remainingDistance <= agent.stoppingDistance) {
if (AIwaitTime>=AImaxWaitTime) {
    m_curentwaypointIndex= (m_curentwaypointIndex + 1) % wayPoints.Length;
    agent.SetDestination(wayPoints[m_curentwaypointIndex].position);
    AIwaitTime=0;
} else {
    AIwaitTime+=Time.deltaTime;
}
}
}
    private void investigate(Vector3 suspos)
    {
        agent.SetDestination(suspos);
        if(agent.remainingDistance<= agent.stoppingDistance)
        {
            soundmanier.GetComponent<SoundAi>().suspisiospos=Vector3.zero;
            agent.SetDestination(wayPoints[m_curentwaypointIndex].position);
            soundmanier.GetComponent<SoundAi>().B_Investagate = false;
        }
    }
    private void OnCollisionEnter(Collision other) {
    if(other.collider.CompareTag("interactable")) {
        Damage(other.gameObject.GetComponent<interactableStats>().velocity,other.gameObject.GetComponent<interactableStats>().Damage,other.gameObject.GetComponent<interactableStats>().stunTime);
        AIwaitTime=0;
    }
}
public void Damage(float velocity,float damage,float forenstunTime) {
    if(velocity>hurtThresh) {
        health-=damage;
    } 
     if(velocity> stunthresh) {
        stunTime=forenstunTime;
    }
}
private void shoot() {
  var bulletl = Instantiate (bullet, transform.position, shootPos.rotation);
        bulletl.GetComponent<Rigidbody> ().velocity = transform.forward * bulletSpeed;
        Destroy (bulletl, despawnTime);
}
public void die() {
    Destroy(gameObject);
}
}
