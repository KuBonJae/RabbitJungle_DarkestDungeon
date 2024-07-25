using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.isDead)
        {
            DataManager.Instance.isDead = false;
            SceneManager.LoadScene("GameOver");
        }
    }
}
