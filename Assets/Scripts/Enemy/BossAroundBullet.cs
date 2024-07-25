using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAroundBullet : MonoBehaviour
{
    /*
    private float bulletSpeed = 10f;
    private Vector2 bulletDirection;

    public void SetBulletProperties(Vector2 direction)
    {
        bulletDirection = direction.normalized;
    }

    // Start is called before the first frame update
    void Start(Vector2 direction)
    {

        //받아온 buleet speed 와 bulletDirection을 기반으로 속도와 방향 설정
        bulletDirection = direction.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(bulletDirection * bulletSpeed * Time.deltaTime);
    }



    // 탄막이 다른 콜라이더와 충돌 시 OnTriggerEnter2D 클래스 발생
    void OnColliderEnter2D(Collider2D other)
    {
        // 탄막이 다른 콜라이더(other)와 충돌했을때 플레이어 태그인지 비교
        // and compare with Wall Tag
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject); // 탄막 제거
        }
    }
*/
}
