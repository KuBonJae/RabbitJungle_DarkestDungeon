using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Effect : MonoBehaviour
{
    private float lifeTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
