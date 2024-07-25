using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongEnemyShoot : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    Transform enemyBulletSpawnPointTransform;
    //private float fireRate = 1.0f;
    // Start is called before the first frame update

    float alertDelayTime = 0.5f; // after 0.5s enemy shoots
    float shootDelayTime = 2.0f; // shoot cool time
    float alertCoolTime = 0f;
    float shootCoolTime = 0f;
    bool alertState = false;
    bool shootState = false;
    bool cooldownState = false;
    void Start()
    {
        //InvokeRepeating("EnemyShoot", 2.0f, fireRate); // 2초 후 부터 fireRate 간격으로 탄막 생성
        StartCoroutine("ShootAlert");
        StartCoroutine("EnemyShoot");
        StartCoroutine("ShootDelay");
    }

    private void Update()
    {
        if(!alertState && !shootState && !cooldownState)
        {
            if ((GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).magnitude < 15f)
            {
                alertState = true;

                Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.Find("Alert").rotation = rotation;
                transform.Find("Alert").transform.Find("ShootAlert").gameObject.SetActive(true);
            }
        }
    }

    IEnumerator ShootAlert()
    {
        while(true)
        {
            yield return null;
            if(alertState)
            {
                alertCoolTime += Time.deltaTime;
                Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.Find("Alert").rotation = rotation;
                if (alertCoolTime > alertDelayTime)
                {
                    shootState = true;
                    alertCoolTime = 0f;
                    alertState = false;
                    transform.Find("Alert").transform.Find("ShootAlert").gameObject.SetActive(false);
                }
            }
        }
    }

    // 임의 EnemyShoot 클래스 생성
    IEnumerator EnemyShoot()
    {
        while(true)
        {
            yield return null;
            if(shootState)
            {
                cooldownState = true;
                enemyBulletSpawnPointTransform = gameObject.transform;
                Instantiate(enemyBulletPrefab, enemyBulletSpawnPointTransform.position, enemyBulletSpawnPointTransform.rotation); // 적 탄막 스폰 위치에서 적 탄막 생성
                shootState = false;
            }
        }
        
    }

    IEnumerator ShootDelay()
    {
        while(true)
        {
            yield return null;
            if(cooldownState)
            {
                shootCoolTime += Time.deltaTime;
                if (shootCoolTime > shootDelayTime)
                {
                    cooldownState = false;
                    shootCoolTime = 0f;
                }
            }
        }
    }
}
