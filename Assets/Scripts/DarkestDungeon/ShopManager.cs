using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public GameObject CoinAmount;
    public GameObject SelectedBuffName;
    public GameObject[] ShopItemBtns;
    public GameObject[] PlayerBtns;
    int[] HowMuch = new int[10] { 150, 150, 300, 200, 450, 350, 200, 250, 250, 200 };

    bool maxhp, heal, minD, maxD, spd, crit, deathResist, healAmount, stressAmount, stress;
    int playerNum = 404;

    // Start is called before the first frame update
    void Start()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        playerNum = 404;
        for(int i=0; i<10; i++)
        {
            ShopItemBtns[i].transform.Find("Gold").GetComponent<TextMeshProUGUI>().text = HowMuch[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoinAmount.GetComponent<TextMeshProUGUI>().text = "현재 코인 : " + DataManager.Instance.coin.ToString();
        BuyMaxHp(maxhp);
        BuyHeal(heal);
        BuyMinDmg(minD);
        BuyMaxDmg(maxD);
        BuySpd(spd);
        BuyCrit(crit);
        BuyDeathResist(deathResist);
        BuyHealAmount(healAmount);
        BuyStressAmount(stressAmount);
        BuyStressDown(stress);
    }

    public void OnExitBtnClicked()
    {
        Time.timeScale = 1f;
        DataManager.Instance.StageLevel++;
        SceneManager.LoadScene("Game");
    }

    void BuyMaxHp(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMaxHP += 3; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[0]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyHeal(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if(DataManager.Instance.PartyFormation[playerNum].heroHp == DataManager.Instance.PartyFormation[playerNum].heroMaxHP)
                    {
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "이미 최대 체력입니다.";
                    }
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroHp += 8; // 버프 제공
                        if (DataManager.Instance.PartyFormation[playerNum].heroHp > DataManager.Instance.PartyFormation[playerNum].heroMaxHP)
                            DataManager.Instance.PartyFormation[playerNum].heroHp = DataManager.Instance.PartyFormation[playerNum].heroMaxHP;
                        DataManager.Instance.coin -= HowMuch[1]; // 코인 감소
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                        playerNum = 404; // 선택 해제
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                    }
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyMinDmg(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMinDamage += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[2]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyMaxDmg(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMaxDamage += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[3]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuySpd(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroBasicSpeed += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[4]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyCrit(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroBasicCriticalHit += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[5]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyDeathResist(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if(DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor >= 80)
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "현재 최대치입니다 (최대 80)";
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor += 5; // 버프 제공
                        if (DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor > 80)
                            DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor = 80;

                        DataManager.Instance.coin -= HowMuch[6]; // 코인 감소
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                        playerNum = 404; // 선택 해제
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                    }  
                    
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    void BuyHealAmount(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead && DataManager.Instance.PartyFormation[playerNum].heroClass == ClassName.Healer)
                {
                    ((HealerSetting)DataManager.Instance.PartyFormation[playerNum]).minHealAmount += 1; // 버프 제공
                    ((HealerSetting)DataManager.Instance.PartyFormation[playerNum]).maxHealAmount += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[7]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "플레이어가 사망했거나 힐러가 아닙니다.";
                }
            }
        }
    }
    void BuyStressAmount(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead && DataManager.Instance.PartyFormation[playerNum].heroClass == ClassName.Supporter)
                {
                    ((SupporterSetting)DataManager.Instance.PartyFormation[playerNum]).minStressDownAmount += 1; // 버프 제공
                    ((SupporterSetting)DataManager.Instance.PartyFormation[playerNum]).maxStressDownAmount += 1; // 버프 제공
                    DataManager.Instance.coin -= HowMuch[7]; // 코인 감소
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                    playerNum = 404; // 선택 해제
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "플레이어가 사망했거나 서포터가 아닙니다.";
                }
            }
        }
    }
    void BuyStressDown(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // 플레이어 선택 대기
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if (DataManager.Instance.PartyFormation[playerNum].heroStress <= 0)
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "이미 최저치의 스트레스를 가지고 있습니다.";
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroStress -= 10; // 버프 제공
                        if (DataManager.Instance.PartyFormation[playerNum].heroStress < 0)
                        {
                            DataManager.Instance.PartyFormation[playerNum].heroStress = 0;
                            // 만약 붕괴상태면 기본 상태로 되돌림
                            if (DataManager.Instance.PartyFormation[playerNum].Stress == Stress.Negative)
                                DataManager.Instance.PartyFormation[playerNum].Stress = Stress.Default;
                        }
                        DataManager.Instance.coin -= HowMuch[9]; // 코인 감소
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false; // 선택 해제
                        playerNum = 404; // 선택 해제
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "구매 완료";
                    }
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 죽어버리렴!";
                }
            }
        }
    }
    #region 버튼 클릭 시 데이터 획득
    public void OnMaxHpBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if(DataManager.Instance.coin > HowMuch[0])
        {
            maxhp = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "최대 체력 증가";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnHealBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[1])
        {
            heal = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "체력 회복";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnMinDmgBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[2])
        {
            minD = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "최소 Dmg 증가";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnMaxDmgBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[3])
        {
            maxD = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "최대 Dmg 증가";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnSpeedBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[4])
        {
            spd = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "속도 증가";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnCritBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[5])
        {
            crit = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "치명률 증가";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnDeathResistBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[6])
        {
            deathResist = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "죽음 저항력 회복";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnHealAmountBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[7])
        {
            healAmount = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "회복량 증가\n(힐러)";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnStressAmountBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[8])
        {
            stressAmount = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "스트레스 감소 증가\n(서포터)";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnStressBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = false;
        if (DataManager.Instance.coin > HowMuch[9])
        {
            stress = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "스트레스 감소";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "얘! 돈가져와!";
        }
    }
    public void OnPlayer1BtnClicked()
    {
        playerNum = 0;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnPlayer2BtnClicked()
    {
        playerNum = 1;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnPlayer3BtnClicked()
    {
        playerNum = 2;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnPlayer4BtnClicked()
    {
        playerNum = 3;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    #endregion
}
