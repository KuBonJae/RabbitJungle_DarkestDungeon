using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Control : MonoBehaviour
{
    public float speed;
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void LateUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Weapon") && !collision.collider.gameObject.CompareTag("Skill"))
        {
            Destroy(gameObject);
        }
    }

    
}
