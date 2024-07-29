using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHeroSetting : BasicHeroSetting
{
    public MeleeHeroSetting()
    {
        heroMaxHP += 5;
        heroHp += 5;
        //heroStress = 0;
        heroMinDamage += 1;
        heroMaxDamage += 1;
        heroBasicSpeed += 1;
        heroBasicDodgeRate += 2;
        heroClass = ClassName.Melee;
        heroBasicAccuracy = 100;
        heroBasicCriticalHit += 5;
    }
}
