using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using TMPro;
using UnityEngine.UI;


public class Rifle_Skill : MonoBehaviour
{
    public GameObject rifle;

    public float skillTime;
    public float skillCool;
    public float skillCoolTime;
    private float coolTIme;

    private bool skill;


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
            {
                skillCoolDownText = CoolDownUI[i].transform.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.epicSkill)
        {
            if (!skill && skillCool > skillCoolTime)
            {
                if (ManagingInput.GetMouseButton(1))
                {
                    skill = true;
                    rifle.GetComponent<Gun_Rifle>().skill = skill;
                }
            }
            else if (skill)
            {
                skillTime = skillTime + Time.deltaTime;

                if (skillTime > 3)
                {
                    skill = false;
                    skillTime = 0;
                    rifle.GetComponent<Gun_Rifle>().skill = skill;
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
            }

            if(skillCoolTime - skillCool > 0)
            {
                coolTIme = skillCoolTime - skillCool;
                skillCool += Time.deltaTime;
                skillCoolDownText.text = coolTIme.ToString("0.0");
            }
            else if(skillCoolTime - skillCool < 0)
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
}

