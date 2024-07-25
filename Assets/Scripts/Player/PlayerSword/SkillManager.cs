using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BOSS"))
        {

            // reduce collision owner's hp
            Destroy(collision.gameObject);
            // if your class is assassin, shuriken will be disappeared after collision
            if (this.gameObject.name.Contains("Shuriken"))
                Destroy(this.gameObject);

        }
    }*/
}
