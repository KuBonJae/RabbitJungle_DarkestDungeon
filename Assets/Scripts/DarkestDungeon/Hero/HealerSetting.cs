using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSetting : BasicHeroSetting
{
    public HealerSetting()
    {
        heroMaxHP -= 5;
        heroHp -= 5;
        //heroStress = 0;
        heroMinDamage -= 1;
        heroMaxDamage -= 1;
        heroBasicSpeed -= 2;
        heroBasicDodgeRate -= 5;
        heroClass = ClassName.Healer;
        //heroBasicAccuracy = 100;
        //heroBasicCriticalHit = 5;
    }
}
