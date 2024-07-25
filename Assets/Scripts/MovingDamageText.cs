using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingDamageText : MonoBehaviour
{
    public GameObject playerObject;

    float deltaHeight = 0f;
    float height = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        //playerObject = GameObject.Find("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject.IsDestroyed())
            Destroy(gameObject);

        deltaHeight += Time.deltaTime;
        Vector3 hpTextPos =
            Camera.main.WorldToScreenPoint(new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + height + deltaHeight, 0));
        if (deltaHeight > 1f)
            Destroy(gameObject);
        GetComponent<RectTransform>().position = hpTextPos;
    }   
}
