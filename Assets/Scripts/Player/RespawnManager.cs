using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private GameObject player;
    void Start()
    {

    }
    public void Respawn()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        player.transform.position = new Vector3(0, 0, 0);
    }
}
