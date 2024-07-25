using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalControl : MonoBehaviour
{
    private string gameScene = "Game";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("포탈 트리거 발동");
        if (other.gameObject.CompareTag("Player"))
        {
            DataManager.Instance.StageLevel++;
            SceneManager.LoadScene(gameScene);
        }
    }
}
