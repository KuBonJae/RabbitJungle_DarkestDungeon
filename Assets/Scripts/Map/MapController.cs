using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{

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
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
