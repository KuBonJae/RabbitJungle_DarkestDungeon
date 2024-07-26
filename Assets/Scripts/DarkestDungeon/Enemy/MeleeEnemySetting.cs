using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemySetting : BasicEnemySetting
{
    public MeleeEnemySetting(int upgrade)
    {
        MaxHP += 5 + upgrade * 2;
        Hp += 5 + upgrade * 2;
        MinDamage += 1 + upgrade;
        MaxDamage += 1 + upgrade;
        BasicSpeed += 2;
        BasicDodgeRate += 5;
        enemyClass = EnemyClassName.Melee;
        BasicCriticalHit += 5;
    }
}
