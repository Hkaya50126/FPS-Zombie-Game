using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int price = 50;

    public TMP_Text priceNumber;
    public TMP_Text priceText;

    PlayerManager playermanager;

    bool playerIsInReach = false;
    public bool healthStation;
    public bool ammoStation;

    void Start()
    {
        priceNumber.text = "Press E To Get";


    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInReach)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                BuyShop();
            }
        }
    }
    public void BuyShop()
    {
        if (playermanager.currentPoint >= price)
        {
            playermanager.currentPoint -= price;
            if (healthStation)
            {
                playermanager.health = playermanager.healthCap;
                playermanager.healthText.text = "Health: "+ playermanager.health.ToString();
            }
            if (ammoStation)
            {
                foreach (Transform child in playermanager.weaponHolder.transform)
                {
                    WeaponManager weaponManager = child.GetComponent<WeaponManager>();
                    weaponManager.reserveAmmo = weaponManager.ammoCap;
                    StartCoroutine(weaponManager.Reload(weaponManager.reloadTime));
                    weaponManager.reserveAmmoText.text = weaponManager.reserveAmmo.ToString();
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            priceNumber.text = "Price: " + price.ToString() + "$";

            //priceNumber.gameObject.SetActive(true);
            priceText.gameObject.SetActive(true);
            playerIsInReach = true;

            playermanager=other.GetComponent<PlayerManager>();
            priceText.text = "Your Point: " + playermanager.currentPoint.ToString();


        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            priceNumber.text = "Press E To Get";
            priceText.gameObject.SetActive(false);
            playerIsInReach = false;
        }
    }
}
