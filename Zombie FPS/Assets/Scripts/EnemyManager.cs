using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;

    public Animator enemyAnimator;
    public float damage = 20f;
    public float enemyHealth = 100f;
    public GameManagerScript gameManager;
    public Slider slider;

    public bool playerInReach;
    private float attackDelayTimer;
    public float attackAnimationStartDelay;
    public float delayBetweenAttacks;

    public int point = 20;

    GameObject[] playersInScene;
    public PhotonView photonView;
    void Start()
    {
        playersInScene = GameObject.FindGameObjectsWithTag("Player");
        slider.maxValue = enemyHealth;
        slider.value = enemyHealth;
    }
    void Update()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GetClosestPlayer();
        if (player != null)
        {
            slider.transform.LookAt(player.transform);
            GetComponent<NavMeshAgent>().destination = player.transform.position;
        }
        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1) 
        {
            enemyAnimator.SetBool("isRunning", true) ;
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }
    }

    private void GetClosestPlayer()
    {
        float minDistance = Mathf.Infinity;
        Vector3 curPosition = transform.position;

        foreach (GameObject thisPlayer in playersInScene)
        {
            if (thisPlayer != null)
            {
                float distance = Vector3.Distance(thisPlayer.transform.position, curPosition);
                if (distance < minDistance)
                {
                    player = thisPlayer;
                    minDistance = distance;
                }
            }
        }
    }

    public void Hit(float dmg)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, dmg, photonView.ViewID);

    }
    [PunRPC]
    public void TakeDamage(float dmg, int viewId)
    {
        if (photonView.ViewID == viewId)
        {
            enemyHealth -= dmg;
            slider.value = enemyHealth;
            if (enemyHealth <= 0)
            {
                enemyAnimator.SetTrigger("isDead");
                if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    gameManager.enemiesAlive--;

                }
                Destroy(gameObject, 5f);

                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GetComponent<EnemyManager>());
                Destroy(GetComponent<CapsuleCollider>());

            }
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }
        if (attackDelayTimer >= delayBetweenAttacks - attackAnimationStartDelay && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            enemyAnimator.SetTrigger("isAttacking");
        }
        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }
    
}
