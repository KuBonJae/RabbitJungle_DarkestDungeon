using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // 싱글턴 인스턴스
    public static DataManager Instance { get; private set; }

    // Private fields for user stats
    private int stageLevel;
    private float speed;
    private float attacSpeed;
    private float health;
    private float maxHealth;
    private float damage;
    private int dashCount;
    private bool dashState;
    private string weapon;
    private string specialWeapon;
    private int bulletCount;
    private float skillDamage;
    private float swordLength;
    private float shurikenDamage;
    private float axeDamage;
    public float firstDamage;
    public float firstMaxHealth;
    public float firstSpeed;
    public float firstAttackSpeed;
    public int firstDashCount;
    public float additionalDamage = 0;
    public float additionalMaxHealth = 0;
    public float additionalSpeed = 0;
    public float additionalAttackSpeed = 0;
    public int additionalDashCount = 0;
    public bool justCleared = false;
    public bool specialWeaponGet = false;
    public bool epicSkill = false;
    public bool isDead = false;
    public bool firstClassChage = true;
    public bool beHit = false;

    // getset 에 접근하게 해주는 프로퍼티
    public int StageLevel
    {
        get { return stageLevel; }
        set { stageLevel = value; }
    }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float AttacSpeed
    {
        get { return attacSpeed; }
        set { attacSpeed = value; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int DashCount
    {
        get { return dashCount; }
        set { dashCount = value; }
    }

    public bool DashState
    {
        get { return dashState; }
        set { dashState = value; }
    }

    public string Weapon
    {
        get { return weapon; }
        set { weapon = value; }
    }
    public string SpecialWeapon
    {
        get { return specialWeapon; }
        set { specialWeapon = value; }
    }
    public int BulletCount
    {
        get { return bulletCount; }
        set { bulletCount = value; }
    }
    public float SkillDamage
    {
        get { return skillDamage; }
        set { skillDamage = value; }
    }
    public float SwordLength
    {
        get { return swordLength; }
        set { swordLength = value; }
    }
    public float ShurikenDamage
    {
        get { return shurikenDamage; }
        set { shurikenDamage = value; }
    }
    public float AxeDamage
    {
        get { return axeDamage; }
        set { axeDamage = value; }
    }

    // Optional: Add any additional methods or functionality here

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
