using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
public class PlayerManager : MonoBehaviourPunCallbacks
{
    public float health = 100f;

    public TMP_Text healthText;
    public TMP_Text pointNumber;
    public GameManagerScript gameManager;
    public CanvasGroup hurtPanel;

    public GameObject weaponHolder;
    public GameObject playerCamera;

    int activeWeaponIndex;
    GameObject activeWeapon;

    public float currentPoint;
    public float healthCap;

    public PhotonView photonView;


    void Start()
    {
        WeaponSwitch(0);
        healthCap = health;
    }
    public void Hit(float dmg)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("PlayerTakeDamage", RpcTarget.All, dmg, photonView.ViewID);
        }
        else
        {
            PlayerTakeDamage(dmg, photonView.ViewID) ;
        }
    }
    [PunRPC]
    public void PlayerTakeDamage(float dmg, int viewId)
    {
        if (photonView.ViewID == viewId)
        {
            health -= dmg;
            if (health <= 0)
            {
                gameManager.EndGame();
            }
            else
            {
                hurtPanel.alpha = 1;
            }
            healthText.text = "Health: " + health.ToString();
        } 
    }

    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.SetActive(false);
            return;
        }
        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            WeaponSwitch(activeWeaponIndex + 1);
        }

        pointNumber.text = currentPoint.ToString();

    }

    public void WeaponSwitch(int weaponIndex)
    {
        int index = 0;
        int amountOfWeapons = weaponHolder.transform.childCount;

        if (weaponIndex > amountOfWeapons - 1)
        {
            weaponIndex = 0;
        }


        foreach (Transform child in weaponHolder.transform)
        {
            if (child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);

            }
            if (index == weaponIndex)
            {
                child.gameObject.SetActive(true);
                activeWeapon = child.gameObject;
            }
            index++;
        }
        activeWeaponIndex = weaponIndex;

        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner && changedProps["weaponIndex"] != null) 
        {
            WeaponSwitch((int)changedProps["weaponIndex"]);
        }
    }

    [PunRPC]
    public void WeaponShootVFX(int viewID)
    {
        activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }


}
