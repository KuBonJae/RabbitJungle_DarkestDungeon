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

    // 버프가 여러 전투동안 지속되려면 지속 턴 수도 추가해줘야 함
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