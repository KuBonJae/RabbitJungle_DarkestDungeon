using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarksmanEnemySetting : BasicEnemySetting
{
    public MarksmanEnemySetting(int upgrade)
    {
        MaxHP += (-2 + upgrade * 2 - 1);
        Hp += (-2 + upgrade * 2 - 1);
        MinDamage += 1 + upgrade;
        MaxDamage += 2 + upgrade;
        BasicSpeed += -2;
        BasicDodgeRate += -5;
        enemyClass = EnemyClassName.Marksman;
        BasicCriticalHit += 5;
    }
}
