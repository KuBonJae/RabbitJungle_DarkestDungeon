using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSetting : BasicHeroSetting
{
    public int minHealAmount = 4;
    public int maxHealAmount = 7;
    public HealerSetting()
    {
        heroMaxHP -= 5;
        heroHp -= 5;
        //heroStress = 0;
        heroMinDamage -= 1;
        heroMaxDamage -= 1;
        heroBasicSpeed -= 1;
        heroBasicDodgeRate -= 2;
        heroClass = ClassName.Healer;
        //heroBasicAccuracy = 100;
        //heroBasicCriticalHit = 5;
    }
}
