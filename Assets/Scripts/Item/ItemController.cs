using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] private GameObject normalCard;
    [SerializeField] private GameObject epicCard;
    [SerializeField] private GameObject CardSelectUI;
    [SerializeField] private GameObject NumberSelectUI;
    private Button[] cardButtons = new Button[3];

    // DD -> 동일 카드 중복 안되도록
    int firstCardIndex = 404;
    int secondCardIndex = 404;
    string cardName = "";
    //

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*UpdateState();
        if (DataManager.Instance.justCleared)
        {
            DataManager.Instance.justCleared = false;
            StartCoroutine("CardSelect");
        }*/
        // DD
        if (DataManager.Instance.itemEvent)
        {
            if(DataManager.Instance.firstRoom)
            {
                DataManager.Instance.firstRoom = false;
            }
            else
            {
                DataManager.Instance.itemEvent = false;
                StartCoroutine("CardSelect");
            }
        }
        // *DD
    }
    IEnumerator CardSelect()
    {
        CardSelectUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        for (int i = 0; i < 3; i++)
        {
            GameObject card = RandcomCard();
            card.GetComponent<RectTransform>().position =
                new Vector3(card.GetComponent<RectTransform>().position.x + (i - 1) * 500,
                card.GetComponent<RectTransform>().position.y, card.GetComponent<RectTransform>().position.z);
            cardButtons[i] = card.GetComponent<Button>();
        }
        foreach (var button in cardButtons)
        {
            button.onClick.AddListener(() => OnCardSelected(button));
        }
    }
    private void OnCardSelected(Button selectedButton)
    {
        cardName = selectedButton.gameObject.name;
        NumberSelectUI.gameObject.SetActive(true);
    }

    public void OnNumberSelected(Button selectedButton)
    {
        int num = 0;
        switch(selectedButton.gameObject.name)
        {
            case "Button1":
                num = 0;
                break;
            case "Button2":
                num = 1;
                break;
            case "Button3":
                num = 2;
                break;
            case "Button4":
                num = 3;
                break;
            default:
                num = 0;
                break;
        }

        NumberSelectUI.gameObject.SetActive(false);

        // 선택된 카드의 효과를 적용
        ApplyCardEffect(cardName, num);
        cardName = "";
        // Time.timeScale을 1로 설정하여 게임 재개
        Time.timeScale = 1f;

        // 카드를 파괴
        foreach (var button in cardButtons)
        {
            Destroy(button.gameObject);
        }

        // 카드 선택 캔버스를 비활성화
        CardSelectUI.gameObject.SetActive(false);
    }

    private void ApplyCardEffect(string cardName, int playerPos)
    {
        Stat cardStat;
        int rand;
        switch (cardName)
        {
            /*case "card1":
                Debug.Log("에픽스킬!");
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShortSword.ToString())
                    DataManager.Instance.ShurikenDamage += 3f;
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.LongSword.ToString())
                    DataManager.Instance.SwordLength += 1f;
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Axe.ToString())
                    DataManager.Instance.AxeDamage += 5;
                if(DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
                {
                    if (DataManager.Instance.epicSkill)
                        DataManager.Instance.SkillDamage *= 1.25f;
                }
                if(DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
                {
                    if (DataManager.Instance.epicSkill)
                        DataManager.Instance.SkillDamage *= 1.25f;
                }
                if(DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
                {
                    if (DataManager.Instance.epicSkill)
                        DataManager.Instance.SkillDamage *= 1.25f;
                }
                DataManager.Instance.epicSkill = true;
                break;
            case "card2":
                DataManager.Instance.additionalDashCount += 1;

                Debug.Log("대시추가!");
                break;
            case "card3":
                Debug.Log("체력추가!");
                DataManager.Instance.additionalMaxHealth += 1;
                DataManager.Instance.Health += 1;
                break;
            case "card4":
                Debug.Log("공격속도증가!");
                if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
                    DataManager.Instance.additionalAttackSpeed -= 0.02f;
                else
                    DataManager.Instance.additionalAttackSpeed += 50f;
                break;
            case "card5":
                Debug.Log("이동속도증가!");
                DataManager.Instance.additionalSpeed += 1.5f;

                break;
            case "card6":
                Debug.Log("공격력증가!");
                DataManager.Instance.additionalDamage += 0.4f;

                break;
            case "card7":
                Debug.Log("체력회복!");
                if (DataManager.Instance.Health < DataManager.Instance.MaxHealth)
                    DataManager.Instance.Health += 0.5f;
                break;
            case "card8":
                Debug.Log("꽝!");
                break;
            case "card9":
                Debug.Log("랜덤!");
                ApplyCardEffect("card" + Random.Range(4, 9));
                break;
            default:
                break;*/
            case "Epic_Heal":
                rand = Random.Range(0, 11);
                DataManager.Instance.PartyFormation[playerPos].heroHp += rand;
                if (DataManager.Instance.PartyFormation[playerPos].heroHp > DataManager.Instance.PartyFormation[playerPos].heroMaxHP)
                    DataManager.Instance.PartyFormation[playerPos].heroHp = DataManager.Instance.PartyFormation[playerPos].heroMaxHP;
                Debug.Log(playerPos.ToString() + "번째 플레이어 :  " + rand.ToString() + "만큼의 체력 회복");
                break;
            case "Epic_Damage":
                cardStat = new Stat(rand = Random.Range(1, 6), 0, 0, 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDmg += cardStat.tempDmg;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else                               
                            DataManager.Instance.tempStats[1].tempDmg += cardStat.tempDmg;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else                               
                            DataManager.Instance.tempStats[2].tempDmg += cardStat.tempDmg;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else                               
                            DataManager.Instance.tempStats[3].tempDmg += cardStat.tempDmg;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 데미지 " + rand.ToString() + "만큼 증가");
                break;
            case "Epic_Dodge":
                cardStat = new Stat(0, rand = Random.Range(5, 16), 0, 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDodge += cardStat.tempDodge;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempDodge += cardStat.tempDodge;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempDodge += cardStat.tempDodge;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempDodge += cardStat.tempDodge;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 회피율 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Epic_Protect":
                cardStat = new Stat(0, 0, rand = Random.Range(5, 21), 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempProtect += cardStat.tempProtect;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempProtect += cardStat.tempProtect;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempProtect += cardStat.tempProtect;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempProtect += cardStat.tempProtect;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 데미지 경감률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Epic_Speed":
                cardStat = new Stat(0, 0, 0, rand = Random.Range(1, 6), 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempSpeed += cardStat.tempSpeed;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 속도 " + rand.ToString() + "만큼 증가");
                break;
            case "Epic_Acc":
                cardStat = new Stat(0, 0, 0, 0, rand = Random.Range(5, 21), 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempAccuracy += cardStat.tempAccuracy;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 명중률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Epic_Crit":
                cardStat = new Stat(0, 0, 0, 0, 0, rand = Random.Range(5, 16), 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempCrit += cardStat.tempCrit;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempCrit += cardStat.tempCrit;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempCrit += cardStat.tempCrit;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempCrit += cardStat.tempCrit;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 치명률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Epic_DD":
                cardStat = new Stat(0, 0, 0, 0, 0, 0, rand = Random.Range(5, 21));
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDD += cardStat.tempDD;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempDD += cardStat.tempDD;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempDD += cardStat.tempDD;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempDD += cardStat.tempDD;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 죽음 저항 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Normal_Heal":
                rand = Random.Range(0, 13) - 2;
                if(rand < 0)
                {
                    if(DataManager.Instance.PartyFormation[playerPos].heroHp == 0)
                    {
                        // 죽음의 일격 트리거 -> 일정 확률로 죽거나 살거나
                    }
                    else
                    {
                        DataManager.Instance.PartyFormation[playerPos].heroHp += rand;
                        if (DataManager.Instance.PartyFormation[playerPos].heroHp <= 0)
                        {
                            DataManager.Instance.PartyFormation[playerPos].heroHp = 0;
                            // 죽음의 문턱 상태 + 죽음의 문턱 디버프 표시
                        }
                    }
                }
                else
                {
                    DataManager.Instance.PartyFormation[playerPos].heroHp += rand;
                    if (DataManager.Instance.PartyFormation[playerPos].heroHp > DataManager.Instance.PartyFormation[playerPos].heroMaxHP)
                        DataManager.Instance.PartyFormation[playerPos].heroHp = DataManager.Instance.PartyFormation[playerPos].heroMaxHP;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 체력 " + rand.ToString() + "만큼 회복");
                break;
            case "Normal_Damage":
                cardStat = new Stat(rand = Random.Range(0, 8) - 2, 0, 0, 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDmg += cardStat.tempDmg;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempDmg += cardStat.tempDmg;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempDmg += cardStat.tempDmg;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempDmg += cardStat.tempDmg;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 데미지 " + rand.ToString() + "만큼 증가");
                break;
            case "Normal_Dodge":
                cardStat = new Stat(0, rand = Random.Range(0, 21) - 5, 0, 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDodge += cardStat.tempDodge;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempDodge += cardStat.tempDodge;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempDodge += cardStat.tempDodge;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempDodge += cardStat.tempDodge;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 회피율 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Normal_Protect":
                cardStat = new Stat(0, 0, rand = Random.Range(0, 31) - 10, 0, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempProtect += cardStat.tempProtect;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempProtect += cardStat.tempProtect;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempProtect += cardStat.tempProtect;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempProtect += cardStat.tempProtect;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 데미지 경감률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Normal_Speed":
                cardStat = new Stat(0, 0, 0, rand = Random.Range(0, 8) - 2, 0, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempSpeed += cardStat.tempSpeed;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempSpeed += cardStat.tempSpeed;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 속도 " + rand.ToString() + "만큼 증가");
                break;
            case "Normal_Acc":
                cardStat = new Stat(0, 0, 0, 0, rand = Random.Range(0, 21) - 5, 0, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempAccuracy += cardStat.tempAccuracy;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempAccuracy += cardStat.tempAccuracy;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 명중률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Normal_Crit":
                cardStat = new Stat(0, 0, 0, 0, 0, rand = Random.Range(0, 16) - 5, 0);
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempCrit += cardStat.tempCrit;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempCrit += cardStat.tempCrit;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempCrit += cardStat.tempCrit;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempCrit += cardStat.tempCrit;
                        break;
                }
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 치명률 " + rand.ToString() + "% 만큼 증가");
                break;
            case "Normal_DD":
                cardStat = new Stat(0, 0, 0, 0, 0, 0, rand = Random.Range(0, 21) - 5);
                Debug.Log(playerPos.ToString() + "번째 플레이어 : 죽음 저항 " + rand.ToString() + "% 만큼 증가");
                switch (playerPos)
                {
                    case 0:
                        if (DataManager.Instance.tempStats[0] == null)
                            DataManager.Instance.tempStats[0] = cardStat;
                        else
                            DataManager.Instance.tempStats[0].tempDD += cardStat.tempDD;
                        break;
                    case 1:
                        if (DataManager.Instance.tempStats[1] == null)
                            DataManager.Instance.tempStats[1] = cardStat;
                        else
                            DataManager.Instance.tempStats[1].tempDD += cardStat.tempDD;
                        break;
                    case 2:
                        if (DataManager.Instance.tempStats[2] == null)
                            DataManager.Instance.tempStats[2] = cardStat;
                        else
                            DataManager.Instance.tempStats[2].tempDD += cardStat.tempDD;
                        break;
                    case 3:
                        if (DataManager.Instance.tempStats[3] == null)
                            DataManager.Instance.tempStats[3] = cardStat;
                        else
                            DataManager.Instance.tempStats[3].tempDD += cardStat.tempDD;
                        break;
                }
                break;
        }
    }
    private void UpdateState()
    {
        DataManager.Instance.Damage = DataManager.Instance.firstDamage + DataManager.Instance.additionalDamage;
        DataManager.Instance.Speed = DataManager.Instance.firstSpeed + DataManager.Instance.additionalSpeed;
        DataManager.Instance.AttacSpeed = DataManager.Instance.firstAttackSpeed + DataManager.Instance.additionalAttackSpeed;
        DataManager.Instance.MaxHealth = DataManager.Instance.firstMaxHealth + DataManager.Instance.additionalMaxHealth;
        DataManager.Instance.DashCount = DataManager.Instance.firstDashCount + DataManager.Instance.additionalDashCount;
    }
    private GameObject RandcomCard()
    {
        GameObject card = null;
        int rand = Random.Range(0, 20);
        if (rand <= 2) // 15% 확률 -> 에픽 이벤트
        {
            string effect = "";
            card = Instantiate(epicCard, CardSelectUI.transform);
            while(true)
            {
                /*if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShortSword.ToString())
                    effect = "Powerful Shuriken!\nDamage ++";
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.LongSword.ToString())
                    effect = "Longer Sword!\nLength ++";
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Axe.ToString())
                    effect = "Powerful Axe!\nDamage ++";
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShotGun.ToString())
                {
                    if (!DataManager.Instance.epicSkill)
                        effect = "[RightClick]\nHuge Shotgun!";
                    else
                        effect = "Powerful Shotgun!\nDamage ++";
                }
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Rifle.ToString())
                {
                    if (!DataManager.Instance.epicSkill)
                        effect = "[RightClick]\nPowerful Rifle!";
                    else
                        effect = "Powerful Rifle!\nDamage ++";
                }
                if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.Sniper.ToString())
                {
                    if (!DataManager.Instance.epicSkill)
                        effect = "[RightClick]\nBIG BULLET!";
                    else
                        effect = "BIG BULLET!\nDamage ++";
                }*/
                rand = Random.Range(0, 8);
                if(firstCardIndex == 404)
                {
                    firstCardIndex = rand;
                    break;
                }
                else if(secondCardIndex == 404 && firstCardIndex != rand)
                {
                    secondCardIndex = rand;
                    break;
                }
                else
                {
                    if(firstCardIndex != rand && secondCardIndex != rand)
                    {
                        firstCardIndex = secondCardIndex = 404;
                        break;
                    }
                }
            }
            switch (rand)
            {
                case 0:
                    effect = "아군 체력 회복\n(0 ~ 10)";
                    card.gameObject.name = "Epic_Heal";
                    break;
                case 1:
                    effect = "아군 최종 데미지 증가\n(1 ~ 5)";
                    card.gameObject.name = "Epic_Damage";
                    break;
                case 2:
                    effect = "아군 회피율 증가\n(5 ~ 15)";
                    card.gameObject.name = "Epic_Dodge";
                    break;
                case 3:
                    effect = "아군 데미지 경감률 증가\n(5 ~ 20)";
                    card.gameObject.name = "Epic_Protect";
                    break;
                case 4:
                    effect = "아군 속도 증가\n(1 ~ 5)";
                    card.gameObject.name = "Epic_Speed";
                    break;
                case 5:
                    effect = "아군 명중 보정치 증가\n(5 ~ 20)";
                    card.gameObject.name = "Epic_Acc";
                    break;
                case 6:
                    effect = "아군 치명타 확률 증가\n(5 ~ 15)";
                    card.gameObject.name = "Epic_Crit";
                    break;
                case 7:
                    effect = "아군의 죽음의 일격 저항력 증가\n(5 ~ 20)";
                    card.gameObject.name = "Epic_DD";
                    break;
            }
            card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "빛이 내리는 은총이로구나...";
            card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effect;
        }
        /*else if (rand <= 6)
        {
            card = Instantiate(epicCard, CardSelectUI.transform);
            if (rand == 5)
            {
                card.gameObject.name = "card2";
                card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "DASH COUNT\n+ 1";
                card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Now, you are much faster...";
            }
            else
            {
                card.gameObject.name = "card3";
                card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "MAX HEALTH\n+ 1";
                card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Now, you are much bigger...";
            }
        }*/
        else // 85% 확률 -> 일반 이벤트
        {
            string effect = "";
            card = Instantiate(normalCard, CardSelectUI.transform);
            while (true)
            {
                rand = Random.Range(10, 18);
                if (firstCardIndex == 404)
                {
                    firstCardIndex = rand;
                    break;
                }
                else if (secondCardIndex == 404 && firstCardIndex != rand)
                {
                    secondCardIndex = rand;
                    break;
                }
                else
                {
                    if (firstCardIndex != rand && secondCardIndex != rand)
                    {
                        firstCardIndex = secondCardIndex = 404;
                        break;
                    }
                }
            }
            switch (rand)
            {
                case 10:
                    effect = "아군 체력 회복\n(-2 ~ 10)";
                    card.gameObject.name = "Normal_Heal";
                    break;
                case 11:
                    effect = "아군 최종 데미지 증가\n(-2 ~ 5)";
                    card.gameObject.name = "Normal_Damage";
                    break;
                case 12:
                    effect = "아군 회피율 증가\n(-5 ~ 15)";
                    card.gameObject.name = "Normal_Dodge";
                    break;
                case 13:
                    effect = "아군 데미지 경감률 증가\n(-10 ~ 20)";
                    card.gameObject.name = "Normal_Protect";
                    break;
                case 14:
                    effect = "아군 속도 증가\n(-2 ~ 5)";
                    card.gameObject.name = "Normal_Speed";
                    break;
                case 15:
                    effect = "아군 명중 보정치 증가\n(-5 ~ 15)";
                    card.gameObject.name = "Normal_Acc";
                    break;
                case 16:
                    effect = "아군 치명타 확률 증가\n(-5 ~ 10)";
                    card.gameObject.name = "Normal_Crit";
                    break;
                case 17:
                    effect = "아군의 죽음의 일격 저항력 증가\n(-5 ~ 15)";
                    card.gameObject.name = "Normal_DD";
                    break;
            }
            card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "강해짐에 대한 열망을 시험해 보자꾸나";
            card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effect;
        }
        return card;
    }
}
