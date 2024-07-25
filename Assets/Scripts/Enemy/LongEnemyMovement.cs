using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongEnemyMovement : MonoBehaviour
{
    private float moveSpeed = 3.0f;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // player 태그를 향해서 Enemy 오브젝트가 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }
}