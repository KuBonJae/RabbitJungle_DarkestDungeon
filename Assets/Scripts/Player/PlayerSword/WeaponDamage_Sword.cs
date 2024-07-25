using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage_Sword : MonoBehaviour
{
    float swordSize;
    // Start is called before the first frame update
    void Start()
    {
        swordSize = DataManager.Instance.SwordLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.SpecialWeapon == "LongSword")
        {
            if (swordSize != DataManager.Instance.SwordLength)
            {
                float swordRatio = 1f;
                transform.Translate(new Vector3(0.5f, 0.5f, 0f));
                Transform parent = gameObject.transform.parent;
                gameObject.transform.parent = null;
                gameObject.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + swordRatio, 1);
                gameObject.transform.parent = parent;
                swordSize = DataManager.Instance.SwordLength;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyWeapon"))// || collision.gameObject.CompareTag("Boss"))
        {
            // reduce enemy's hp or kill them
            Destroy(collision.gameObject);
        }
    }
}
