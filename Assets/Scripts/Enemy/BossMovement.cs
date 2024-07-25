using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public GameObject bossShootBulletPrefab;
    public GameObject bossAroundBulletPrefab;
    private float moveSpeed = 4.0f;
    private float runspeed = 10.0f;
    private Transform playerTransform;

    private float bossShootFireRate = 0.5f;
    private float bossAroundFireRate = 1.0f;

    private enum BossState
    {
        Move,
        Rest,
        Run,
    }
    private enum BossShootState
    {
        Shoot,
        Stop,
        Around,
    }

    private BossState currentState;
    private BossShootState currentShootState;
    void Awake()
    {
        //StartCoroutine(StopGameForSeconds());
    }
    IEnumerator StopGameForSeconds()
    {
        Time.timeScale = 0;
        yield return new WaitForSeconds(3);
        Time.timeScale = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = BossState.Rest; // 처음 3초는 Rest
        StartCoroutine(StateControl());
        StartCoroutine(BossShootControl());
    }

    // Update is called once per frame
    /*void Update()
    {
        switch (currentState)
        {
            case BossState.Basic:
                break;
            case BossState.Shoot:
                BossShoot();
                break;
            case BossState.Move:
                BossMove();
                break;
            case BossState.Run:
                BossRun();
                break;
            case BossState.AroundShoot:
                BossAroundShoot();
                break;
        }
    }*/

    void Update()
    {
        switch (currentState)
        {
            case BossState.Move:
                BossMove();
                break;
            case BossState.Run:
                BossRun();
                break;
            case BossState.Rest:
                break;
        }
    }

    void BossMove()
    {
        // player 태그를 향해서 Enemy 오브젝트가 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }
    void BossRun()
    {
        // player 태그를 향해서 Enemy 오브젝트가 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, runspeed * Time.deltaTime);
    }

    void ShootBullet()
    {
        Instantiate(bossShootBulletPrefab, gameObject.transform.position, gameObject.transform.rotation); // 적 탄막 스폰 위치에서 적 탄막 생성
    }
    /*void AroundBullet()
    {
        int bulletCount = 10;
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;
            GameObject bullet = Instantiate(bossAroundBulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BossAroundBullet>().SetBulletProperties(bulDir);
            Instantiate(bossAroundBulletPrefab, gameObject.transform.position, gameObject.transform.rotation);// 적 탄막 스폰 위치에서 적 탄막 생성

            angle += angleStep;
        }
    }
    */

    IEnumerator BossShootControl()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            CancelInvoke();
            BossShootState newState = (BossShootState)Random.Range(0, System.Enum.GetValues(typeof(BossShootState)).Length);
            currentShootState = newState;
            Debug.Log("슛 상태 변경!");
            switch (newState)
            {
                case BossShootState.Shoot:
                    InvokeRepeating("ShootBullet", 0f, bossShootFireRate);
                    break;
                case BossShootState.Around:
                    InvokeRepeating("ShootBullet", 0f, bossShootFireRate);
                    //InvokeRepeating("AroundBullet", 0f, bossAroundFireRate);
                    break;
                case BossShootState.Stop:
                    break;
            }
        }
    }

    IEnumerator StateControl()
    {
        //yield return new WaitForSeconds(3f); // Initial 3-second delay
        while (true)
        {
            yield return new WaitForSeconds(3f);
            // Randomly select a new state
            BossState newState = (BossState)Random.Range(0, System.Enum.GetValues(typeof(BossState)).Length);
            // Set the current state to the new random state
            currentState = newState;

            // Cancel any ongoing invokes when changing state

        }
    }

}