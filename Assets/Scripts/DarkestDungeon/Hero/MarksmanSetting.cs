using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanSetting : BasicHeroSetting
{
    public MarksmanSetting()
    {
        heroMaxHP -= 4;
        heroHp -= 4;
        //heroStress = 0;
        heroMinDamage += 2;
        heroMaxDamage += 4;
        heroBasicSpeed -= 1;
        heroBasicDodgeRate -= 2;
        heroClass = ClassName.Marksman;
        heroBasicAccuracy = 100;
        heroBasicCriticalHit += 15;
    }
}
