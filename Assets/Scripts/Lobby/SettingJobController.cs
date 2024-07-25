using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingJobController : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject selectjobCanvas;
    [SerializeField] private CinemachineVirtualCamera mainCamera;

    public GameObject HeroName1, HeroName2, HeroName3, HeroName4;
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.PartyFormation = new BasicHeroSetting[4] { null, null, null, null };
        DataManager.Instance.ChangeFormation = new BasicHeroSetting[4] { null, null, null, null };
        DataManager.Instance.HeroNum = 0;

        DataManager.Instance.firstRoom = true;
        DataManager.Instance.itemEvent = false;
        DataManager.Instance.coin = 0;

        DataManager.Instance.tempStats_P1 = new List<TempStat>();
        DataManager.Instance.tempStats_P2 = new List<TempStat>();
        DataManager.Instance.tempStats_P3 = new List<TempStat>();
        DataManager.Instance.tempStats_P4 = new List<TempStat>();
        DataManager.Instance.tempStats_Switch = new List<TempStat>();
}

    // Update is called once per frame
    void Update()
    {
        if (selectjobCanvas.activeSelf) // DataManager.Instance.PartyFormation[i].heroClass;
        {
            if (DataManager.Instance.PartyFormation[0] != null)
                HeroName1.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.PartyFormation[0].heroClass;
            else
                HeroName1.GetComponent<TextMeshProUGUI>().text = "";

            if (DataManager.Instance.PartyFormation[1] != null)
                HeroName2.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.PartyFormation[1].heroClass;
            else
                HeroName2.GetComponent<TextMeshProUGUI>().text = "";

            if (DataManager.Instance.PartyFormation[2] != null)
                HeroName3.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.PartyFormation[2].heroClass;
            else
                HeroName3.GetComponent<TextMeshProUGUI>().text = "";

            if (DataManager.Instance.PartyFormation[3] != null)
                HeroName4.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.PartyFormation[3].heroClass;
            else
                HeroName4.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void GameStart()
    {
        mainCamera.GetComponent<CinemachineVirtualCamera>().enabled = false;
        mainCanvas.SetActive(false);
        selectjobCanvas.SetActive(true);
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    public void SelectGun()
    {
        DataManager.Instance.StageLevel = 1;
        DataManager.Instance.MaxHealth = 4f;
        DataManager.Instance.Health = DataManager.Instance.MaxHealth;
        DataManager.Instance.Speed = 10f;
        DataManager.Instance.Damage = 2f;
        DataManager.Instance.DashCount = 2;
        DataManager.Instance.DashState = false;
        DataManager.Instance.AttacSpeed = 0.25f;
        DataManager.Instance.Weapon = WeaponType.Gun.ToString();
        DataManager.Instance.SpecialWeapon = null;
        DataManager.Instance.BulletCount = 20;

        DataManager.Instance.firstDamage = DataManager.Instance.Damage;
        DataManager.Instance.firstMaxHealth = DataManager.Instance.MaxHealth;
        DataManager.Instance.firstSpeed = DataManager.Instance.Speed;
        DataManager.Instance.firstDashCount = DataManager.Instance.DashCount;
        DataManager.Instance.firstAttackSpeed = DataManager.Instance.AttacSpeed;

        DataManager.Instance.additionalDamage = 0;
        DataManager.Instance.additionalMaxHealth = 0;
        DataManager.Instance.additionalSpeed = 0;
        DataManager.Instance.additionalDashCount = 0;
        DataManager.Instance.additionalAttackSpeed = 0;

        DataManager.Instance.specialWeaponGet = false;

        SceneManager.LoadScene("Game");
    }
    public void SelectSword()
    {
        DataManager.Instance.StageLevel = 1;
        DataManager.Instance.MaxHealth = 5f;
        DataManager.Instance.Health = DataManager.Instance.MaxHealth;
        DataManager.Instance.Speed = 10f;
        DataManager.Instance.Damage = 2f;
        DataManager.Instance.DashCount = 3;
        DataManager.Instance.DashState = false;
        DataManager.Instance.AttacSpeed = 500f;
        DataManager.Instance.Weapon = WeaponType.Sword.ToString();
        DataManager.Instance.SpecialWeapon = null;
        DataManager.Instance.SwordLength = 2f;
        DataManager.Instance.AxeDamage = 5f;
        DataManager.Instance.ShurikenDamage = 3f;

        DataManager.Instance.firstDamage = DataManager.Instance.Damage;
        DataManager.Instance.firstMaxHealth = DataManager.Instance.MaxHealth;
        DataManager.Instance.firstSpeed = DataManager.Instance.Speed;
        DataManager.Instance.firstDashCount = DataManager.Instance.DashCount;
        DataManager.Instance.firstAttackSpeed = DataManager.Instance.AttacSpeed;

        DataManager.Instance.additionalDamage = 0;
        DataManager.Instance.additionalMaxHealth = 0;
        DataManager.Instance.additionalSpeed = 0;
        DataManager.Instance.additionalDashCount = 0;
        DataManager.Instance.additionalAttackSpeed = 0;

        DataManager.Instance.specialWeaponGet = false;

        SceneManager.LoadScene("Game");
    }

    // Darkest Dungeon
    public void SelectMarksman()
    {
        BasicHeroSetting marksMan = new MarksmanSetting();
        DataManager.Instance.PartyFormation[DataManager.Instance.HeroNum] = marksMan;
        if (DataManager.Instance.HeroNum == 3)
            SceneManager.LoadScene("Game");
        else
            DataManager.Instance.HeroNum++;
    }

    public void SelectMelee()
    {
        BasicHeroSetting melee = new MeleeHeroSetting();
        DataManager.Instance.PartyFormation[DataManager.Instance.HeroNum] = melee;
        if (DataManager.Instance.HeroNum == 3)
            SceneManager.LoadScene("Game");
        else
            DataManager.Instance.HeroNum++;
    }
    
    public void SelectTanker()
    {
        BasicHeroSetting tanker = new TankerSetting();
        DataManager.Instance.PartyFormation[DataManager.Instance.HeroNum] = tanker;
        if (DataManager.Instance.HeroNum == 3)
            SceneManager.LoadScene("Game");
        else
            DataManager.Instance.HeroNum++;
    }

    public void SelectSupporter()
    {
        BasicHeroSetting supporter = new SupporterSetting();
        DataManager.Instance.PartyFormation[DataManager.Instance.HeroNum] = supporter;
        if (DataManager.Instance.HeroNum == 3)
            SceneManager.LoadScene("Game");
        else
            DataManager.Instance.HeroNum++;
    }

    public void SelectHealer()
    {
        BasicHeroSetting healer = new HealerSetting();
        DataManager.Instance.PartyFormation[DataManager.Instance.HeroNum] = healer;
        if (DataManager.Instance.HeroNum == 3)
            SceneManager.LoadScene("Game");
        else
            DataManager.Instance.HeroNum++;
    }

    public void ResetParty()
    {
        for(int i=0; i<4;i++)
        {
            DataManager.Instance.PartyFormation[i] = null;
        }
        DataManager.Instance.HeroNum = 0;
    }
    //
}
