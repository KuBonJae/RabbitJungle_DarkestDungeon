using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Gun_Basic_Shooting : MonoBehaviour
{
    public int ammo;


    public GameObject bullet;
    public GameObject bulletEffect;
    public Transform spawnPos;

    public GameObject rotation;

    public float timeBetweenShots;
    public float shotTime;

    public bool isReloading;


    public BulletUIManager bulletUIManager;

    // Start is called before the first frame update
    void Start()
    {
        ammo = DataManager.Instance.BulletCount;
        bulletUIManager.SetBulletCount(ammo);
    }

    // Update is called once per frame
    void Update()
    {
        if (ManagingInput.GetMouseButton(0) && ammo > 0 && !isReloading)
        {
            if (Time.time > shotTime)
            {
                Instantiate(bullet, spawnPos.position, rotation.transform.rotation);
                Instantiate(bulletEffect, spawnPos.position, rotation.transform.rotation);
                ammo = ammo - 1;
                shotTime = Time.time + timeBetweenShots;

                bulletUIManager.SetBulletCount(ammo);
            }

        }
        Reload();
    }

    private void Reload()
    {
        if (ammo == 0)
        {
            StartCoroutine("ReloadTime");
        }
        if (ManagingInput.GetKeyDown(KeyCode.R))
        {
            StartCoroutine("ReloadTime");
        }
    }

    //public void AmmoText(int ammo)
    //{
    //    ammoText.text = "Ammo : " + ammo;
    //}

    IEnumerator ReloadTime()
    {
        isReloading = true;
        yield return new WaitForSeconds(1);
        ammo = DataManager.Instance.BulletCount;
        bulletUIManager.SetBulletCount(ammo);
        isReloading = false;
    }

}
