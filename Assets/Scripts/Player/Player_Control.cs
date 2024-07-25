using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player_Control : MonoBehaviour
{
    // Player Movement
    public float horizontalInput;
    public float verticalInput;
    bool up = false;
    bool down = false;
    bool right = false;
    bool left = false;

    // Player UI
    public GameObject map;
    public GameObject keyGuide;
    public HealthUIManager healthUIManager;

    // Dash
    private bool dashState = false;
    private bool isDashing = false;
    private float dashDuration = 0.1f;
    private float dashTimer = 0f;
    private int dashCount;
    public float dashCoolTime = 0f;

    // Player Gun
    public GameObject[] playerGun;

    public float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        // Max Health / Health Setting 
        maxHealth = 4.0f;
        healthUIManager.SethealthCount(maxHealth);
        DataManager.Instance.Health = maxHealth;

        StartCoroutine("Flip");
        StartCoroutine("ChargeDash");
        StartCoroutine("HitDelay");

        DataManager.Instance.Speed = 10.0f;
        DataManager.Instance.DashCount = 2;

        dashCount = DataManager.Instance.DashCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Player Movement
        horizontalInput = ManagingInput.GetAxis("Horizontal");
        verticalInput = ManagingInput.GetAxis("Vertical");

        if(SceneManager.GetActiveScene().name == "BossScene")
        {
            if(GameObject.Find("Camera").transform.Find("BossCam").gameObject.activeSelf)
            {
                ManagingInput.blockInputs = true;
            }
            else
            {
                ManagingInput.blockInputs = false;
                UpdateData();
            }
        }
        else
        {
            ManagingInput.blockInputs = false;
            UpdateData();
        }
    }

    private void UpdateData()
    {
        Block();
        transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * DataManager.Instance.Speed);
        transform.Translate(Vector2.up * verticalInput * Time.deltaTime * DataManager.Instance.Speed);


        // Basic Movement Skill
        baseSkill();

        // Toogle Map
        toggleMap();

        // Special Weapon
        if (DataManager.Instance.specialWeaponGet)
        {
            if (DataManager.Instance.firstClassChage)
            {
                SpecialWeaponGet();
                DataManager.Instance.firstClassChage = false;
            }
        }

        WeaponChange();

        // Check Player Life
        PlayerDeath();
        healthUIManager.SethealthCount(DataManager.Instance.Health);
    }

    //Toogle Map 
    private void toggleMap()
    {
        if (ManagingInput.GetKeyDown(KeyCode.Tab))
        {
            map.SetActive(!map.activeSelf);
            keyGuide.SetActive(!keyGuide.activeSelf);
            if (map.activeSelf)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
    }

    // player move block 
    private void Block()
    {
        RaycastHit2D[] hitDownRight = Physics2D.RaycastAll(transform.position, (Vector2.down + Vector2.right).normalized);
        for (int i = 0; i < hitDownRight.Length; i++)
        {
            if (hitDownRight[i].transform != null)
            {
                if (hitDownRight[i].distance < 0.5 && hitDownRight[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        //horizontalInput = 0;
                        right = true;
                    }
                    if (verticalInput < 0)
                    {
                        //verticalInput = 0;
                        down = true;
                    }

                    if (down && right)
                    {
                        down = right = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        down = right = false;
                }
            }
        }

        RaycastHit2D[] hitDownLeft = Physics2D.RaycastAll(transform.position, (Vector2.down + Vector2.left).normalized);
        for (int i = 0; i < hitDownLeft.Length; i++)
        {
            if (hitDownLeft[i].transform != null)
            {
                if (hitDownLeft[i].distance < 0.5 && hitDownLeft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        //horizontalInput = 0;
                        left = true;
                    }
                    if (verticalInput < 0)
                    {
                        //verticalInput = 0;
                        down = true;
                    }

                    if (down && left)
                    {
                        down = left = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        down = left = false;
                }
            }
        }

        RaycastHit2D[] hitUpRight = Physics2D.RaycastAll(transform.position, (Vector2.up + Vector2.right).normalized);
        for (int i = 0; i < hitUpRight.Length; i++)
        {
            if (hitUpRight[i].transform != null)
            {
                if (hitUpRight[i].distance < 0.5 && hitUpRight[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        //horizontalInput = 0;
                        right = true;
                    }
                    if (verticalInput > 0)
                    {
                        //verticalInput = 0;
                        up = true;
                    }

                    if (up && right)
                    {
                        up = right = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        up = right = false;
                }
            }
        }

        RaycastHit2D[] hitUpLeft = Physics2D.RaycastAll(transform.position, (Vector2.up + Vector2.left).normalized);
        for (int i = 0; i < hitUpLeft.Length; i++)
        {
            if (hitUpLeft[i].transform != null)
            {
                if (hitUpLeft[i].distance < 0.5 && hitUpLeft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        //horizontalInput = 0;
                        left = true;
                    }
                    if (verticalInput > 0)
                    {
                        //verticalInput = 0;
                        up = true;
                    }

                    if (up && left)
                    {
                        up = left = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        up = left = false;
                }
            }
        }

        RaycastHit2D[] hitdown = Physics2D.RaycastAll(transform.position, Vector2.down);

        for (int i = 0; i < hitdown.Length; i++)
        {
            if (hitdown[i].transform != null)
            {
                if (hitdown[i].distance < 1 && hitdown[i].collider.CompareTag("Wall"))
                {
                    if (verticalInput < 0)
                    {
                        verticalInput = 0;
                        return;
                    }
                }

            }
        }

        RaycastHit2D[] hitup = Physics2D.RaycastAll(transform.position, Vector2.up);
        for (int i = 0; i < hitup.Length; i++)
        {
            if (hitup[i].transform != null)
            {
                if (hitup[i].distance < 1 && hitup[i].collider.CompareTag("Wall"))
                {
                    if (verticalInput > 0)
                    {
                        verticalInput = 0;
                        return;
                    }
                }
            }
        }

        RaycastHit2D[] hitleft = Physics2D.RaycastAll(transform.position, Vector2.left);
        for (int i = 0; i < hitleft.Length; i++)
        {
            if (hitleft[i].transform != null)
            {
                if (hitleft[i].distance < 0.5 && hitleft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        horizontalInput = 0;
                        return;
                    }
                }

            }
        }

        RaycastHit2D[] hitright = Physics2D.RaycastAll(transform.position, Vector2.right);
        for (int i = 0; i < hitright.Length; i++)
        {
            if (hitright[i].transform != null)
            {
                if (hitright[i].distance < 0.5 && hitright[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        horizontalInput = 0;
                        return;
                    }
                }
            }
        }
    }

    // player Death
    private void PlayerDeath()
    {
        if (DataManager.Instance.Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // player movement skill
    public void baseSkill()
    {
        if (ManagingInput.GetKeyDown(KeyCode.Space) && !isDashing && !DataManager.Instance.DashState)
        {
            BaseSkill();
            //DataManager.Instance.DashCount--;
        }
        else if (ManagingInput.GetKeyUp(KeyCode.Space))
        {
            DataManager.Instance.Speed = 10.0f;
        }

        if (isDashing)
        {
            Dash();
        }
    }

    // player movement skill
    private void BaseSkill()
    {
        if (dashCount > 0)
        {
            isDashing = true;
            DataManager.Instance.DashState = true;
            dashTimer = dashDuration;

            GameObject.Find("Canvas_Dash").transform.GetChild(dashCount).gameObject.SetActive(false);
            dashCount--;


            StartCoroutine("DashCutter");
        }
    }

    void Dash()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer > 0)
        {
            transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * 40);
            transform.Translate(Vector2.up * verticalInput * Time.deltaTime * 40);
        }
        else
        {
            isDashing = false;
            //DataManager.Instance.Speed = DataManager.Instance.Speed;
        }
    }


    public void WeaponChange()
    {
        if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
        {
            playerGun[0].SetActive(false);
            playerGun[1].SetActive(true);
            playerGun[2].SetActive(false);
            playerGun[3].SetActive(false);
        }
        else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
        {
            playerGun[0].SetActive(false);
            playerGun[1].SetActive(false);
            playerGun[2].SetActive(true);
            playerGun[3].SetActive(false);

        }
        else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
        {
            playerGun[0].SetActive(false);
            playerGun[1].SetActive(false);
            playerGun[2].SetActive(false);
            playerGun[3].SetActive(true);
        }
    }

    // player gun switch case
    public void SpecialWeaponGet()
    {
        if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
        {
            DataManager.Instance.firstMaxHealth = 4;
            DataManager.Instance.firstDashCount = 2;
            DataManager.Instance.firstSpeed = 10f;
            DataManager.Instance.firstDamage = 1;
            DataManager.Instance.firstAttackSpeed = 0.1f;
            DataManager.Instance.BulletCount = 50;
            DataManager.Instance.SkillDamage = 2.0f;
        }
        else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
        {
            DataManager.Instance.firstMaxHealth = 4;
            DataManager.Instance.firstDashCount = 2;
            DataManager.Instance.firstSpeed = 10f;
            DataManager.Instance.firstDamage = 1;
            DataManager.Instance.firstAttackSpeed = 1;
            DataManager.Instance.BulletCount = 10;
            DataManager.Instance.SkillDamage = 1.0f;

        }
        else if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
        {
            DataManager.Instance.firstMaxHealth = 4;
            DataManager.Instance.firstDashCount = 2;
            DataManager.Instance.firstSpeed = 10f;
            DataManager.Instance.firstDamage = 4;
            DataManager.Instance.firstAttackSpeed = 1;
            DataManager.Instance.BulletCount = 10;
            DataManager.Instance.SkillDamage = 5.0f;
        }

    }

    IEnumerator Flip()
    {
        while (true)
        {
            yield return null;
            if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontalInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }

    IEnumerator DashCutter()
    {
        DataManager.Instance.DashState = true;
        this.gameObject.layer = 10;
        yield return new WaitForSeconds(0.25f);
        DataManager.Instance.DashState = false;
        this.gameObject.layer = 0;
    }

    IEnumerator ChargeDash()
    {
        while (true)
        {
            yield return null;

            if (dashCount < DataManager.Instance.DashCount)
            {
                dashCoolTime += Time.deltaTime;
                if (dashCoolTime >= 3)
                {
                    dashCount++;
                    GameObject.Find("Canvas_Dash").transform.GetChild(dashCount).gameObject.SetActive(true);
                    dashCoolTime = 0;
                }
            }
            else
                continue;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BOSS") || collision.gameObject.CompareTag("EnemyWeapon"))
        {
            if (!DataManager.Instance.DashState)
            {
                if (!DataManager.Instance.beHit)
                {
                    DataManager.Instance.beHit = true;
                    Debug.Log("피해입음!");
                    this.gameObject.GetComponent<HitEffect>().TriggerHitEffect();
                    DataManager.Instance.Health = DataManager.Instance.Health - 0.5f;

                    if (DataManager.Instance.Health <= 0)
                    {
                        DataManager.Instance.beHit = false;
                        DataManager.Instance.isDead = true;
                    }
                }
            }
        }
    }

    IEnumerator HitDelay()
    {
        float hitDelay = 0f;

        while (true)
        {
            yield return null;

            if (DataManager.Instance.beHit)
            {
                hitDelay += Time.deltaTime;
                if (hitDelay >= 0.5f)
                {
                    hitDelay = 0f;
                    DataManager.Instance.beHit = false;
                }
            }
        }
    }
}
