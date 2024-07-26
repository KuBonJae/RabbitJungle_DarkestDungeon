using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebufferEnemySetting : BasicEnemySetting
{
    public int Skill1Rate = 75;
    public int Skill2Rate = 25;

    public DebufferEnemySetting(int upgrade)
    {
        MaxHP += (-2 + upgrade * 2 - 1);
        Hp += (-2 + upgrade * 2 - 1);
        MinDamage += (-1 + upgrade);
        MaxDamage += (-2 + upgrade);
        BasicSpeed += 2;
        BasicDodgeRate -= 5;
        enemyClass = EnemyClassName.Debuffer;
        //BasicCriticalHit += 5;
    }
}
