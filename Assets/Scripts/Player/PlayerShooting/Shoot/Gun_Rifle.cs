using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Gun_Rifle : MonoBehaviour
{
    public int ammo;


    public GameObject bullet;
    public GameObject bigBullet;
    public GameObject bulletEffect;
    public Transform spawnPos;

    public GameObject rotation;

    public float timeBetweenShots;
    public float shotTime;

    public bool isReloading;
    public bool skill;

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
                if (!skill)
                {
                    Instantiate(bullet, spawnPos.position, rotation.transform.rotation);
                    Instantiate(bulletEffect, spawnPos.position, rotation.transform.rotation);
                    ammo = ammo - 1;
                    bulletUIManager.SetBulletCount(ammo);
                }
                else if (skill)
                {
                    Instantiate(bigBullet, spawnPos.position, rotation.transform.rotation);
                    Instantiate(bulletEffect, spawnPos.position, rotation.transform.rotation);
                }

                shotTime = Time.time + timeBetweenShots;
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

    IEnumerator ReloadTime()
    {
        isReloading = true;
        yield return new WaitForSeconds(1);
        ammo = DataManager.Instance.BulletCount;
        bulletUIManager.SetBulletCount(ammo);
        isReloading = false;
    }

}
