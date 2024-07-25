using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float bulletSpeed = 25.0f;

    private Transform playerTransform;

    public Vector2 bulletDirection;

    // Start is called before the first frame update
    void Start()
    {
        //플레이어 위치 인식
        playerTransform = GameObject.FindWithTag("Player").transform;
        //플레이어 위치와 몬스터 위치 기반으로 방향 설정
        bulletDirection = (playerTransform.position - transform.position).normalized;
    }

    void Update()
    {
        // update bullet transform position by delta time
        //transform.position += (Vector3)(bulletDirection * bulletSpeed * Time.deltaTime);
        transform.Translate((Vector3)(bulletDirection * bulletSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.gameObject.CompareTag("Shield"))
        //{
        //    bulletDirection *= -1;
        //    this.gameObject.tag = "Weapon";
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Control_Sword>().shield.GetComponent<CircleCollider2D>().enabled = false;
        //    Time.timeScale = 0.1f;
        //    StartCoroutine("SlowMotionInCounter");
        //}
    }

    // 탄막이 다른 콜라이더와 충돌 시 OnTriggerEnter2D 클래스 발생
    void OnTriggerStay2D(Collider2D other)
    {
        // 탄막이 다른 콜라이더(other)와 충돌했을때 플레이어 태그인지 비교
        // and compare with Wall Tag
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Wall") || other.CompareTag("Weapon"))
        {
            Destroy(gameObject); // 탄막 제거
        }
    }

    //IEnumerator SlowMotionInCounter()
    //{
    //    float slowTime = 0f;
    //    while (true)
    //    {
    //        yield return null;
    //
    //        slowTime += Time.unscaledDeltaTime;
    //        if (slowTime > 0.05f)
    //        {
    //            Time.timeScale = 1f;
    //            break;
    //        }
    //    }
    //}
}
