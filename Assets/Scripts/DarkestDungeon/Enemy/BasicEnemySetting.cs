using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemySetting
{
    public bool isDead = false;
    public int MaxHP = 15;
    public int Hp = 15;
    public int MinDamage = 23;
    public int MaxDamage = 27;
    public int BasicSpeed = 5;
    public int BasicDodgeRate = 10;
    public int BasicAccuracy = 100;
    public int BasicCriticalHit = 5;
    public EnemyClassName enemyClass = EnemyClassName.Default;
}
