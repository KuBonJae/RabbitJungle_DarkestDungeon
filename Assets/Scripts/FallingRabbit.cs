using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRabbit : MonoBehaviour
{
    public GameObject rabbits;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRabbits());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnRabbits()
    {
        while (true)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-8f, 8f), 10f, 0f);
            GameObject newRabbit = Instantiate(rabbits, randomPosition, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

            yield return new WaitForSeconds(1f);
        }
    }
}
