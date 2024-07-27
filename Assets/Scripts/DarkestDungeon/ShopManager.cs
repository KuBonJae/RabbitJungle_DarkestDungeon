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
    public GameObject[] PlayerPrefabs; // Melee / Marksman / Healer / Supporter / Tanker
    int[] HowMuch = new int[11] { 150, 150, 300, 200, 450, 350, 200, 250, 250, 200, 500 };

    bool maxhp, heal, minD, maxD, spd, crit, deathResist, healAmount, stressAmount, stress, revive;
    int playerNum = 404;

    // Start is called before the first frame update
    void Start()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false;
        playerNum = 404;
        for(int i=0; i<11; i++)
        {
            ShopItemBtns[i].transform.Find("Gold").GetComponent<TextMeshProUGUI>().text = HowMuch[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CoinAmount.GetComponent<TextMeshProUGUI>().text = "���� ���� : " + DataManager.Instance.coin.ToString();
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
        BuyRevive(revive);
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMaxHP += 3; // ���� ����
                    DataManager.Instance.coin -= HowMuch[0]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if(DataManager.Instance.PartyFormation[playerNum].heroHp == DataManager.Instance.PartyFormation[playerNum].heroMaxHP)
                    {
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�̹� �ִ� ü���Դϴ�.";
                    }
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroHp += 8; // ���� ����
                        if (DataManager.Instance.PartyFormation[playerNum].heroHp > DataManager.Instance.PartyFormation[playerNum].heroMaxHP)
                            DataManager.Instance.PartyFormation[playerNum].heroHp = DataManager.Instance.PartyFormation[playerNum].heroMaxHP;
                        DataManager.Instance.coin -= HowMuch[1]; // ���� ����
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                        playerNum = 404; // ���� ����
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                    }
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMinDamage += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[2]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroMaxDamage += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[3]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroBasicSpeed += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[4]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    DataManager.Instance.PartyFormation[playerNum].heroBasicCriticalHit += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[5]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if(DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor >= 80)
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �ִ�ġ�Դϴ�\n(�ִ� 80)";
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor += 5; // ���� ����
                        if (DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor > 80)
                            DataManager.Instance.PartyFormation[playerNum].heroBasicDeathDoor = 80;

                        DataManager.Instance.coin -= HowMuch[6]; // ���� ����
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                        playerNum = 404; // ���� ����
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                    }  
                    
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead && DataManager.Instance.PartyFormation[playerNum].heroClass == ClassName.Healer)
                {
                    ((HealerSetting)DataManager.Instance.PartyFormation[playerNum]).minHealAmount += 1; // ���� ����
                    ((HealerSetting)DataManager.Instance.PartyFormation[playerNum]).maxHealAmount += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[7]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�÷��̾ ����߰ų�\n������ �ƴմϴ�.";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead && DataManager.Instance.PartyFormation[playerNum].heroClass == ClassName.Supporter)
                {
                    ((SupporterSetting)DataManager.Instance.PartyFormation[playerNum]).minStressDownAmount += 1; // ���� ����
                    ((SupporterSetting)DataManager.Instance.PartyFormation[playerNum]).maxStressDownAmount += 1; // ���� ����
                    DataManager.Instance.coin -= HowMuch[7]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�÷��̾ ����߰ų�\n�����Ͱ� �ƴմϴ�.";
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
                // �÷��̾� ���� ���
            }
            else
            {
                if (!DataManager.Instance.PartyFormation[playerNum].isDead)
                {
                    if (DataManager.Instance.PartyFormation[playerNum].heroStress <= 0)
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�̹� ����ġ�� ��Ʈ������\n������ �ֽ��ϴ�.";
                    else
                    {
                        DataManager.Instance.PartyFormation[playerNum].heroStress -= 10; // ���� ����
                        if (DataManager.Instance.PartyFormation[playerNum].heroStress < 0)
                        {
                            DataManager.Instance.PartyFormation[playerNum].heroStress = 0;
                            // ���� �ر����¸� �⺻ ���·� �ǵ���
                            if (DataManager.Instance.PartyFormation[playerNum].Stress == Stress.Negative)
                                DataManager.Instance.PartyFormation[playerNum].Stress = Stress.Default;
                        }
                        DataManager.Instance.coin -= HowMuch[9]; // ���� ����
                        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                        playerNum = 404; // ���� ����
                        SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� �Ϸ�";
                    }
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! �׾������!";
                }
            }
        }
    }
    void BuyRevive(bool b)
    {
        if (b)
        {
            if (playerNum == 404)
            {
                // �÷��̾� ���� ���
            }
            else
            {
                if (DataManager.Instance.PartyFormation[playerNum].isDead) // ���� ������׸� ��� ����
                {
                    //DataManager.Instance.PartyFormation[playerNum].heroBasicSpeed += 1; // ���� ����
                    DataManager.Instance.PartyFormation[playerNum].isDead = false; // ���� ĳ���� �ǻ츮��
                    DataManager.Instance.PartyFormation[playerNum].DeathChecked = false;
                    DataManager.Instance.PartyFormation[playerNum].heroHp = 1; // ü���� 1��
                    DataManager.Instance.PartyFormation[playerNum].heroStress = 0; // ��Ʈ���� ����
                    DataManager.Instance.PartyFormation[playerNum].Stress = Stress.Default; // ���� �ر�, ���� ����
                    DataManager.Instance.coin -= HowMuch[10]; // ���� ����
                    maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
                    playerNum = 404; // ���� ����
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�ǻ츮�� �Ϸ�";

                    Destroy(GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform.GetChild(0).gameObject); // ������ ���� ���� ������ ����
                    switch(DataManager.Instance.PartyFormation[playerNum].heroClass) // Ŭ���� �� ���� �˸��� ���� ������ �����
                    {
                        case ClassName.Healer:
                            Instantiate(PlayerPrefabs[2], GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform).transform.localPosition = Vector3.zero;
                            break;
                        case ClassName.Supporter:
                            Instantiate(PlayerPrefabs[3], GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform).transform.localPosition = Vector3.zero;
                            break;
                        case ClassName.Melee:
                            Instantiate(PlayerPrefabs[0], GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform).transform.localPosition = Vector3.zero;
                            break;
                        case ClassName.Marksman:
                            Instantiate(PlayerPrefabs[1], GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform).transform.localPosition = Vector3.zero;
                            break;
                        case ClassName.Tanker:
                            Instantiate(PlayerPrefabs[4], GameObject.Find("ShopScene").transform.Find("Player" + (playerNum + 1).ToString()).transform).transform.localPosition = Vector3.zero;
                            break;
                    }
                }
                else
                {
                    SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��� ����ִ� �����\n���� ����!";
                }
            }
        }
    }
    #region ��ư Ŭ�� �� ������ ȹ��
    public void OnMaxHpBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[0])
        {
            maxhp = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�ִ� ü�� ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnHealBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[1])
        {
            heal = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "ü�� ȸ��";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnMinDmgBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[2])
        {
            minD = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�ּ� Dmg ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnMaxDmgBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[3])
        {
            maxD = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�ִ� Dmg ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnSpeedBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[4])
        {
            spd = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "�ӵ� ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnCritBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[5])
        {
            crit = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "ġ��� ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnDeathResistBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[6])
        {
            deathResist = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "���� ���׷� ȸ��";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnHealAmountBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[7])
        {
            healAmount = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "ȸ���� ����\n(����)";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnStressAmountBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[8])
        {
            stressAmount = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��Ʈ���� ���� ����\n(������)";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnStressBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[9])
        {
            stress = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��Ʈ���� ����";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
        }
    }
    public void OnReviveBtnClicked()
    {
        maxhp = heal = minD = maxD = spd = crit = deathResist = healAmount = stressAmount = stress = revive = false; // ���� ����
        playerNum = 404; // ���� ����
        if (DataManager.Instance.coin > HowMuch[9])
        {
            revive = true;
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "ü�� 1�� �ǻ츮��";
        }
        else
        {
            SelectedBuffName.GetComponent<TextMeshProUGUI>().text = "��! ��������!";
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
