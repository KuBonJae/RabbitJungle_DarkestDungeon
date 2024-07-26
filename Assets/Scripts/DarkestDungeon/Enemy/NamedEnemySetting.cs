using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedEnemySetting : BasicEnemySetting
{
    public NamedEnemySetting(int upgrade)
    {
        MaxHP += 15 + upgrade * 3;
        Hp += 15 + upgrade * 3;
        MinDamage += 2 + upgrade;
        MaxDamage += 4 + upgrade;
        BasicSpeed += 0;
        BasicDodgeRate -= 10;
        enemyClass = EnemyClassName.Named;
        BasicCriticalHit += 10;
    }
}
