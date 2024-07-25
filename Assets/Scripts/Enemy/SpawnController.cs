using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnController : MonoBehaviour
{
    public int SpawnCount = 0;
    public GameObject shortEnemy;
    public GameObject LongEnemy;

    private DataManager DataManager;
    private int playerLayerMask;
    private bool visited = false;


    private Transform playerTransform;
    private int numberOfEnemy = 5; // 몬스터 생성 개수

    //맵 상에서 적 생성 반경
    private float minX = -20.0f;
    private float maxX = 20.0f;
    private float minY = -20.0f;
    private float maxY = 20.0f;
    private float minDistanceFromPlayer = 15.0f; // 플레이어와 최소 거리

    // Start is called before the first frame update
    void Start()
    {



    }

    //임의의 SpawnEnemy 메소드 선언
    public void SpawnEnemies()
    {
        SpawnCount++;
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player를 찾을 수 없어 몬스터를 스폰할 수 없습니다.");
            return;
        }
        Debug.Log("스폰 에너미");
        int spawnedEnemys = 0;
        int enemyNum = DataManager.Instance.StageLevel > 4? numberOfEnemy * 4 + (DataManager.Instance.StageLevel - 1) : numberOfEnemy * DataManager.Instance.StageLevel;
        while (spawnedEnemys < enemyNum)
        {

            // Select Enemy type Random
            GameObject enemyToSpawn = Random.Range(0, 2) == 0 ? shortEnemy : LongEnemy;

            // 몹 스폰 위치 선언
            Vector2 spawnPosition = new Vector2(transform.position.x + Random.Range(minX, maxX),
                                                transform.position.y + Random.Range(minY, maxY));

            //플레이어와의 거리 계산
            float distanceFromPlayer = Vector2.Distance(spawnPosition, playerTransform.position);


            // 플레이어와의 최소 거리보다 길다면 몹 생성. 지금은 단거리 몹만 생성하지만 나중에 중거리 몹과 섞는 로직 짜야할듯
            if (distanceFromPlayer > minDistanceFromPlayer)
            {
                Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity); // 기본 회전값으로 생성
                spawnedEnemys++;
            }
        }
    }
    private void Update()
    {
        if (visited)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyNumber");
            gameObject.transform.parent.GetComponent<RoomData>().RemainedEnemy = enemies.Length;

            if (SpawnCount >= 3 && gameObject.transform.parent.GetComponent<RoomData>().RemainedEnemy <= 0 && gameObject.transform.parent.GetComponent<RoomData>().RoomType == RoomType.Battle.ToString())
            {
                //set currentMap cleared
                Debug.Log("클리어!");
                gameObject.transform.parent.GetComponent<RoomData>().RoomType = RoomType.Cleared.ToString();
                DataManager.Instance.justCleared = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("콜리션 발동");
        if (other.gameObject.CompareTag("Player") && gameObject.transform.parent.GetComponent<RoomData>().RoomType.ToString() == RoomType.Battle.ToString())
        {
            visited = true;// spawnEnemies
            SpawnEnemies();
            Invoke("SpawnEnemies", 3.0f);// spawn once more
            Invoke("SpawnEnemies", 6.0f);// spawn twice more

        }
    }
}