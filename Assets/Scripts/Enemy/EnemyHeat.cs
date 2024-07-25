using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHeat : MonoBehaviour
{
    public float enemyHP;

    public ParticleSystem deathEffectPrefab;

    float resetForceCooltime = 1f;
    float resetForceDelay = 0f;

    public GameObject hpText;
    public GameObject canvas;
    GameObject hpTextObject = null;
    //RectTransform hpRectTransform;
    public bool beDamaged = false;
    public float height = 1.5f;
    public float deltaHeight = 0f;

    public GameObject hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        enemyHP = 5.0f * DataManager.Instance.StageLevel;
        //hpRectTransform = hpTextObject.GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas");
        //StartCoroutine("ShowDamageText");
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            resetForceDelay += Time.deltaTime;
            if(resetForceDelay > resetForceCooltime)
            {
                resetForceDelay = 0f;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    // 충돌 시 
    private void OnTriggerEnter2D(Collider2D collision)
    {

        /* for special weapon
                switch (DataManager.Instance.Weapon)
        {
            case nameof(WeaponType.Sword):
                Destroy(collision.gameObject);
                enemyHP -= 1;
                break;

            case nameof(WeaponType.Axe):
                Destroy(collision.gameObject);
                enemyHP -= 2;
                break;

            case nameof(WeaponType.Bow):
                Destroy(collision.gameObject);
                enemyHP -= 1;
                break;

            // 다른 무기 유형들...

            default:
                // for no weapon
                Debug.LogWarning("알 수 없는 무기 유형입니다.");
                break;
        }
          */

        //if(collision.gameObject.CompareTag("Shield"))
        //{
        //    Debug.Log("카운터!");
        //    enemyHP -= DataManager.Instance.Damage;
        //    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
        //    beDamaged = true;
        //    Vector3 revDir = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
        //    GetComponent<Rigidbody2D>().AddForce(revDir.normalized * 1000f);
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Control_Sword>().shield.GetComponent<CircleCollider2D>().enabled = false;
        //    Time.timeScale = 0.1f;
        //    StartCoroutine("SlowMotionInCounter");
        //}

        // different damage according to weapon type ShortSword, LongSword, Axe, ShotGun, Rifle, Sniper
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Skill"))
        {
            Vector3 revDir = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
            GetComponent<Rigidbody2D>().AddForce(revDir.normalized * 500f);

            if (collision.gameObject.CompareTag("Weapon"))
            {
                if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
                {
                    Debug.Log("총 맞음!");
                    enemyHP -= DataManager.Instance.Damage;
                }
                else if (DataManager.Instance.Weapon == WeaponType.Sword.ToString())
                {
                    Debug.Log("칼 맞음!");
                    enemyHP -= DataManager.Instance.Damage;
                }
                else
                {
                    Debug.Log("총기 타입 없음!");
                    enemyHP -= DataManager.Instance.Damage;
                }

                hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
                beDamaged = true;
            }
            else if (collision.gameObject.CompareTag("Skill"))
            {
                Debug.Log("스킬 맞음!");
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Axe.ToString())
                {
                    Debug.Log("도끼 스킬 맞음!");
                    enemyHP -= DataManager.Instance.AxeDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.AxeDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.LongSword.ToString())
                {
                    Debug.Log("대검 스킬 맞음!");
                    enemyHP -= (DataManager.Instance.Damage * 5f);
                    hpText.GetComponent<TextMeshProUGUI>().text = (DataManager.Instance.Damage * 5f).ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShortSword.ToString())
                {
                    Debug.Log("단검 스킬 맞음!");
                    enemyHP -= DataManager.Instance.ShurikenDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.ShurikenDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
                {
                    Debug.Log("저격 스킬 맞음!");
                    enemyHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
                {
                    Debug.Log("샷건 스킬 맞음!");
                    enemyHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
                {
                    Debug.Log("라이플 스킬 맞음!");
                    enemyHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else
                {
                    Debug.Log("칼 기본 스킬 맞음!");
                    enemyHP -= (DataManager.Instance.Damage * 2.5f);
                    hpText.GetComponent<TextMeshProUGUI>().text = (DataManager.Instance.Damage * 2.5f).ToString();
                }

                beDamaged = true;
            }
            else
            {
                Debug.Log("넌 왜 닳는거..?");
                enemyHP = DataManager.Instance.Damage;

            }

            Instantiate(hitParticle, transform.position, transform.rotation);
        }

        checkEnemysHP();
    }

    public void checkEnemysHP()
    {
        Transform enemyHPTransform = gameObject.transform.Find("EnemyHP");
        Transform hp2Transform = enemyHPTransform.Find("HP_2");
        Transform hp3Transform = enemyHPTransform.Find("HP_3");
        Transform hp4Transform = enemyHPTransform.Find("HP_4");
        Transform hp5Transform = enemyHPTransform.Find("HP_5");
        switch (enemyHP)
        {
            case <= 0:
                Debug.Log("Die!");
                Destroy(gameObject);
                //Instantiate(deathEffectPrefab, transform.position, quaternion.identity); 사망 파티클
                return;

            case <= 1:
                Debug.Log("적피 1");
                if (hp2Transform != null)
                {
                    Destroy(hp2Transform.gameObject);
                }
                if (hp3Transform != null)
                {
                    Destroy(hp3Transform.gameObject);
                }
                if (hp4Transform != null)
                {
                    Destroy(hp4Transform.gameObject);
                }
                if (hp5Transform != null)
                {
                    Destroy(hp5Transform.gameObject);
                }
                //Destroy(enemyHPTransform.gameObject.transform.Find("HP_2"));
                break;

            case <= 2:
                Debug.Log("적피 2");
                if (hp3Transform != null)
                {
                    Destroy(hp3Transform.gameObject);
                }
                if (hp4Transform != null)
                {
                    Destroy(hp4Transform.gameObject);
                }
                if (hp5Transform != null)
                {
                    Destroy(hp5Transform.gameObject);
                }
                break;

            case <= 3:
                Debug.Log("적피 3");
                if (hp4Transform != null)
                {
                    Destroy(hp4Transform.gameObject);
                }
                if (hp5Transform != null)
                {
                    Destroy(hp5Transform.gameObject);
                }
                break;

            case <= 4:
                Debug.Log("적피 4");
                if (hp5Transform != null)
                {
                    Destroy(hp5Transform.gameObject);
                }
                break;

            default:
                break;
        }

        StartCoroutine("ShowDamageText");
    }

    IEnumerator ShowDamageText()
    {
        while (true)
        {
            yield return null;
            if(beDamaged)
            {
                Vector3 hpTextPos =
                    Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));

                hpTextObject = Instantiate(hpText, canvas.transform);
                hpTextObject.GetComponent<MovingDamageText>().playerObject = this.gameObject;
                hpTextObject.GetComponent<RectTransform>().position = hpTextPos;
                hpTextObject = null;
                beDamaged = false;

                break;
            }
        }
    }

    //IEnumerator SlowMotionInCounter()
    //{
    //    float slowTime = 0f;
    //    while (true)
    //    {
    //        yield return null;
    //
    //        slowTime += Time.unscaledDeltaTime;
    //        if (slowTime > 0.05f)
    //        {
    //            Time.timeScale = 1f;
    //            break;
    //        }
    //    }
    //}
}