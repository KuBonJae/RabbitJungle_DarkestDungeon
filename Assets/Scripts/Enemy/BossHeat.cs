using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BossHeat : MonoBehaviour
{
    public Slider HpBarSlider;
    public float bossHP;
    private float maxHP;

    public GameObject hpText;
    public GameObject canvas;
    public GameObject hpTextObject = null;
    //RectTransform hpRectTransform;
    public bool beDamaged = false;
    float height = 1.5f;
    //float deltaHeight = 0f;

    float resetForceCooltime = 1f;
    float resetForceDelay = 0f;

    public GameObject hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        bossHP = 75.0f * DataManager.Instance.StageLevel;
        maxHP = 75.0f * DataManager.Instance.StageLevel;

        canvas = GameObject.Find("Canvas");
        StartCoroutine("ShowDamageText");
    }

    // Update is called once per frame
    void Update()
    {
        CheckHp();
        if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            resetForceDelay += Time.deltaTime;
            if (resetForceDelay > resetForceCooltime)
            {
                resetForceDelay = 0f;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }


    public void CheckHp() //*HP 갱신
    {
        if (HpBarSlider != null)
            HpBarSlider.value = (bossHP / maxHP);
        Debug.Log("적 체력 : " + bossHP / maxHP);
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



        // different damage according to weapon type
        /*
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Skill"))
        {

            if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
            {
                Debug.Log("총 맞음!");

                bossHP -= 1;
            }
            else if (DataManager.Instance.Weapon == WeaponType.Sword.ToString())
            {
                Debug.Log("칼 맞음!");
                bossHP -= 3;
            }
            else
            {
                Debug.Log("총기 타입 없음!");
                bossHP -= 2;
            }

        }
        */
        /*
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Skill"))
        {
            if (collision.gameObject.CompareTag("Weapon"))
            {
                if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
                {
                    Debug.Log("총 맞음!");
                    switch (DataManager.Instance.Weapon)
                    {
                        case nameof(SpecialWeaponType.ShotGun):
                            Destroy(collision.gameObject);
                            bossHP -= 3;
                            Debug.LogWarning("샷건 맞음!");
                            break;

                        case nameof(SpecialWeaponType.Rifle):
                            Destroy(collision.gameObject);
                            Debug.LogWarning("라이플 맞음!");
                            bossHP -= 2;
                            break;

                        case nameof(SpecialWeaponType.Sniper):
                            Destroy(collision.gameObject);
                            Debug.LogWarning("스나이퍼 맞음!");
                            bossHP -= 5;
                            break;
                        default:
                            Destroy(collision.gameObject);
                            Debug.LogWarning("기본 총 맞음");
                            bossHP -= 1;
                            break;
                    }
                }
                else if (DataManager.Instance.Weapon == WeaponType.Sword.ToString())
                {
                    Debug.Log("칼 맞음!");
                    switch (DataManager.Instance.Weapon)
                    {
                        case nameof(SpecialWeaponType.ShortSword):
                            Destroy(collision.gameObject);
                            bossHP -= 2;
                            Debug.LogWarning("단검 맞음!");
                            break;

                        case nameof(SpecialWeaponType.LongSword):
                            Destroy(collision.gameObject);
                            Debug.LogWarning("대검 맞음!");
                            bossHP -= 5;
                            break;

                        case nameof(SpecialWeaponType.Axe):
                            Destroy(collision.gameObject);
                            Debug.LogWarning("도끼 맞음!");
                            bossHP -= 3;
                            break;
                        default:
                            Destroy(collision.gameObject);
                            Debug.LogWarning("기본 칼 맞음");
                            bossHP -= 2;
                            break;
                    }
                }
                else
                {
                    Debug.Log("총기 타입 없음!");
                    bossHP -= 2;
                }
            }
            else if (collision.gameObject.CompareTag("Skill"))
            {
                Debug.Log("스킬 맞음!");
                bossHP -= 5;
            }
            else
            {
                Debug.Log("넌 왜 닳는거..?");
                bossHP = -1;

            }
        }
        */
        //if(collision.gameObject.CompareTag("Shield"))
        //{
        //    Debug.Log("카운터!");
        //    bossHP -= DataManager.Instance.Damage;
        //    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
        //    beDamaged = true;d
        //}

        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Skill"))
        {
            if (collision.gameObject.CompareTag("Weapon"))
            {
                if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
                {
                    Debug.Log("총 맞음!");
                    bossHP -= DataManager.Instance.Damage;
                }
                else if (DataManager.Instance.Weapon == WeaponType.Sword.ToString())
                {
                    Debug.Log("칼 맞음!");
                    bossHP -= DataManager.Instance.Damage;
                }
                else
                {
                    Debug.Log("총기 타입 없음!");
                    bossHP -= DataManager.Instance.Damage;
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
                    bossHP -= DataManager.Instance.AxeDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.AxeDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.LongSword.ToString())
                {
                    Debug.Log("대검 스킬 맞음!");
                    bossHP -= DataManager.Instance.Damage * 5f;
                    hpText.GetComponent<TextMeshProUGUI>().text = (DataManager.Instance.Damage * 5f).ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShortSword.ToString())
                {
                    Debug.Log("단검 스킬 맞음!");
                    bossHP -= DataManager.Instance.ShurikenDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.ShurikenDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
                {
                    Debug.Log("단검 스킬 맞음!");
                    bossHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
                {
                    Debug.Log("단검 스킬 맞음!");
                    bossHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
                {
                    Debug.Log("단검 스킬 맞음!");
                    bossHP -= DataManager.Instance.SkillDamage;
                    hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.SkillDamage.ToString();
                }
                else
                {
                    Debug.Log("모르는 스킬 맞음!");
                    bossHP -= DataManager.Instance.Damage * 2.5f;
                    hpText.GetComponent<TextMeshProUGUI>().text = (DataManager.Instance.Damage * 2.5f).ToString();
                }
            }
            else
            {
                Debug.Log("넌 왜 닳는거..?");
                bossHP = DataManager.Instance.Damage;

            }
            beDamaged = true;
            Instantiate(hitParticle, transform.position, transform.rotation);
        }

        // when enemy heated, compare bossHP
        checkBosssHP();
    }
    public void checkBosssHP()
    {
        switch (bossHP)
        {
            case <= 0:
                Debug.Log("Die!");
                HpBarSlider.value = 0;
                HpBarSlider.transform.parent.gameObject.SetActive(false);
                Destroy(gameObject);

                break;

            /*case 1 :
            Debug.Log("적피 1");
            if (hp2Transform != null ) {
                Destroy(hp2Transform.gameObject);
            }
            if (hp3Transform != null ) {
                Destroy(hp3Transform.gameObject);
            }
            if (hp4Transform != null ) {
                Destroy(hp4Transform.gameObject);
            }
            if (hp5Transform != null ) {
                Destroy(hp5Transform.gameObject);
            }
            //Destroy(enemyHPTransform.gameObject.transform.Find("HP_2"));
            break;

            case 2 :
            Debug.Log("적피 2");
            if (hp3Transform != null ) {
                Destroy(hp3Transform.gameObject);
            }
            if (hp4Transform != null ) {
                Destroy(hp4Transform.gameObject);
            }
            if (hp5Transform != null ) {
                Destroy(hp5Transform.gameObject);
            }
            break;

            case 3 :
            Debug.Log("적피 3");
            if (hp4Transform != null ) {
                Destroy(hp4Transform.gameObject);
            }
            if (hp5Transform != null ) {
                Destroy(hp5Transform.gameObject);
            }
            break;

            case 4 :
            Debug.Log("적피 4");
            if (hp5Transform != null ) {
                Destroy(hp5Transform.gameObject);
            }
            break;
            */
            default:
                break;
        }
    }
    IEnumerator ShowDamageText()
    {
        while (true)
        {
            yield return null;
            if (beDamaged)
            {
                Vector3 hpTextPos =
                    Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));

                hpTextObject = Instantiate(hpText, canvas.transform);
                hpTextObject.GetComponent<MovingDamageText>().playerObject = this.gameObject;
                hpTextObject.GetComponent<RectTransform>().position = hpTextPos;
                hpTextObject = null;
                beDamaged = false;
            }
        }
    }

    //IEnumerator SlowMotionInCounter()
    //{
    //    float slowTime = 0f;
    //    while(true)
    //    {
    //        yield return null;
    //
    //        slowTime += Time.unscaledDeltaTime;
    //        if(slowTime > 0.5f)
    //        {
    //            Time.timeScale = 1f;
    //            break;
    //        }
    //    }
    //}
}

