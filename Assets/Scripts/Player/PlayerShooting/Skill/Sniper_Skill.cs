using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class Sniper_Skill : MonoBehaviour
{
    public GameObject bigBullet;
    public GameObject bulletEffect;
    public Transform spawnPos;

    public GameObject rotation;

    public float skillCoolTime;
    public float skillCool;
    private float coolTIme;


    public GameObject[] CoolDownUI;
    public TextMeshProUGUI skillCoolDownText;


    // Start is called before the first frame update
    void Start()
    {
        CoolDownUI = new GameObject[10];

        for (int i = 0; i < GameObject.Find("Battle_Ui").transform.Find("SkillCoolDown").transform.childCount; i++)
        {
            CoolDownUI[i] = GameObject.Find("Battle_Ui").transform.Find("SkillCoolDown").transform.GetChild(i).gameObject;
            if (CoolDownUI[i].name.Contains("Text"))
                skillCoolDownText = CoolDownUI[i].transform.GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.epicSkill)
        {
            if (ManagingInput.GetMouseButton(1) && skillCool > skillCoolTime)
            {
                Instantiate(bigBullet, spawnPos.position, rotation.transform.rotation);
                Instantiate(bulletEffect, spawnPos.position, rotation.transform.rotation);
                skillCool = 0;
                //for (int i = 0; i < CoolDownUI.Length; i++)
                //{
                //    if (CoolDownUI[i] != null)
                //    {
                //        CoolDownUI[i].SetActive(true);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
            }
            skillCool = skillCool + Time.deltaTime;


            if (skillCoolTime - skillCool > 0)
            {
                coolTIme = skillCoolTime - skillCool;
                skillCool = skillCool + Time.deltaTime;
                skillCoolDownText.text = coolTIme.ToString("0.0");
            }
            else if (skillCoolTime - skillCool < 0)
            {
                //for (int i = 0; i < CoolDownUI.Length; i++)
                //{
                //    if (CoolDownUI[i] != null)
                //    {
                //        CoolDownUI[i].SetActive(false);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                skillCoolDownText.text = "(MRB)\nSkill";
            }

            


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

}
