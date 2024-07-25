using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCount : MonoBehaviour
{
    public TMP_Text enemyCountText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Count enemies Based on EnemyHPbar
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyNumber");
        enemyCountText.text = "Enemies Left: " + enemies.Length;
    }
}
