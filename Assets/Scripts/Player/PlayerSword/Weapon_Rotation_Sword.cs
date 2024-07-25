using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Rotation_Sword : MonoBehaviour
{
    public float horizontalInput;
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Flip");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
        
    }

    IEnumerator Flip()
    {
        while (true)
        {
            horizontalInput = ManagingInput.GetAxis("Horizontal");

            yield return null;
            if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontalInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }
}
