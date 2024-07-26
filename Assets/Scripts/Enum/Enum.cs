public enum WeaponType
{
    Sword, Gun
}
public enum SpecialWeaponType
{
    ShortSword, LongSword, Axe, ShotGun, Rifle, Sniper
}
public enum RoomType
{
    Battle, Boss, Item, Cleared, None
}
public enum userState
{
    moving , stop, attaking
}

public enum ClassName
{
    Melee, Marksman, Tanker, Healer, Supporter, Default
}

public enum EnemyClassName
{
    Melee, Marksman, Debuffer, Named, Default
}

public enum Stress
{
    Positive, Negative, Default
}

public class Stat
{
    public int tempDmg;
    public int tempDodge;
    public int tempProtect;
    public int tempSpeed;
    public int tempAccuracy;
    public int tempCrit;
    public int tempDD;

    // ������ ���� �������� ���ӵǷ��� ���� �� ���� �߰������ ��
    public Stat(int dmg, int dodge, int prot, int spd, int acc, int crit, int dd)
    {
        tempDmg = dmg;
        tempDodge = dodge;
        tempProtect = prot;
        tempSpeed = spd;
        tempAccuracy = acc;
        tempCrit = crit;
        tempDD = dd;
    }
}

