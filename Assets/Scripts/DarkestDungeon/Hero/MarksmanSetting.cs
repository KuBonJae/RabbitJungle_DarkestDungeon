using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanSetting : BasicHeroSetting
{
    public MarksmanSetting()
    {
        heroMaxHP -= 3;
        heroHp -= 3;
        //heroStress = 0;
        heroMinDamage += 1;
        heroMaxDamage += 3;
        heroBasicSpeed -= 2;
        heroBasicDodgeRate -= 5;
        heroClass = "Marksman";
        heroBasicAccuracy = 100;
        heroBasicCriticalHit += 5;
    }
}
