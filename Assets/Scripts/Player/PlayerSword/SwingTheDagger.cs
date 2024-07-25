using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SwingTheDagger : MonoBehaviour
{
    public float swingSpeed = 500.0f;
    public GameObject swordObject;
    public GameObject yeolpacham;
    //GameObject powerSlash;
    float deltaAngle = 0;
    //float deltaTime = 0;
    bool skillReady = true;
    float coolTime = 0.5f;
    float lifespan = 2.0f;
    float swingAngle = 90.0f;

    Queue<GameObject> q = new Queue<GameObject>();

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
        if (ManagingInput.GetMouseButton(0) && DataManager.Instance.specialWeaponGet && DataManager.Instance.SpecialWeapon == "ShortSword") 
        {
            //if(DataManager.Instance.firstClassChage)
            {
                DataManager.Instance.firstMaxHealth = 4;
                DataManager.Instance.firstSpeed = 13f;
                DataManager.Instance.firstDamage = 1.5f;
                DataManager.Instance.firstDashCount = 3;
                DataManager.Instance.firstAttackSpeed = 800f;

                //DataManager.Instance.firstClassChage = false;
            }

            if(!swordObject.activeSelf)
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
        if (ManagingInput.GetMouseButtonDown(1) && /*(deltaTime == 0 || deltaTime >= coolTime)*/skillReady && DataManager.Instance.specialWeaponGet && DataManager.Instance.SpecialWeapon == "ShortSword")
        {
            //deltaTime = 0;
            skillReady = false;

            //for (int i = 0; i < CoolDownUI.Length; i++)
            //{
            //    if (CoolDownUI[i] == null)
            //        break;
            //    CoolDownUI[i].SetActive(true);
            //}

            StartCoroutine(PowerSlash(0f));
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
    //IEnumerator PowerSlash(float t)
    //{
    //    Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //
    //    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //
    //    GameObject powerSlash = Instantiate(yeolpacham, new Vector2(transform.position.x, transform.position.y) + direction.normalized * 2, rotation);
    //    q.Enqueue(powerSlash);
    //
    //    while (true)
    //    {
    //        yield return null;
    //        if (powerSlash.IsDestroyed())
    //        {
    //            q.Dequeue();
    //            //deltaTime = 0;
    //            break;
    //        }
    //        Vector3 dir3 = rotation.eulerAngles;
    //        Vector3 dir = new Vector3(Mathf.Cos(dir3.z * Mathf.Deg2Rad), Mathf.Sin(dir3.z * Mathf.Deg2Rad), 0);
    //        powerSlash.transform.position += dir.normalized * Time.deltaTime * 50;
    //        deltaTime += Time.deltaTime;
    //        t += Time.deltaTime;
    //
    //        
    //
    //        if(deltaTime >= coolTime /*&& CoolDownUI[0].activeSelf*/)
    //        {
    //            //for (int i = 0; i < CoolDownUI.Length; i++)
    //            //{
    //            //    if (CoolDownUI[i] == null)
    //            //        break;
    //            //    CoolDownUI[i].SetActive(false);
    //            //}
    //            deltaTime = 0;
    //            skillCoolDownText.text = "(MRB)\nSkill";
    //        }
    //        else
    //            skillCoolDownText.text = (coolTime - deltaTime).ToString("0.0");
    //
    //        if (t >= lifespan)
    //        {
    //            Destroy(q.Dequeue());
    //            break;
    //        }
    //    }
    //}

    IEnumerator PowerSlash(float t)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject powerSlash = Instantiate(yeolpacham, new Vector2(transform.position.x, transform.position.y) + direction.normalized * 2, rotation);
        //q.Enqueue(powerSlash);

        float checkCoolTime = 0f;
        bool skillAlreadyReady = false;

        while (true)
        {
            yield return null;
            //if (powerSlash.IsDestroyed())
            //{
            //    q.Dequeue();
            //    //deltaTime = 0;
            //    break;
            //}
            Vector3 dir3 = rotation.eulerAngles;
            Vector3 dir = new Vector3(Mathf.Cos(dir3.z * Mathf.Deg2Rad), Mathf.Sin(dir3.z * Mathf.Deg2Rad), 0);
            powerSlash.transform.position += dir.normalized * Time.deltaTime * 50;
            checkCoolTime += Time.deltaTime;
            t += Time.deltaTime;



            if (checkCoolTime >= coolTime && !skillAlreadyReady/*&& CoolDownUI[0].activeSelf*/)
            {
                //for (int i = 0; i < CoolDownUI.Length; i++)
                //{
                //    if (CoolDownUI[i] == null)
                //        break;
                //    CoolDownUI[i].SetActive(false);
                //}
                skillReady = true;
                skillAlreadyReady = true;
                skillCoolDownText.text = "(MRB)\nSkill";
            }
            if(!skillAlreadyReady)
                skillCoolDownText.text = (coolTime - checkCoolTime).ToString("0.0");

            if (t >= lifespan)
            {
                Destroy(powerSlash);
                break;
            }
        }
    }
}
