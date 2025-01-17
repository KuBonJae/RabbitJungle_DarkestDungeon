using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public GameObject BattleManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.GetComponent<RoomData>().RoomType == RoomType.Cleared.ToString() || transform.parent.GetComponent<RoomData>().RoomType == RoomType.Item.ToString())
        {
            for (int i = 1; i < 5; i++)
            {
                transform.parent.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
        }
        if (transform.parent.GetComponent<RoomData>().RoomType == RoomType.Cleared.ToString())
        {
            GameObject wallUI = transform.parent.Find("MinimapUI").gameObject;
            for (int i = 0; i < 4; i++)
            {
                wallUI.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.CompareTag("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }*/
        // Darkest Dungeon -> Player가 콜라이더에 부딪히면, 이벤트 씬 or 전투 진행
        if (collision.gameObject.CompareTag("Player"))
        {
            if (transform.parent.GetComponent<RoomData>().RoomType == RoomType.Cleared.ToString())
                return;

            transform.parent.GetComponent<RoomData>().RoomType = RoomType.Cleared.ToString();

            int randomNum = Random.Range(1, 101);
            if(randomNum < DataManager.Instance.itemEngageRate) // 확률로 Item event
            {
                DataManager.Instance.itemEvent = true;
                if (DataManager.Instance.itemEngageRate > DataManager.Instance.battleEngageRate)
                    DataManager.Instance.itemEngageRate = DataManager.Instance.battleEngageRate = 50;
                else
                {
                    DataManager.Instance.itemEngageRate -= 5;
                    DataManager.Instance.battleEngageRate += 5;
                }
            }
            else // 확률로 전투
            {
                DataManager.Instance.battleEvent = true;
                if (DataManager.Instance.itemEngageRate < DataManager.Instance.battleEngageRate)
                    DataManager.Instance.itemEngageRate = DataManager.Instance.battleEngageRate = 50;
                else
                {
                    DataManager.Instance.itemEngageRate += 5;
                    DataManager.Instance.battleEngageRate -= 5;
                }
            }
        }
    }
}
