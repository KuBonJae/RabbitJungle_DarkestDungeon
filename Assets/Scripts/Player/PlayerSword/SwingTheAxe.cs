using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SwingTheAxe : MonoBehaviour
{
    public float swingSpeed = 500.0f;
    public GameObject swordObject;
    public GameObject yeolpacham;
    GameObject powerSlash;
    float deltaAngle = 0;
    float deltaTime = 0;
    float coolTime = 2.5f;
    float lifespan = 2.0f;
    float swingAngle = 90.0f;
    bool firstClassChange = false;

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
                skillCoolDownText.text = "(MRB)\nSkill";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ManagingInput.GetMouseButton(0) && DataManager.Instance.specialWeaponGet && DataManager.Instance.SpecialWeapon == "Axe") 
        {
            //if (DataManager.Instance.firstClassChage)
            {
                DataManager.Instance.firstMaxHealth = 5;
                DataManager.Instance.firstSpeed = 10f;
                DataManager.Instance.firstDamage = 2f;
                DataManager.Instance.firstAttackSpeed = 500f;
                DataManager.Instance.firstDashCount = 2;

                //DataManager.Instance.firstClassChage = false;
            }

            if (!swordObject.activeSelf)
            {
                swordObject.SetActive(true);

                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = rotation;

                deltaAngle = 0;

                StartCoroutine("Swing");
            }
        }
        if (ManagingInput.GetMouseButtonDown(1) && (deltaTime == 0 || deltaTime >= coolTime) && DataManager.Instance.specialWeaponGet && DataManager.Instance.SpecialWeapon == "Axe")
        {
            deltaTime = 0;

            //for (int i = 0; i < CoolDownUI.Length; i++)
            //{
            //    if (CoolDownUI[i] == null)
            //        break;
            //    CoolDownUI[i].SetActive(true);
            //}

            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            powerSlash = Instantiate(yeolpacham, new Vector2(transform.position.x, transform.position.y) + direction.normalized * 2, rotation);

            StartCoroutine(PowerSlash(rotation));
        }
    }

    IEnumerator Swing()
    {
        while (deltaAngle < swingAngle)
        {
            yield return null;

            float delta = (DataManager.Instance.firstAttackSpeed + DataManager.Instance.additionalAttackSpeed) * Time.deltaTime;

            transform.Rotate(-Vector3.forward * delta);
            deltaAngle += delta;

            if (deltaAngle > swingAngle)
            {
                swordObject.SetActive(false);
            }
        }
    }
    IEnumerator PowerSlash(Quaternion rot)
    {
        while (true)
        {
            yield return null;

            deltaTime += Time.deltaTime;

            skillCoolDownText.text = (coolTime - deltaTime).ToString("0.0");

            if (!powerSlash.IsDestroyed())
            {
                Vector3 dir3 = rot.eulerAngles;
                Vector3 dir = new Vector3(Mathf.Cos(dir3.z * Mathf.Deg2Rad), Mathf.Sin(dir3.z * Mathf.Deg2Rad), 0);
                powerSlash.transform.position += dir.normalized * Time.deltaTime * 50;
                powerSlash.transform.Rotate(Vector3.forward * 1000 * Time.deltaTime);
                if (deltaTime >= lifespan)
                    Destroy(powerSlash);
            }

            if (deltaTime >= coolTime)
            {
                //for (int i = 0; i < CoolDownUI.Length; i++)
                //{
                //    if (CoolDownUI[i] == null)
                //        break;
                //    CoolDownUI[i].SetActive(false);
                //}
                skillCoolDownText.text = "(MRB)\nSkill";
                break;
            }
        }

        deltaTime = 0;
    }
}
