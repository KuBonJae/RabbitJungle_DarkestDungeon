using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHeroSetting
{
    public bool isDead = false;
    public bool DeathChecked = false;
    public int heroMaxHP = 30;
    public int heroHp = 30;
    public int heroStress = 0;
    public int heroStressMax = 200;
    public int heroStressLimit = 100;
    public int heroMinDamage = 3;
    public int heroMaxDamage = 7;
    public int heroBasicSpeed = 5;
    public int heroBasicDodgeRate = 10;
    public int heroBasicAccuracy = 100;
    public int heroBasicCriticalHit = 5;
    public int heroBasicDeathDoor = 67;
    public int heroBasicProtection = 0;
    public int heroBuffRemain = 0;
    public ClassName heroClass = ClassName.Default;
    public Stress Stress = Stress.Default;
}
