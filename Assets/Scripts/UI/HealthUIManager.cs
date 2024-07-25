using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    public GameObject healthIconPrefeb;


    public Transform healthUiContainer;
    public float healthCount;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthUI(healthCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateHealthUI(float count)
    {
        foreach (Transform child in healthUiContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < healthCount; i++)
        {
            if(healthCount - i >= 1)
            {
                GameObject bulletIcon = Instantiate(healthIconPrefeb, healthUiContainer);
            }
            else
            {
                GameObject bulletIcon = Instantiate(healthIconPrefeb, healthUiContainer);
                bulletIcon.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
            }
        }

    }

    public void SethealthCount(float count)
    {
        healthCount = count;
        UpdateHealthUI(healthCount);
    }
}
