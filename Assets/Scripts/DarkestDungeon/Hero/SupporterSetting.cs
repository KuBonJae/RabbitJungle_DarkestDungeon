using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterSetting : BasicHeroSetting
{
    public int minStressDownAmount = 8;
    public int maxStressDownAmount = 16;
    public SupporterSetting()
    {
        //heroHp -= 3;
        //heroStress = 0;
        //heroMinDamage += 1;
        //heroMaxDamage += 3;
        //heroBasicSpeed -= 2;
        //heroBasicDodgeRate -= 5;
        heroClass = ClassName.Supporter;
        //heroBasicAccuracy = 100;
        //heroBasicCriticalHit = 5;
    }
}
