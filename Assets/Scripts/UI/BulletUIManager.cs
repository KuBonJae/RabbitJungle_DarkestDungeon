using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BulletUIManager : MonoBehaviour
{
    public GameObject bulletIconPrefeb;
    public Transform bullitUiContainer;
    public int bulletCount;


    // Start is called before the first frame update
    void Start()
    {
        UpdateBulletUI(bulletCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBulletUI(int count)
    {
        foreach (Transform child in bullitUiContainer) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject bulletIcon = Instantiate(bulletIconPrefeb, bullitUiContainer);
        }
    }

    public void SetBulletCount(int count)
    {
        bulletCount = count;
        UpdateBulletUI(bulletCount);
    }
}
