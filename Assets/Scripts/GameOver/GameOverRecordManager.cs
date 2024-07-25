using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameOverRecordManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetComponent<TextMeshProUGUI>().text = "RECORD : <color=\"red\">STAGE " + DataManager.Instance.StageLevel + " </color>\n";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
