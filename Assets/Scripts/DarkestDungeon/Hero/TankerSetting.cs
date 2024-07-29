using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankerSetting : BasicHeroSetting
{
    public TankerSetting()
    {
        heroMaxHP += 10;
        heroHp += 10;
        //heroStress = 0;
        //heroMinDamage += 1;
        heroMaxDamage += 1;
        heroBasicSpeed -= 4;
        heroBasicDodgeRate -= 8;
        heroClass = ClassName.Tanker;
        heroBasicProtection += 20;
        //heroBasicAccuracy = 100;
        //heroBasicCriticalHit = 5;
    }
}
