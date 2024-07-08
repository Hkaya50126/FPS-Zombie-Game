using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using TMPro;
using Photon.Pun;
public class WeaponManager : MonoBehaviour
{
    
    public Animator playerAnimator;

    public GameObject playerCam;
    public ParticleSystem muzzleFlash;
    public GameObject hitParticles;
    public GameObject crossHair;

    bool isReloading;

    public float currentAmmo;
    public float maxAmmo;
    public float reloadTime = 2f;
    public float reserveAmmo;
    public float maxDistance = 100f;
    public float damage = 25f;
    public float ammoCap;

    public string weaponType;

    public TMP_Text currentAmmoText;
    public TMP_Text reserveAmmoText;

    public PlayerManager playerManager;

    public PhotonView photonView;

    private void OnEnable()
    {
        playerAnimator.SetTrigger(weaponType);

        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();
    }
    void Start()
    {
        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();

        ammoCap = reserveAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        if (playerAnimator.GetBool("isShooting"))
        {
            playerAnimator.SetBool("isShooting", false);

        }
        if (reserveAmmo <= 0 && currentAmmo <= 0)
        {
            //no ammo
            return;
        }
        if (currentAmmo <= 0 && isReloading == false)
        {
            //no ammo clip
            StartCoroutine(Reload(reloadTime));
            return;
        }
        if (isReloading)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0)
        {
            //manual reload
            StartCoroutine(Reload(reloadTime));
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            
            Shoot();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (weaponType!="Pistol")
            {
                Aim();
            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            if (playerAnimator.GetBool("isAiming"))
            {
                playerAnimator.SetBool("isAiming", false);
            }
            crossHair.SetActive(true);

        }
    }
    public IEnumerator Reload(float rt)
    {
        isReloading= true;
        playerAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(rt);
        playerAnimator.SetBool("isReloading", false);

        float missingAmmo = maxAmmo-currentAmmo;
        if (reserveAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reserveAmmo -= missingAmmo;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();
        isReloading = false;

    }
    void Shoot()
    {
        currentAmmo--;
        currentAmmoText.text = currentAmmo.ToString();


        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("WeaponShootVFX", RpcTarget.All, photonView.ViewID) ;
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }
        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxDistance))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
                if (enemyManager.enemyHealth <= 0)
                {
                    playerManager.currentPoint += enemyManager.point;
                }
                GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));

                instParticles.transform.parent = hit.transform;

                Destroy(instParticles, 2f);
            }
        }
    }

    public void ShootVFX(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            muzzleFlash.Play();

        }

    }

    void Aim()
    {
        playerAnimator.SetBool("isAiming", true);
        
        crossHair.SetActive(false);
    }
}
