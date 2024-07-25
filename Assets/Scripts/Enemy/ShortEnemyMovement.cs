using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortEnemyMovement : MonoBehaviour
{
    private float moveSpeed = 7.0f;
    private Transform playerTransform;

    public GameObject AttackIndicator;
    public GameObject Knife;
    float indicatorDelay = 0.8f;
    float attackDelay = 2f;
    float swordDelay = 1f;
    float indicatorCooltime = 0f;
    float knifeShowTime = 0f;
    float swordDelayTime = 0f;
    bool indicate = false;
    bool knifeShow = false;
    bool delayState = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("AttackDelay");
        StartCoroutine("IndicatorDisappear");
        StartCoroutine("SwordDisappear");
    }

    // Update is called once per frame
    void Update()
    {
        // player 태그를 향해서 Enemy 오브젝트가 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

        if((playerTransform.position - transform.position).magnitude < 3f)
        {
            if(!indicate &&
                !knifeShow &&
                !delayState)
            {
                AttackIndicator.SetActive(true);
                indicate = true;
            }
        }
    }

    IEnumerator IndicatorDisappear()
    {
        while (true)
        {
            yield return null;
            if (indicate)
            {
                indicatorCooltime += Time.deltaTime;
                if (indicatorCooltime > indicatorDelay)
                {
                    AttackIndicator.SetActive(false);
                    indicatorCooltime = 0f;
                    knifeShow = true;
                    indicate = false;

                    Vector2 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    Knife.transform.rotation = rotation;

                    Knife.transform.Find("knife").gameObject.SetActive(true);
                }
            }
        }
    }
    IEnumerator SwordDisappear()
    {
        while (true)
        {
            yield return null;
            if (knifeShow)
            {
                knifeShowTime += Time.deltaTime;
                if (knifeShowTime > swordDelay)
                {
                    Knife.transform.Find("knife").gameObject.SetActive(false);
                    knifeShowTime = 0f;
                    delayState = true;
                    knifeShow = false;
                }
            }
        }
    }

    IEnumerator AttackDelay()
    {
        while(true)
        {
            yield return null;
            if (delayState)
            {
                swordDelayTime += Time.deltaTime;
                if (swordDelayTime > attackDelay)
                {
                    swordDelayTime = 0f;
                    delayState = false;
                }
            }
        }
    }
}