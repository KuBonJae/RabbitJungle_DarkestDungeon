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
public struct TempStat
{
    int tempDmg;
    int tempDodge;
    int tempProtect;
    int tempSpeed;
    int tempAccuracy;
    int tempCrit;
    int tempDD;

    // ������ ���� �������� ���ӵǷ��� ���� �� ���� �߰������ ��
    public TempStat(int dmg, int dodge, int prot, int spd, int acc, int crit, int dd)
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