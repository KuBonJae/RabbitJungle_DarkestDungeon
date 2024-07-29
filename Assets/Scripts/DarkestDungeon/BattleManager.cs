using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject BattleCanvas;
    public GameObject BattleScene;
    public GameObject Tomb;
    public GameObject Marksman;
    public GameObject Melee;
    public GameObject Tanker;
    public GameObject Healer;
    public GameObject Supporter;
    public GameObject[] EnemyPrefabs;
    public GameObject SkillIcons;
    public GameObject[] Skills;
    private Button CurSelectedBtn;
    public GameObject[] Serifu;
    GameObject BattleEventCamera;
    public GameObject DamageTextPrefab;

    private Queue<string> BattleLog = new Queue<string>();
    private const int MaxLog = 10;

    private int EnemyLeft = 4;
    private int HeroLeft = 4;
    private int TurnCount = 0;
    private int NamedEnemyNum = 0;

    private int CurHero;
    private int CurEnemy;
    private int SelectedEnemy;
    private int SelectedHero;

    //private bool nextTurn = false;
    private bool TurnEnd = false;
    private bool battleStart = false;
    private bool firstBtnClicked = false;
    private bool secondBtnClicked = false;
    private bool firstEnemyClicked = false;
    private bool secondEnemyClicked = false;
    private bool thirdEnemyClicked = false;
    private bool fourthEnemyClicked = false;
    private bool firstHeroClicked = false;
    private bool secondHeroClicked = false;
    private bool thirdHeroClicked = false;
    private bool fourthHeroClicked = false;
    private bool nextBattleOrder;
    private bool shouldCheckingOrder;

    PriorityQueue SpeedOrder;

    GameObject moveObject = null;
    Vector3 destination;
    Vector3 origin;
    float moveTime = 0f;

    private string[] CorruptionSerifu = new string[4] { "�� ����!", "�츰 ���߾�", "��û��!", "�峭��?" };
    private string[] CourageSerifu = new string[4] { "�� ��!", "�� �� �־�", "�����ٰ�!", "�����̾�!" };

    WaitForSecondsRealtime waitForAttack = new WaitForSecondsRealtime(1f);
    WaitForSecondsRealtime waitForCritical = new WaitForSecondsRealtime(1f);
    WaitForSecondsRealtime waitForSerifu = new WaitForSecondsRealtime(2f);
    WaitForSecondsRealtime waitForCorruption = new WaitForSecondsRealtime(2.5f);
    WaitForSecondsRealtime waitForTurnEnd = new WaitForSecondsRealtime(2f);

    // Start is called before the first frame update
    void Start()
    {
        BattleEventCamera = GameObject.Find("Camera").transform.Find("BattleEventCam").gameObject;

        EnemyLeft = 4;
        NamedEnemyNum = 0;
        SpeedOrder = new PriorityQueue(8);
        firstBtnClicked = false;
        secondBtnClicked = false;
        nextBattleOrder = false; // �� ó�� �����Ҷ� �ѹ� ����
        firstEnemyClicked = false;
        secondEnemyClicked = false;
        thirdEnemyClicked = false;
        fourthEnemyClicked = false;
        firstHeroClicked = false;
        secondHeroClicked = false;
        thirdHeroClicked = false;
        fourthHeroClicked = false;

        for(int i=0; i<4;i++)
        {
            if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative) // �������� �Ѿ �� �������̸� ����� ��?
            {
                if (DataManager.Instance.PartyFormation[i].heroStress <= 100)
                    DataManager.Instance.PartyFormation[i].heroStress += 20; // ��Ʈ���� 100 ���ϸ� 20 �߰�
            }
            else if(DataManager.Instance.PartyFormation[i].Stress == Stress.Positive) // �������� �Ѿ �� �������̸� ����� ��?
            {
                if (DataManager.Instance.PartyFormation[i].heroStress >= 150) // ��Ʈ������ 150 �̻��̸�
                {
                    DataManager.Instance.PartyFormation[i].heroStress = 0; // ��Ʈ���� �ʱ�ȭ
                    DataManager.Instance.PartyFormation[i].Stress = Stress.Default; // ������ ���� �ʱ�ȭ ���ѹ�����
                }
                else // ��Ʈ���� ������ �� �ߴٸ�
                {
                    for(int j=0;j<4;j++)
                    {
                        if (i == j)
                            continue; // ������ ����

                        DataManager.Instance.PartyFormation[j].heroStress -= 10; // �Ʊ��� ��Ʈ���� 10�� �ٿ���
                        if (DataManager.Instance.PartyFormation[j].heroStress <= 0)
                            DataManager.Instance.PartyFormation[j].heroStress = 0;
                        DataManager.Instance.PartyFormation[j].heroHp += 5; // �Ʊ��� ü�� ȸ�� 5��
                        if (DataManager.Instance.PartyFormation[j].heroHp >= DataManager.Instance.PartyFormation[j].heroMaxHP)
                            DataManager.Instance.PartyFormation[j].heroHp = DataManager.Instance.PartyFormation[j].heroMaxHP;
                    }
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(BattleCanvas.activeSelf && Input.GetKeyDown(KeyCode.Z))
        {
            OnFirstSkillBtnClicked(Skills[0].GetComponent<Button>());
            Skills[0].GetComponent<Button>().Select();
        }
        if(BattleCanvas.activeSelf && Input.GetKeyDown(KeyCode.X))
        {
            OnSecondSkillBtnClicked(Skills[1].GetComponent<Button>());
            Skills[1].GetComponent<Button>().Select();
        }

        if (BattleCanvas.activeSelf && EnemyLeft == 0) // ��Ʋ ���� ���ε� ���� ���� 0�̸� ��Ʋ�� ����
        {
            // ��Ʋ ����� �����ϸ�, ������ ȹ���ϰ� ��� ������
            EnemyLeft = 4;
            TurnCount = 0;
            DataManager.Instance.battle_ing = false; // end the battle phase

            BattleCanvas.transform.Find("Player1").gameObject.SetActive(true); // Ȥ�� ���ܵ� ��ư���� �̸� �� ���ڸ��� �����α�
            BattleCanvas.transform.Find("Player2").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Player3").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Player4").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy1").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy2").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy3").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy4").gameObject.SetActive(true);


            firstBtnClicked = false;
            secondBtnClicked = false;
            nextBattleOrder = false; // ������ BattleSetting���� ��������ִ� �׳� �����ϰ� false�� ��ȯ
            firstEnemyClicked = false;
            secondEnemyClicked = false;
            thirdEnemyClicked = false;
            fourthEnemyClicked = false;
            firstHeroClicked = false;
            secondHeroClicked = false;
            thirdHeroClicked = false;
            fourthHeroClicked = false;

            for(int i=0;i<4;i++)
            {
                DataManager.Instance.tempStats[i] = new Stat(0, 0, 0, 0, 0, 0, 0); // �ӽ� ���� �ʱ�ȭ
            }

            int coinAmount = 200 + UnityEngine.Random.Range(150, 300 + 1) + NamedEnemyNum * 100; // �ּ� 300, �ִ� 500 + ���ӵ�� �� * 100;
            NamedEnemyNum = 0; // ����
            DataManager.Instance.coin += coinAmount;
            string coinText = "���� " + coinAmount.ToString() + " ȹ��!";
            if(UnityEngine.Random.Range(0, 100) < 5) // 5% Ȯ����
            {
                DataManager.Instance.coin += coinAmount; // ���� �ѹ� �� ����
                coinText = "���� ���ʽ�! X 2��!\n���� " + (coinAmount * 2).ToString() + " ȹ��!";
            }
            DataManager.Instance.announcement = coinText;
            DataManager.Instance.makeAnnouncement = true;
            BattleCanvas.SetActive(false);
            Time.timeScale = 1f;
        }

        if (DataManager.Instance.battleEvent) // ��Ʋ �̺�Ʈ �߻� -> ��Ʋ ����
        {
            DataManager.Instance.battleEvent = false;
            while (BattleLog.Count > 0)
                BattleLog.Dequeue();
            SkillIcons.SetActive(false);
            StartCoroutine("BattleSetting");
        }
        
        if(HeroLeft == 0) // �Ʊ��� 0�̸� ���� ����
        {
            SceneManager.LoadScene("GameOver");
        }

        if(TurnEnd || battleStart) // ��Ʋ�� ù ���� or ���� �����ٸ� ���� ���ġ
        {
            battleStart = false;
            TurnEnd = false;
            BattleCanvas.transform.Find("TurnCount").GetComponent<TextMeshProUGUI>().text = "���� �� : " + (++TurnCount).ToString();
            for(int i=0;i<4;i++)
            {
                if (DataManager.Instance.PartyFormation[i].heroBasicProtection != 0) // ��Ŀ�� �� ��ȣ ������ �����Ѵٸ�
                {
                    DataManager.Instance.PartyFormation[i].heroBuffRemain--; // ���� ���� Ƚ�� ����
                    if (DataManager.Instance.PartyFormation[i].heroBuffRemain < 0)
                        DataManager.Instance.PartyFormation[i].heroBasicProtection = 0;
                }
            }
            shouldCheckingOrder = true;
            while (SpeedOrder.Count > 0)
                SpeedOrder.Dequeue(); // �� �������� ���� ���� �ִٸ� �̸� �� ��� ��
            CheckAttackOrder();
        }

        if (DataManager.Instance.battle_ing) // ���� ��Ʋ ������
        {
            Time.timeScale = 0f; // �������϶� �׽� 0���� ����

            if (SpeedOrder.Count == 0 && !shouldCheckingOrder)
                TurnEnd = true;
            else
            {
                if (nextBattleOrder)
                {
                    nextBattleOrder = false;

                    firstBtnClicked = false; // ��� ��ư�� ���� ����
                    secondBtnClicked = false;
                    firstEnemyClicked = false;
                    secondEnemyClicked = false;
                    thirdEnemyClicked = false;
                    fourthEnemyClicked = false;
                    firstHeroClicked = false;
                    secondHeroClicked = false;
                    thirdHeroClicked = false;
                    fourthHeroClicked = false;

                    if (SpeedOrder.Peek().Item1 >= 10) // �����̸�
                    {
                        if (!DataManager.Instance.EnemyFormation[SpeedOrder.Peek().Item1 - 10].isDead)
                        {
                            SkillIcons.SetActive(false);
                            StartCoroutine("BattlePhase_Enemy");
                        }
                        else
                        {
                            SpeedOrder.Dequeue();
                            nextBattleOrder = true;
                        }
                    }
                    else
                    {
                        if (!DataManager.Instance.PartyFormation[SpeedOrder.Peek().Item1].isDead)
                        {
                            SkillIcons.SetActive(true);
                            switch (DataManager.Instance.PartyFormation[SpeedOrder.Peek().Item1].heroClass)
                            {
                                case ClassName.Melee:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "����";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "E1, E2\n���� ����";
                                    break;
                                case ClassName.Marksman:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "����";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "E3, E4\n���� ����";
                                    break;
                                case ClassName.Tanker:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "����";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "������ �氨\n����";
                                    break;
                                case ClassName.Healer:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "����";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "ġ��";
                                    break;
                                case ClassName.Supporter:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "����";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "��Ʈ����\n����";
                                    break;
                            }
                            StartCoroutine("BattlePhase_Hero");
                        }
                        else
                        {
                            SpeedOrder.Dequeue();
                            nextBattleOrder = true;
                        }
                    }
                }
            }
        }

        if (moveObject != null)
        {
            if(moveTime < 1.2f)
            {
                moveObject.transform.localPosition = Vector3.MoveTowards(moveObject.transform.localPosition, destination, Time.unscaledDeltaTime * 75);
                moveTime += Time.unscaledDeltaTime;
            }
            else
            {
                moveObject.transform.localPosition = Vector3.MoveTowards(moveObject.transform.localPosition, origin, Time.unscaledDeltaTime * 75);
                moveTime += Time.unscaledDeltaTime;
                if (moveObject.transform.localPosition == origin || moveTime >= 3f)
                {
                    moveTime = 0f;
                    moveObject = null;
                }
            }
        }

        if(BattleCanvas.activeSelf)
            UpdateHpAndStress();
    }

    void UpdateHpAndStress()
    {
        for(int i=0; i<4; i++)
        {
            //TextMeshProUGUI TEXT = BattleCanvas.transform.Find("Player" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>(); // ����, UI Canvas
            TextMeshProUGUI TEXT = BattleScene.transform.Find("Player" + (i + 1).ToString()).transform.Find("Canvas_P" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>(); // �ű�, ���� ������Ʈ�� ���� Canvas
            if (DataManager.Instance.PartyFormation[i].isDead)
                TEXT.text = "<color=\"red\">Dead </color>";
            else
            {
                TEXT.text = "<color=\"red\">" + DataManager.Instance.PartyFormation[i].heroHp.ToString() + " / " + DataManager.Instance.PartyFormation[i].heroMaxHP.ToString()
                    + "</color>\n" + DataManager.Instance.PartyFormation[i].heroStress.ToString();
            }
            //TEXT = BattleCanvas.transform.Find("Enemy" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>();
            TEXT = BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.Find("Canvas_E" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>();
            if (DataManager.Instance.EnemyFormation[i].isDead)
                TEXT.text = "<color=\"red\">Dead </color>";
            else
            {
                TEXT.text = "<color=\"red\">" + DataManager.Instance.EnemyFormation[i].Hp.ToString() + " / " + DataManager.Instance.EnemyFormation[i].MaxHP.ToString() + "</color>";
            }
        }
        
    }

    IEnumerator BattlePhase_Enemy()
    {
        Tuple<int, int> WhoseTurn = SpeedOrder.Peek();
        
        CurEnemy = WhoseTurn.Item1 - 10;
        // ������ ��ų�� �ϳ� �����ϰ� ����
        int randomSkill = UnityEngine.Random.Range(0, 2); // 1�� or 2�� ��ų
        if (randomSkill == 0) // ������ �������� �ִ� ��ų
        {
            // �� ������ ����
            int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
            // �������� ���� �Ʊ� ���� ����
            int HeroDmged;
            while (true)
            {
                HeroDmged = UnityEngine.Random.Range(0, 4);
                if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                    continue;
                else
                    break;
            }

            ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "(��)�� " + "Player" + (HeroDmged + 1).ToString() + "���� ����!");
            destination = new Vector3(-1, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.localPosition.y, 0);
            moveObject = BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).gameObject;
            origin = moveObject.transform.localPosition;
            //yield return new WaitForSecondsRealtime(2f);
            yield return waitForAttack;//new WaitForSecondsRealtime(2f); // �ð� ���̱�
            // �Ʊ��� ȸ������ Ȯ���ؼ� ȸ�� Ȯ��
            int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
            int randomDodge = UnityEngine.Random.Range(0, 101);
            if (HeroDodgeRate >= randomDodge)
            {
                // ȸ�� ����
                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                StartCoroutine(ShowDamageText("ȸ��", false, HeroDmged, false, true));
            }
            else // ����
            {
                int randomCritRate = UnityEngine.Random.Range(0, 101);
                if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                {
                    // ġ��Ÿ ����
                    ShowBattleLog("<color=\"red\">ġ��Ÿ!</color>");
                    Vector3 pos = BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.position 
                        + BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).transform.position; // �����ڿ� �ǰ����� ���� ��տ� ��ġ��ų ����
                    pos.x /= 2;
                    pos.y /= 2; // ��� ��
                    pos.z = BattleEventCamera.transform.position.z;
                    BattleEventCamera.transform.position = pos;
                    BattleEventCamera.SetActive(true);
                    randomDmg *= 2;
                    moveTime -= 1f; // movetoward�� �̵����� ģ���� ��� ����
                    StartCoroutine(ShowDamageText("<color=\"yellow\">ġ��Ÿ!</color>", false, HeroDmged, false, true));
                    //yield return new WaitForSecondsRealtime(1.5f);
                    yield return waitForCritical;
                    BattleEventCamera.SetActive(false);
                }
                if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // ���� ������ ����
                {
                    int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                        + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // ���� ������ �ϰ� ��Ƽ�� ����
                    int randomDeath = UnityEngine.Random.Range(0, 101);
                    if (randomDeathDoor >= randomDeath) // ���ڰ� �� ũ�� => ���߳´�
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���߳½��ϴ�. " + randomDeathDoor.ToString() + " / " + randomDeath.ToString());
                        DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // ���������� �׹� Ȯ�� ����
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� ������ �����ϴ�. ��Ʈ���� 20 ����");
                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += 20;
                        yield return StartCoroutine(CheckCorruptionOrCourage(HeroDmged));
                    }
                    else // ���߳��� ���ߴ�
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� <color=\"red\">���</color>�߽��ϴ�.");
                        DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                        HeroLeft--;
                        BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                        Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                        Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                        ShowBattleLog("��� Player�� ��Ʈ���� ����!");
                        for(int i=0;i<4;i++)
                        {
                            if (!DataManager.Instance.PartyFormation[i].isDead)
                            {
                                DataManager.Instance.PartyFormation[i].heroStress += 20;
                                yield return StartCoroutine(CheckCorruptionOrCourage(i));
                            }
                        }
                    }
                }
                else // ������ ������ �ƴϴ�
                {
                    // �氨���� ���� ������ �氨
                    //int realDmg = randomDmg * 100 *
                    //    ((100 - DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100) / 100;

                    float realFloatDmg = ((float)randomDmg);
                    realFloatDmg *= ((100f - (float)DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - (float)DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100f);
                    int realDmg = Convert.ToInt32(Math.Truncate(realFloatDmg));

                    //float realFloatDmg = ((float)randomDmg / 2) * 100 *
                    //        ((100 - DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100) / 100;
                    //int realDmg = (int)Math.Ceiling(realFloatDmg);

                    DataManager.Instance.PartyFormation[HeroDmged].heroHp -= realDmg;
                    StartCoroutine(ShowDamageText("<color=\"red\">" + realDmg.ToString() + "</color>", false, HeroDmged, false, false));
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + realDmg.ToString() + " ��ŭ�� ������");
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� �����Դϴ�.");
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                    }
                }
            }
        }
        else // �������� ���ݸ� �ִ� ��� �� ������ ��Ʈ������ ���� ��ų, ���ܷ� ����۴� ��� ���� ���� ��Ʈ������ ��
        {
            // �� ������ ����
            int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
            // �������� ���� �Ʊ� ���� ����
            int HeroDmged;
            while (true)
            {
                HeroDmged = UnityEngine.Random.Range(0, 4);
                if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                    continue;
                else
                    break;
            }

            ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "(��)�� " + "Player" + (HeroDmged + 1).ToString() + "���� ����!");
            destination = new Vector3(-1, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.localPosition.y, 0);
            moveObject = BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).gameObject;
            origin = moveObject.transform.localPosition;
            //yield return new WaitForSecondsRealtime(2f);
            yield return waitForAttack;

            // �Ʊ��� ȸ������ Ȯ���ؼ� ȸ�� Ȯ��
            int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
            int randomDodge = UnityEngine.Random.Range(0, 101);
            if (HeroDodgeRate >= randomDodge)
            {
                // ȸ�� ����
                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                StartCoroutine(ShowDamageText("ȸ��", false, HeroDmged, false, true));
            }
            else // ����
            {
                int randomCritRate = UnityEngine.Random.Range(0, 101);
                if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                {
                    // ġ��Ÿ ����
                    ShowBattleLog("<color=\"red\">ġ��Ÿ!</color>");
                    Vector3 pos = BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.position
                        + BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).transform.position; // �����ڿ� �ǰ����� ���� ��տ� ��ġ��ų ����
                    pos.x /= 2;
                    pos.y /= 2; // ��� ��
                    pos.z = BattleEventCamera.transform.position.z;
                    BattleEventCamera.transform.position = pos;
                    BattleEventCamera.SetActive(true);
                    randomDmg *= 2;
                    moveTime -= 1f; // movetoward�� �̵����� ģ���� ��� ����

                    StartCoroutine(ShowDamageText("<color=\"yellow\">ġ��Ÿ!</color>", false, HeroDmged, false, true)); // ��Ʈ���� �������� ���� �ؽ�Ʈ

                    //yield return new WaitForSecondsRealtime(1.5f);
                    yield return waitForCritical;
                    BattleEventCamera.SetActive(false);
                }

                if (DataManager.Instance.EnemyFormation[CurEnemy].enemyClass == EnemyClassName.Debuffer) // ��� �������� ��Ʈ����
                {
                    DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg * 2;
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg * 2).ToString() + " ��ŭ�� ��Ʈ����");
                    StartCoroutine(ShowDamageText((randomDmg * 2).ToString(), false, HeroDmged, false, false)); // ��Ʈ���� �������� ���� �ؽ�Ʈ
                    // �ر� �� �������� Ȯ�������� �߰� ��Ʈ����
                    for (int i = 0; i < 4; i++) 
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if(randomStress < 25) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg).ToString() + " ��ŭ�� �߰� ��Ʈ����");
                                //yield return new WaitForSecondsRealtime(2f);

                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = CorruptionSerifu[UnityEngine.Random.Range(0, 4)];
                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                //yield return new WaitForSecondsRealtime(2.5f);
                                yield return waitForSerifu;//new WaitForSecondsRealtime(2.5f);
                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                    //
                    // ���� �� ������ ��Ʈ���� �ǰ� �������� ����
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 30) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "�� \"������\" ȿ�� �ߵ�!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress -= randomDmg;
                                if (DataManager.Instance.PartyFormation[HeroDmged].heroStress < 0)
                                    DataManager.Instance.PartyFormation[HeroDmged].heroStress = 0;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg).ToString() + " ��ŭ�� ��Ʈ���� ȸ��");
                                //yield return new WaitForSecondsRealtime(2f);

                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.white;
                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = CourageSerifu[UnityEngine.Random.Range(0, 4)];
                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                //yield return new WaitForSecondsRealtime(2.5f);
                                yield return waitForSerifu;//new WaitForSecondsRealtime(2.5f);
                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                    //

                    // ��Ʈ������ 200�� �ʰ� -> ü���� 0���� ����� ������ ����
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� <color=\"red\">���帶��</color>�� <color=\"red\">���</color>�߽��ϴ�.");
                            HeroLeft--;
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("��� Player�� ��Ʈ���� ����!");
                            for (int i = 0; i < 4; i++)
                            {
                                if (!DataManager.Instance.PartyFormation[i].isDead)
                                {
                                    DataManager.Instance.PartyFormation[i].heroStress += 20;
                                    yield return StartCoroutine(CheckCorruptionOrCourage(i));
                                }
                            }
                        }
                        else
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ �ر��˴ϴ�! ü���� 0�� �˴ϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                    // �ر�/���� ���°� �ƴϸ� ó������ 100�� �ʰ�
                    else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ����ް� �ֽ��ϴ�.");

                        //yield return new WaitForSecondsRealtime(3f);
                        yield return waitForCorruption;

                        if (UnityEngine.Random.Range(1, 101) > 25)
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"red\">������!</color>");
                        }
                        else
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"yellow\">������!</color>");
                        }
                    }
                }
                else
                {
                    // �氨���� ���� ������ ����
                    //float realFloatDmg = ((float)randomDmg / 2) * 100 *
                    //        ((100 - DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100) / 100;
                    //int realDmg = (int)Math.Ceiling(realFloatDmg);
                    float realFloatDmg = ((float)randomDmg / 2f);
                    realFloatDmg *= ((100f - (float)DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - (float)DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100f);
                    int realDmg = Convert.ToInt32(Math.Truncate(realFloatDmg));

                    if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // ���� ������ ����
                    {
                        int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                            + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // ���� ������ �ϰ� ��Ƽ�� ����
                        int randomDeath = UnityEngine.Random.Range(0, 101);
                        if (randomDeathDoor >= randomDeath) // ���ڰ� �� ũ�� => ���߳´�
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���߳½��ϴ�. " + randomDeathDoor.ToString() + " / " + randomDeath.ToString());
                            DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // ���������� �׹� Ȯ�� ����
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� ������ �����ϴ�. ��Ʈ���� 20 ����");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress += 20;
                            yield return StartCoroutine(CheckCorruptionOrCourage(HeroDmged));
                        }
                        else // ���߳��� ���ߴ�
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� <color=\"red\">���</color>�߽��ϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            HeroLeft--;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("��� Player�� ��Ʈ���� ����!");
                            for (int i = 0; i < 4; i++)
                            {
                                if (!DataManager.Instance.PartyFormation[i].isDead)
                                {
                                    DataManager.Instance.PartyFormation[i].heroStress += 20;
                                    yield return StartCoroutine(CheckCorruptionOrCourage(i));
                                }
                            }
                        }
                    }
                    else // ������ ������ �ƴϴ�
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp -= realDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + realDmg.ToString() + " ��ŭ�� ������");
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� �����Դϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }

                    DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + randomDmg.ToString() + " ��ŭ�� ��Ʈ����");
                    StartCoroutine(ShowDamageText("<color=\"red\">" + realDmg.ToString() + "</color>\n" + randomDmg.ToString(), false, HeroDmged, false, false));

                    // �ر� �� �������� Ȯ�������� �߰� ��Ʈ����
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 25) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg / 2;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg / 2).ToString() + " ��ŭ�� �߰� ��Ʈ����");
                                //yield return new WaitForSecondsRealtime(2f);

                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = CorruptionSerifu[UnityEngine.Random.Range(0, 4)];
                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                //yield return new WaitForSecondsRealtime(2.5f);
                                yield return waitForSerifu;
                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                    //
                    // ���� �� ������ ��Ʈ���� �ǰ� �������� ����
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 40) // 30%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"yellow\">\"������\"</color> ȿ�� �ߵ�!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress -= randomDmg / 2;
                                if (DataManager.Instance.PartyFormation[HeroDmged].heroStress < 0)
                                    DataManager.Instance.PartyFormation[HeroDmged].heroStress = 0;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg / 2).ToString() + " ��ŭ�� ��Ʈ���� ȸ��");
                                //yield return new WaitForSecondsRealtime(2f);

                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.white;
                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = CourageSerifu[UnityEngine.Random.Range(0, 4)];
                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                //yield return new WaitForSecondsRealtime(2.5f);
                                yield return waitForSerifu;
                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                    //

                    // ��Ʈ������ 200�� �ʰ� -> ü���� 0���� ����� ������ ����
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� <color=\"red\">���帶��</color>�� <color=\"red\">���</color>�߽��ϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            HeroLeft--;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("��� Player�� ��Ʈ���� ����!");
                            for (int i = 0; i < 4; i++)
                            {
                                if (!DataManager.Instance.PartyFormation[i].isDead)
                                {
                                    DataManager.Instance.PartyFormation[i].heroStress += 20;
                                    yield return StartCoroutine(CheckCorruptionOrCourage(i));
                                }
                            }
                        }
                        else
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ �ر��˴ϴ�! ü���� 0�� �˴ϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                    // �ر�/���� ���°� �ƴϸ� ó������ 100�� �ʰ�
                    else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ����ް� �ֽ��ϴ�.");

                        //yield return new WaitForSecondsRealtime(3f);
                        yield return waitForCorruption;

                        if (UnityEngine.Random.Range(1, 101) > 25)
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"red\">������!</color>");
                        }
                        else
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"yellow\">������!</color>");
                        }

                    }
                    // �� ��� ���� �ȵǴ� ���¸� �׳� �Ѿ
                }
            }
        }

        //yield return new WaitForSecondsRealtime(3f);
        yield return waitForTurnEnd;//new WaitForSecondsRealtime(3f);
        SpeedOrder.Dequeue();
        nextBattleOrder = true;
    }

    IEnumerator BattlePhase_Hero()
    {
        Tuple<int, int> WhoseTurn = SpeedOrder.Peek();
        /*if(WhoseTurn.Item1 >= 10) // ����
        {
            CurEnemy = WhoseTurn.Item1 - 10;
            // ������ ��ų�� �ϳ� �����ϰ� ����
            int randomSkill = UnityEngine.Random.Range(0, 2); // 1�� or 2�� ��ų
            if(randomSkill == 0) // ������ �������� �ִ� ��ų
            {
                // �� ������ ����
                int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
                // �������� ���� �Ʊ� ���� ����
                int HeroDmged;
                while (true)
                {
                    HeroDmged = UnityEngine.Random.Range(0, 4);
                    if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                        continue;
                    else
                        break;
                }

                ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "�� ����!");
                yield return new WaitForSeconds(1f);

                // �Ʊ��� ȸ������ Ȯ���ؼ� ȸ�� Ȯ��
                int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate 
                    + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
                int randomDodge = UnityEngine.Random.Range(0, 101);
                if(HeroDodgeRate >= randomDodge)
                {
                    // ȸ�� ����
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                }
                else // ����
                {
                    int randomCritRate = UnityEngine.Random.Range(0, 101);
                    if(DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                    {
                        // ġ��Ÿ ����
                        ShowBattleLog("ġ��Ÿ!");
                        randomDmg *= 2;
                    }
                    if(DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // ���� ������ ����
                    {
                        int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                            + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // ���� ������ �ϰ� ��Ƽ�� ����
                        int randomDeath = UnityEngine.Random.Range(0, 101);
                        if(randomDeathDoor >= randomDeath) // ���ڰ� �� ũ�� => ���߳´�
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���߳½��ϴ�. " + randomDeath.ToString() + " / " + randomDeath.ToString());
                            DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // ���������� �׹� Ȯ�� ����
                        }
                        else // ���߳��� ���ߴ�
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ����߽��ϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                    else // ������ ������ �ƴϴ�
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp -= randomDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + randomDmg.ToString() + " ��ŭ�� ������");
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� �����Դϴ�.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                }
            }
            else // �������� ���ݸ� �ִ� ��� �� ������ ��Ʈ������ ���� ��ų, ���ܷ� ����۴� ��� ���� ���� ��Ʈ������ ��
            {
                // �� ������ ����
                int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
                // �������� ���� �Ʊ� ���� ����
                int HeroDmged;
                while (true)
                {
                    HeroDmged = UnityEngine.Random.Range(0, 4);
                    if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                        continue;
                    else
                        break;
                }

                ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "�� ����!");
                yield return new WaitForSeconds(1f);

                // �Ʊ��� ȸ������ Ȯ���ؼ� ȸ�� Ȯ��
                int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                    + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
                int randomDodge = UnityEngine.Random.Range(0, 101);
                if (HeroDodgeRate >= randomDodge)
                {
                    // ȸ�� ����
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                }
                else // ����
                {
                    int randomCritRate = UnityEngine.Random.Range(0, 101);
                    if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                    {
                        // ġ��Ÿ ����
                        ShowBattleLog("ġ��Ÿ!");
                        randomDmg *= 2;
                    }

                    if (DataManager.Instance.EnemyFormation[CurEnemy].enemyClass == EnemyClassName.Debuffer) // ��� �������� ��Ʈ����
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + randomDmg.ToString() + " ��ŭ�� ��Ʈ����");

                        // ��Ʈ������ 200�� �ʰ� -> ü���� 0���� ����� ������ ����
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                        {
                            if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ���帶��� ����߽��ϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ �ر��˴ϴ�! ü���� 0�� �˴ϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }
                        // �ر�/���� ���°� �ƴϸ� ó������ 100�� �ʰ�
                        else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ����ް� �ֽ��ϴ�.");

                            yield return new WaitForSeconds(1.5f);

                            if (UnityEngine.Random.Range(1, 101) > 25)
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : ������!");
                            }
                            else
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : ������!");
                            }
                        }
                    }
                    else
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // ���� ������ ����
                        {
                            int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // ���� ������ �ϰ� ��Ƽ�� ����
                            int randomDeath = UnityEngine.Random.Range(0, 101);
                            if (randomDeathDoor >= randomDeath) // ���ڰ� �� ũ�� => ���߳´�
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���߳½��ϴ�. " + randomDeath.ToString() + " / " + randomDeath.ToString());
                                DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // ���������� �׹� Ȯ�� ����
                            }
                            else // ���߳��� ���ߴ�
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ����߽��ϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                        }
                        else // ������ ������ �ƴϴ�
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp -= randomDmg / 2;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + (randomDmg / 2).ToString() + " ��ŭ�� ������");
                            if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ������ ���� �����Դϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }

                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg / 2;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "���� " + randomDmg.ToString() + " ��ŭ�� ��Ʈ����");
                        // ��Ʈ������ 200�� �ʰ� -> ü���� 0���� ����� ������ ����
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                        {
                            if(DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(��)�� ���帶��� ����߽��ϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ �ر��˴ϴ�! ü���� 0�� �˴ϴ�.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }
                        // �ر�/���� ���°� �ƴϸ� ó������ 100�� �ʰ�
                        else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "�� ������ ����ް� �ֽ��ϴ�.");

                            yield return new WaitForSeconds(1.5f);

                            if (UnityEngine.Random.Range(1, 101) > 25)
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : ������!");
                            }
                            else
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : ������!");
                            }

                        }
                        // �� ��� ���� �ȵǴ� ���¸� �׳� �Ѿ
                    }
                }
            }

            yield return new WaitForSeconds(1f);
            nextBattleOrder = true;
        }*/
        //else
        {
            CurHero = WhoseTurn.Item1;
            ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " �� ����!");
            BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).Find("Indicator").gameObject.SetActive(true);
            while (true)
            {
                yield return null;
                if (firstBtnClicked) // ù��° ��ų ��ư�� ���ȴ��� Ȯ�� -> ���� ��ų
                {
                    if (firstEnemyClicked || secondEnemyClicked || thirdEnemyClicked || fourthEnemyClicked) // ����� �����ߴ��� Ȯ��
                    {
                        if (DataManager.Instance.PartyFormation[CurHero].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[CurHero].isDead)
                        {
                            if (UnityEngine.Random.Range(0, 100) < 10) // 10%
                            {
                                ShowBattleLog("Player" + (CurHero + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                ShowBattleLog("Player" + (CurHero + 1).ToString() + "�� ���� �Ѿ�ϴ�!");
                                Serifu[CurHero].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                Serifu[CurHero].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "���� ����~";
                                Serifu[CurHero].transform.Find("Image").gameObject.SetActive(true);

                                //yield return new WaitForSecondsRealtime(2f);
                                yield return waitForSerifu;
                                Serifu[CurHero].transform.Find("Image").gameObject.SetActive(false);;

                                break;
                            }
                        }

                        if (firstEnemyClicked)
                            SelectedEnemy = 0;
                        else if (secondEnemyClicked)
                            SelectedEnemy = 1;
                        else if (thirdEnemyClicked)
                            SelectedEnemy = 2;
                        else if (fourthEnemyClicked)
                            SelectedEnemy = 3;

                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " �� ����!");
                        SkillIcons.SetActive(false);
                        destination = new Vector3(1, BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.localPosition.y, 0);
                        moveObject = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).gameObject;
                        origin = moveObject.transform.localPosition;
                        //yield return new WaitForSecondsRealtime(2f);
                        yield return waitForAttack;

                        int dodgeRate = DataManager.Instance.EnemyFormation[SelectedEnemy].BasicDodgeRate; // �ش� ���� ȸ���� ��������
                        int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // �Ʊ��� ���߷� ��������
                        if (DataManager.Instance.tempStats[CurHero] != null)
                            hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // ���� (��)���� �����ֱ�
                        int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                        int randomHit = UnityEngine.Random.Range(0, 101);  // Ÿ�ݿ� �����ߴ��� Ȯ��
                        if (totalRate >= randomHit) // ������
                        {
                            int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // �ּ� ~ �ִ� ������ ������ ������ �� ��������
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // ������ ������ �ִٸ� �����ֱ�

                            if (dealingDmg <= 0)
                                dealingDmg = 1;

                            int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // ũ��Ƽ�� ���� Ȯ���ϱ�
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // ũ�� ������ �ִٸ� �����ֱ�
                            randomHit = UnityEngine.Random.Range(0, 101);
                            if (critRate >= randomHit) // ũ�� Ȯ������ �Ʒ� ��, �� ���ԵǴ� ���� ���´ٸ� ũ��Ƽ��
                            {
                                ShowBattleLog("<color=\"red\">ġ��Ÿ!</color>");
                                //Vector3 pos = BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.position;
                                Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.position; // �����ڿ� �ǰ����� ���� ��տ� ��ġ��ų ����
                                pos.x /= 2;
                                pos.y /= 2; // ��� ��
                                pos.z = BattleEventCamera.transform.position.z;
                                BattleEventCamera.transform.position = pos;
                                BattleEventCamera.SetActive(true);
                                dealingDmg *= 2;
                                moveTime -= 1f; // movetoward�� �̵����� ģ���� ��� ����
                                StartCoroutine(ShowDamageText("<color=\"yellow\">ġ��Ÿ!</color>", true, SelectedEnemy, false, true));
                                //yield return new WaitForSecondsRealtime(1.5f);
                                yield return waitForCritical;
                                BattleEventCamera.SetActive(false);
                            }

                            // �ش�Ǵ� ���� ü�� ����
                            DataManager.Instance.EnemyFormation[SelectedEnemy].Hp -= dealingDmg;
                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                            StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, SelectedEnemy, false, false));

                            #region ������ ���� �� ȿ�� �ߵ�
                            for (int i=0;i<4;i++)
                            {
                                if(i != CurHero && DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // ������ ���� �ٸ� �̰� ������ ȿ���� ��
                                {
                                    if(UnityEngine.Random.Range(0, 100) < 40) // 20%
                                    {
                                        ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"yellow\">\"������\"</color> ȿ�� �ߵ�!");
                                        ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "���� " + (dealingDmg / 2).ToString() + "��ŭ�� �߰� ������");
                                        StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, SelectedEnemy, true, false));
                                        DataManager.Instance.EnemyFormation[SelectedEnemy].Hp -= dealingDmg / 2;
                                        break; // �ѹ��� ����
                                    }
                                }
                            }
                            #endregion
                            if (DataManager.Instance.EnemyFormation[SelectedEnemy].Hp <= 0) // ���� �׾����� üũ
                            {
                                DataManager.Instance.EnemyFormation[SelectedEnemy].isDead = true;
                                Destroy(BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.GetChild(1).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // �� ������ ����� ��ü
                                ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "�� ���ݿ� ���� <color=\"red\">���</color>�߽��ϴ�.");
                                BattleCanvas.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).gameObject.SetActive(false);
                                EnemyLeft--; // �� �� ����
                            }
                            else // ���� �ʾҴٸ� �α׸� ���
                            {
                                //ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                            }
                            // �� �ѱ�
                            break;
                        }
                        else // ���� ��
                        {
                            // �α� ���
                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + totalRate.ToString() + " / " + randomHit.ToString());
                            StartCoroutine(ShowDamageText("ȸ��", true, SelectedEnemy, false, true));
                            // �� �� �ѱ�
                            break;
                        }
                    }
                    else
                        continue;
                }
                else if (secondBtnClicked) // ��ų ��ư�� ���ȴ��� Ȯ�� -> �������� or ���� ����
                {
                    if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Melee) // �� �γ��� ������ ������
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " �� ����!");
                        SkillIcons.SetActive(false);
                        destination = new Vector3(1, -1.5f, 0);
                        moveObject = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).gameObject;
                        origin = moveObject.transform.localPosition;
                        //yield return new WaitForSecondsRealtime(2f);
                        yield return waitForAttack;

                        for (int i = 0; i < 2; i++)
                        {
                            if (DataManager.Instance.EnemyFormation[i].isDead)
                                continue;
                            int dodgeRate = DataManager.Instance.EnemyFormation[i].BasicDodgeRate; // �ش� ���� ȸ���� ��������
                            int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // �Ʊ��� ���߷� ��������
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // ���� (��)���� �����ֱ�
                            int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                            int randomHit = UnityEngine.Random.Range(0, 101);  // Ÿ�ݿ� �����ߴ��� Ȯ��
                            if (totalRate >= randomHit) // ������
                            {
                                int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                    DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // �ּ� ~ �ִ� ������ ������ ������ �� ��������
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // ������ ������ �ִٸ� �����ֱ�

                                if (dealingDmg <= 0)
                                    dealingDmg = 1;

                                // �������̹Ƿ� �������� ���ݸ� ���� (�ø�)
                                dealingDmg *= 5;
                                if (dealingDmg % 10 != 0) // �������� �ִ� -> 10�� �ڸ��� �ø���
                                {
                                    dealingDmg /= 10; dealingDmg++;
                                }
                                else
                                    dealingDmg /= 10;

                                int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // ũ��Ƽ�� ���� Ȯ���ϱ�
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // ũ�� ������ �ִٸ� �����ֱ�
                                randomHit = UnityEngine.Random.Range(0, 101);
                                if (critRate >= randomHit) // ũ�� Ȯ������ �Ʒ� ��, �� ���ԵǴ� ���� ���´ٸ� ũ��Ƽ��
                                {
                                    ShowBattleLog("<color=\"red\">ġ��Ÿ!</color>");
                                    //Vector3 pos = BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position;
                                    Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position; // �����ڿ� �ǰ����� ���� ��տ� ��ġ��ų ����
                                    pos.x /= 2;
                                    pos.y /= 2; // ��� ��
                                    pos.z = BattleEventCamera.transform.position.z;
                                    BattleEventCamera.transform.position = pos;
                                    BattleEventCamera.SetActive(true);
                                    dealingDmg *= 2;
                                    moveTime -= 1f; // movetoward�� �̵����� ģ���� ��� ����
                                    StartCoroutine(ShowDamageText("<color=\"yellow\">ġ��Ÿ!</color>", true, i, false, true));
                                    //yield return new WaitForSecondsRealtime(1.5f);
                                    yield return waitForCritical;
                                    BattleEventCamera.SetActive(false);
                                }

                                // �ش�Ǵ� ���� ü�� ����
                                DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg;
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                                StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, i, false, false));
                                #region ������ ���� �� ȿ�� �ߵ�
                                for (int j = 0; j < 4; j++)
                                {
                                    if (j != CurHero && DataManager.Instance.PartyFormation[j].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // ������ ���� �ٸ� �̰� ������ ȿ���� ��
                                    {
                                        if (UnityEngine.Random.Range(0, 100) < 40) // 20%
                                        {
                                            ShowBattleLog("Player" + (j + 1).ToString() + "�� <color=\"yellow\">\"������\"</color> ȿ�� �ߵ�!");
                                            ShowBattleLog("Enemy" + (i + 1).ToString() + "���� " + (dealingDmg / 2).ToString() + "��ŭ�� �߰� ������");
                                            StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, i, true, false));
                                            DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg / 2;
                                            break; // �ѹ��� ����
                                        }
                                    }
                                }
                                #endregion

                                if (DataManager.Instance.EnemyFormation[i].Hp <= 0) // ���� �׾����� üũ
                                {
                                    DataManager.Instance.EnemyFormation[i].isDead = true;
                                    Destroy(BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.GetChild(1).gameObject);
                                    Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // �� ������ ����� ��ü
                                    ShowBattleLog("Enemy" + (i + 1).ToString() + "�� ���ݿ� ���� <color=\"red\">���</color>�߽��ϴ�.");
                                    BattleCanvas.transform.Find("Enemy" + (i + 1).ToString()).gameObject.SetActive(false);
                                    EnemyLeft--; // �� �� ����
                                }
                                else // ���� �ʾҴٸ� �α׸� ���
                                {
                                    //ShowBattleLog("Enemy" + (i + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                                }
                                // �� �ѱ�
                            }
                            else // ���� ��
                            {
                                // �α� ���
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + totalRate.ToString() + " / " + randomHit.ToString());
                                StartCoroutine(ShowDamageText("ȸ��", true, i, false, true));
                                // �� �� �ѱ�
                            }
                        }
                        break;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Marksman)
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " �� ����!");
                        SkillIcons.SetActive(false);
                        destination = new Vector3(1, 4.5f, 0);
                        moveObject = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).gameObject;
                        origin = moveObject.transform.localPosition;
                        //yield return new WaitForSecondsRealtime(2f);
                        yield return waitForAttack;

                        for (int i = 2; i < 4; i++)
                        {
                            if (DataManager.Instance.EnemyFormation[i].isDead)
                                continue;
                            int dodgeRate = DataManager.Instance.EnemyFormation[i].BasicDodgeRate; // �ش� ���� ȸ���� ��������
                            int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // �Ʊ��� ���߷� ��������
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // ���� (��)���� �����ֱ�
                            int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                            int randomHit = UnityEngine.Random.Range(0, 101);  // Ÿ�ݿ� �����ߴ��� Ȯ��
                            if (totalRate >= randomHit) // ������
                            {
                                int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                    DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // �ּ� ~ �ִ� ������ ������ ������ �� ��������
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // ������ ������ �ִٸ� �����ֱ�

                                if (dealingDmg <= 0)
                                    dealingDmg = 1;

                                // �������̹Ƿ� �������� ���ݸ� ���� (�ø�)
                                dealingDmg *= 5;
                                if (dealingDmg % 10 != 0) // �������� �ִ� -> 10�� �ڸ��� �ø���
                                {
                                    dealingDmg /= 10; dealingDmg++;
                                }
                                else
                                    dealingDmg /= 10;

                                int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // ũ��Ƽ�� ���� Ȯ���ϱ�
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // ũ�� ������ �ִٸ� �����ֱ�
                                randomHit = UnityEngine.Random.Range(0, 101);
                                if (critRate >= randomHit) // ũ�� Ȯ������ �Ʒ� ��, �� ���ԵǴ� ���� ���´ٸ� ũ��Ƽ��
                                {
                                    ShowBattleLog("<color=\"red\">ġ��Ÿ!</color>");
                                    //Vector3 pos = BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position;
                                    Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position; // �����ڿ� �ǰ����� ���� ��տ� ��ġ��ų ����
                                    pos.x /= 2;
                                    pos.y /= 2; // ��� ��
                                    pos.z = BattleEventCamera.transform.position.z;
                                    BattleEventCamera.transform.position = pos;
                                    BattleEventCamera.SetActive(true);
                                    dealingDmg *= 2;
                                    moveTime -= 1f; // movetoward�� �̵����� ģ���� ��� ����
                                    StartCoroutine(ShowDamageText("<color=\"yellow\">ġ��Ÿ!</color>", true, i, false, true));
                                    //yield return new WaitForSecondsRealtime(1.5f);
                                    yield return waitForCritical;
                                    BattleEventCamera.SetActive(false);
                                }

                                // �ش�Ǵ� ���� ü�� ����
                                DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg;
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                                StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, i, false, false));
                                #region ������ ���� �� ȿ�� �ߵ�
                                for (int j = 0; j < 4; j++)
                                {
                                    if (j != CurHero && DataManager.Instance.PartyFormation[j].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // ������ ���� �ٸ� �̰� ������ ȿ���� ��
                                    {
                                        if (UnityEngine.Random.Range(0, 100) < 40) // 20%
                                        {
                                            ShowBattleLog("Player" + (j + 1).ToString() + "�� <color=\"yellow\">\"������\"</color> ȿ�� �ߵ�!");
                                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "���� " + (dealingDmg / 2).ToString() + "��ŭ�� �߰� ������");
                                            StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, i, true, false));
                                            DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg / 2;
                                            break; // �ѹ��� ����
                                        }
                                    }
                                }
                                #endregion

                                if (DataManager.Instance.EnemyFormation[i].Hp <= 0) // ���� �׾����� üũ
                                {
                                    DataManager.Instance.EnemyFormation[i].isDead = true;
                                    Destroy(BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.GetChild(1).gameObject);
                                    Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // �� ������ ����� ��ü
                                    ShowBattleLog("Enemy" + (i + 1).ToString() + "�� ���ݿ� ���� <color=\"red\">���</color>�߽��ϴ�.");
                                    BattleCanvas.transform.Find("Enemy" + (i + 1).ToString()).gameObject.SetActive(false);
                                    EnemyLeft--; // �� �� ����
                                }
                                else // ���� �ʾҴٸ� �α׸� ���
                                {
                                    //ShowBattleLog("Enemy" + (i + 1).ToString() + "���� " + dealingDmg.ToString() + "��ŭ�� ������");
                                }
                                // �� �ѱ�
                            }
                            else // ���� ��
                            {
                                // �α� ���
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "�� ������ ȸ���߽��ϴ�. " + totalRate.ToString() + " / " + randomHit.ToString());
                                StartCoroutine(ShowDamageText("ȸ��", true, i, false, true));
                                // �� �� �ѱ�
                            }
                        }
                        break;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Tanker)
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " �� �氨 ����!");
                        SkillIcons.SetActive(false);
                        for (int i = 0; i < 4; i++)
                        {
                            if (DataManager.Instance.PartyFormation[i].heroBasicProtection != 20) // ������ ���� ���¿��ٸ�
                                DataManager.Instance.PartyFormation[i].heroBasicProtection = 20; // ���� �߰�
                            DataManager.Instance.PartyFormation[i].heroBuffRemain = 2; // ���� Ƚ�� 2��
                        }
                        break;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Healer)
                    {
                        if(firstHeroClicked || secondHeroClicked || thirdHeroClicked || fourthHeroClicked)
                        {
                            for(int i=0;i<4;i++)
                            {
                                if (!DataManager.Instance.PartyFormation[i].isDead)
                                {
                                    if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                                    {
                                        bool sameHero = false;
                                        if(UnityEngine.Random.Range(0, 100) < 10) // 10%
                                        {
                                            switch (i)
                                            {
                                                case 0:
                                                    if(firstHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        firstHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        firstHeroClicked = true;
                                                    }
                                                    break;
                                                case 1:
                                                    if(secondHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        secondHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        secondHeroClicked = true;
                                                    }
                                                    break;
                                                case 2:
                                                    if (thirdHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        thirdHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        thirdHeroClicked = true;
                                                    }
                                                    break;
                                                case 3:
                                                    if (fourthHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        fourthHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        fourthHeroClicked = true;
                                                    }
                                                    break;
                                            }

                                            if(sameHero)
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(��)�� ȸ���� �ź��մϴ�!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "�ʿ� ����!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                            else
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(��)�� ȸ���� ����é�ϴ�!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "�װ� ������!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (firstHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[0].isDead)
                                {
                                    firstHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).minHealAmount,
                                    ((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).maxHealAmount + 1);
                                DataManager.Instance.PartyFormation[0].heroHp += randomHeal;
                                if (DataManager.Instance.PartyFormation[0].heroHp > DataManager.Instance.PartyFormation[0].heroMaxHP)
                                    DataManager.Instance.PartyFormation[0].heroHp = DataManager.Instance.PartyFormation[0].heroMaxHP;
                                SkillIcons.SetActive(false);
                                ShowBattleLog("Player1�� " + randomHeal.ToString() + "��ŭ�� ü�� ȸ��");
                                break;
                            }
                            else if (secondHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[1].isDead)
                                {
                                    secondHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).minHealAmount,
                                    ((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).maxHealAmount + 1);
                                DataManager.Instance.PartyFormation[1].heroHp += randomHeal;
                                if (DataManager.Instance.PartyFormation[1].heroHp > DataManager.Instance.PartyFormation[1].heroMaxHP)
                                    DataManager.Instance.PartyFormation[1].heroHp = DataManager.Instance.PartyFormation[1].heroMaxHP;
                                ShowBattleLog("Player2�� " + randomHeal.ToString() + "��ŭ�� ü�� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else if (thirdHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[2].isDead)
                                {
                                    thirdHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).minHealAmount,
                                    ((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).maxHealAmount + 1);
                                DataManager.Instance.PartyFormation[2].heroHp += randomHeal;
                                if (DataManager.Instance.PartyFormation[2].heroHp > DataManager.Instance.PartyFormation[2].heroMaxHP)
                                    DataManager.Instance.PartyFormation[2].heroHp = DataManager.Instance.PartyFormation[2].heroMaxHP;
                                ShowBattleLog("Player3�� " + randomHeal.ToString() + "��ŭ�� ü�� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else if (fourthHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[3].isDead)
                                {
                                    fourthHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).minHealAmount,
                                    ((HealerSetting)(DataManager.Instance.PartyFormation[CurHero])).maxHealAmount + 1);
                                DataManager.Instance.PartyFormation[3].heroHp += randomHeal;
                                if (DataManager.Instance.PartyFormation[3].heroHp > DataManager.Instance.PartyFormation[3].heroMaxHP)
                                    DataManager.Instance.PartyFormation[3].heroHp = DataManager.Instance.PartyFormation[3].heroMaxHP;
                                ShowBattleLog("Player4�� " + randomHeal.ToString() + "��ŭ�� ü�� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else
                                break;
                        }
                        else
                            continue;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Supporter)
                    {
                        if (firstHeroClicked || secondHeroClicked || thirdHeroClicked || fourthHeroClicked)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (!DataManager.Instance.PartyFormation[i].isDead)
                                {
                                    if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                                    {
                                        bool sameHero = false;
                                        if (UnityEngine.Random.Range(0, 100) < 10) // 10%
                                        {
                                            switch (i)
                                            {
                                                case 0:
                                                    if (firstHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        firstHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        firstHeroClicked = true;
                                                    }
                                                    break;
                                                case 1:
                                                    if (secondHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        secondHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        secondHeroClicked = true;
                                                    }
                                                    break;
                                                case 2:
                                                    if (thirdHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        thirdHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        thirdHeroClicked = true;
                                                    }
                                                    break;
                                                case 3:
                                                    if (fourthHeroClicked)
                                                    {
                                                        sameHero = true;
                                                        fourthHeroClicked = false;
                                                    }
                                                    else
                                                    {
                                                        firstHeroClicked = secondHeroClicked = thirdHeroClicked = fourthHeroClicked = false;
                                                        fourthHeroClicked = true;
                                                    }
                                                    break;
                                            }

                                            if (sameHero)
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(��)�� ȸ���� �ź��մϴ�!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "�ʿ� ����!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                            else
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "�� <color=\"red\">\"������\"</color> ȿ�� �ߵ�!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(��)�� ȸ���� ����é�ϴ�!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "�װ� ������!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (firstHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[0].isDead)
                                {
                                    firstHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).minStressDownAmount,
                                    ((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).maxStressDownAmount + 1);
                                DataManager.Instance.PartyFormation[0].heroStress -= randomHeal;
                                if (DataManager.Instance.PartyFormation[0].heroStress <= 0)
                                {
                                    DataManager.Instance.PartyFormation[0].heroStress = 0;
                                    // ���� �ر����¸� �⺻ ���·� �ǵ���
                                    if (DataManager.Instance.PartyFormation[0].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[0].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player1�� " + randomHeal.ToString() + "��ŭ�� ��Ʈ���� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else if (secondHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[1].isDead)
                                {
                                    secondHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).minStressDownAmount,
                                    ((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).maxStressDownAmount + 1);
                                DataManager.Instance.PartyFormation[1].heroStress -= randomHeal;
                                if (DataManager.Instance.PartyFormation[1].heroStress <= 0)
                                {
                                    DataManager.Instance.PartyFormation[1].heroStress = 0;
                                    // ���� �ر����¸� �⺻ ���·� �ǵ���
                                    if (DataManager.Instance.PartyFormation[1].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[1].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player2�� " + randomHeal.ToString() + "��ŭ�� ��Ʈ���� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else if (thirdHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[2].isDead)
                                {
                                    thirdHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).minStressDownAmount,
                                    ((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).maxStressDownAmount + 1);
                                DataManager.Instance.PartyFormation[2].heroStress -= randomHeal;
                                if (DataManager.Instance.PartyFormation[2].heroStress <= 0)
                                {
                                    DataManager.Instance.PartyFormation[2].heroStress = 0;
                                    // ���� �ر����¸� �⺻ ���·� �ǵ���
                                    if (DataManager.Instance.PartyFormation[2].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[2].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player3�� " + randomHeal.ToString() + "��ŭ�� ��Ʈ���� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else if (fourthHeroClicked)
                            {
                                if (DataManager.Instance.PartyFormation[3].isDead)
                                {
                                    fourthHeroClicked = false;
                                    continue;
                                }
                                int randomHeal = UnityEngine.Random.Range(((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).minStressDownAmount,
                                    ((SupporterSetting)(DataManager.Instance.PartyFormation[CurHero])).maxStressDownAmount + 1);
                                DataManager.Instance.PartyFormation[3].heroStress -= randomHeal;
                                if (DataManager.Instance.PartyFormation[3].heroStress <= 0)
                                {
                                    DataManager.Instance.PartyFormation[3].heroStress = 0;
                                    // ���� �ر����¸� �⺻ ���·� �ǵ���
                                    if (DataManager.Instance.PartyFormation[3].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[3].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player4�� " + randomHeal.ToString() + "��ŭ�� ��Ʈ���� ȸ��");
                                SkillIcons.SetActive(false);
                                break;
                            }
                            else
                                break;
                        }
                        else
                            continue;
                    }
                }
                else
                    continue;
            }

            //yield return new WaitForSecondsRealtime(2f);
            yield return waitForTurnEnd;
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).Find("Indicator").gameObject.SetActive(false);
            SpeedOrder.Dequeue();
            nextBattleOrder = true;
        }
    }

    IEnumerator BattleSetting()
    {
        BattleCanvas.SetActive(true);
        BattleCanvas.transform.Find("BattleLog").GetComponent<TextMeshProUGUI>().text = "";
        BattleStart();
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 0f;
        battleStart = true;
        StartCoroutine("SetNextBattleOrder");
    }
    
    IEnumerator SetNextBattleOrder()
    {
        yield return new WaitForSecondsRealtime(1f);
        nextBattleOrder = true;
        StartCoroutine("SetBattleING");
    }

    IEnumerator SetBattleING()
    {
        yield return new WaitForSecondsRealtime(1f);
        DataManager.Instance.battle_ing = true;
    }

    void CheckAttackOrder()
    {
        int speed;
        int randomSpeed;
        for (int i=0;i<4;i++)
        {
            if (!DataManager.Instance.PartyFormation[i].isDead)
            {
                speed = DataManager.Instance.PartyFormation[i].heroBasicSpeed;
                if (DataManager.Instance.tempStats[i] != null)
                    speed += DataManager.Instance.tempStats[i].tempSpeed;

                randomSpeed = UnityEngine.Random.Range(speed - 5, speed + 6);
                SpeedOrder.Enqueue(new Tuple<int, int>(i, randomSpeed));
            }
            if (!DataManager.Instance.EnemyFormation[i].isDead)
            {
                speed = DataManager.Instance.EnemyFormation[i].BasicSpeed;
                randomSpeed = UnityEngine.Random.Range(speed - 5, speed + 6);
                SpeedOrder.Enqueue(new Tuple<int, int>(10 + i, randomSpeed));
            }
        }
        shouldCheckingOrder = false;
    }

    void BattleStart()
    {
        GameObject Hero;

        if (GameObject.Find("BattleScene").transform.Find("Player1").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Player1").GetChild(2).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Player2").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Player2").GetChild(2).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Player3").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Player3").GetChild(2).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Player4").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Player4").GetChild(2).gameObject);

        if (DataManager.Instance.PartyFormation[0].isDead)
        {
            Hero = Instantiate(Tomb, GameObject.Find("BattleScene").transform.Find("Player1"));
            Hero.transform.localPosition = new Vector3(0, 0, 0);
            BattleCanvas.transform.Find("Player1").gameObject.SetActive(false);
        }
        else
        {
            switch (DataManager.Instance.PartyFormation[0].heroClass)
            {
                case ClassName.Marksman:
                    Hero = Instantiate(Marksman, GameObject.Find("BattleScene").transform.Find("Player1"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Melee:
                    Hero = Instantiate(Melee, GameObject.Find("BattleScene").transform.Find("Player1"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Tanker:
                    Hero = Instantiate(Tanker, GameObject.Find("BattleScene").transform.Find("Player1"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Healer:
                    Hero = Instantiate(Healer, GameObject.Find("BattleScene").transform.Find("Player1"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Supporter:
                    Hero = Instantiate(Supporter, GameObject.Find("BattleScene").transform.Find("Player1"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
            }
        }

        if (DataManager.Instance.PartyFormation[1].isDead)
        {
            Hero = Instantiate(Tomb, GameObject.Find("BattleScene").transform.Find("Player2"));
            Hero.transform.localPosition = new Vector3(0, 0, 0);
            BattleCanvas.transform.Find("Player2").gameObject.SetActive(false);
        }
        else
        {
            switch (DataManager.Instance.PartyFormation[1].heroClass)
            {
                case ClassName.Marksman:
                    Hero = Instantiate(Marksman, GameObject.Find("BattleScene").transform.Find("Player2"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Melee:
                    Hero = Instantiate(Melee, GameObject.Find("BattleScene").transform.Find("Player2"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Tanker:
                    Hero = Instantiate(Tanker, GameObject.Find("BattleScene").transform.Find("Player2"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Healer:
                    Hero = Instantiate(Healer, GameObject.Find("BattleScene").transform.Find("Player2"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Supporter:
                    Hero = Instantiate(Supporter, GameObject.Find("BattleScene").transform.Find("Player2"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
            }
        }

        if (DataManager.Instance.PartyFormation[2].isDead)
        {
            Hero = Instantiate(Tomb, GameObject.Find("BattleScene").transform.Find("Player3"));
            Hero.transform.localPosition = new Vector3(0, 0, 0);
            BattleCanvas.transform.Find("Player3").gameObject.SetActive(false);
        }
        else
        {
            switch (DataManager.Instance.PartyFormation[2].heroClass)
            {
                case ClassName.Marksman:
                    Hero = Instantiate(Marksman, GameObject.Find("BattleScene").transform.Find("Player3"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Melee:
                    Hero = Instantiate(Melee, GameObject.Find("BattleScene").transform.Find("Player3"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Tanker:
                    Hero = Instantiate(Tanker, GameObject.Find("BattleScene").transform.Find("Player3"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Healer:
                    Hero = Instantiate(Healer, GameObject.Find("BattleScene").transform.Find("Player3"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Supporter:
                    Hero = Instantiate(Supporter, GameObject.Find("BattleScene").transform.Find("Player3"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
            }
        }

        if (DataManager.Instance.PartyFormation[3].isDead)
        {
            Hero = Instantiate(Tomb, GameObject.Find("BattleScene").transform.Find("Player4"));
            Hero.transform.localPosition = new Vector3(0, 0, 0);
            BattleCanvas.transform.Find("Player4").gameObject.SetActive(false);
        }
        else
        {
            switch (DataManager.Instance.PartyFormation[3].heroClass)
            {
                case ClassName.Marksman:
                    Hero = Instantiate(Marksman, GameObject.Find("BattleScene").transform.Find("Player4"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Melee:
                    Hero = Instantiate(Melee, GameObject.Find("BattleScene").transform.Find("Player4"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Tanker:
                    Hero = Instantiate(Tanker, GameObject.Find("BattleScene").transform.Find("Player4"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Healer:
                    Hero = Instantiate(Healer, GameObject.Find("BattleScene").transform.Find("Player4"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                case ClassName.Supporter:
                    Hero = Instantiate(Supporter, GameObject.Find("BattleScene").transform.Find("Player4"));
                    Hero.transform.localPosition = new Vector3(0, 0, 0);
                    break;
            }
        }

        // DD -> �� ���� ���� �����
        if (GameObject.Find("BattleScene").transform.Find("Enemy1").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Enemy1").GetChild(1).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Enemy2").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Enemy2").GetChild(1).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Enemy3").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Enemy3").GetChild(1).gameObject);
        if (GameObject.Find("BattleScene").transform.Find("Enemy4").childCount != 0)
            Destroy(GameObject.Find("BattleScene").transform.Find("Enemy4").GetChild(1).gameObject);

        int upgradeCount = DataManager.Instance.StageLevel;
        DataManager.Instance.EnemyFormation = new BasicEnemySetting[4] { null, null, null, null };

        for(int i=0; i<4;i++)
        {
            int randomEnemy = UnityEngine.Random.Range(0, 20);
            if(randomEnemy <= 5) // Melee
            {
                int randomUpgrade = UnityEngine.Random.Range(0, upgradeCount + 1);
                upgradeCount -= randomUpgrade;
                MeleeEnemySetting meleeEnemy = new MeleeEnemySetting(randomUpgrade);
                DataManager.Instance.EnemyFormation[i] = meleeEnemy;
                Instantiate(EnemyPrefabs[0], GameObject.Find("BattleScene").transform.Find("Enemy" + (i + 1).ToString())).transform.localPosition = new Vector3(0, 0, 0);
            }
            else if(randomEnemy > 5 && randomEnemy <= 11) // Marksman
            {
                int randomUpgrade = UnityEngine.Random.Range(0, upgradeCount + 1);
                upgradeCount -= randomUpgrade;
                MarksmanEnemySetting MakrsEnemy = new MarksmanEnemySetting(randomUpgrade);
                DataManager.Instance.EnemyFormation[i] = MakrsEnemy;
                Instantiate(EnemyPrefabs[1], GameObject.Find("BattleScene").transform.Find("Enemy" + (i + 1).ToString())).transform.localPosition = new Vector3(0, 0, 0);
            }
            else if(randomEnemy > 11 && randomEnemy <= 18) // Debuffer
            {
                int randomUpgrade = UnityEngine.Random.Range(0, upgradeCount + 1);
                upgradeCount -= randomUpgrade;
                DebufferEnemySetting debufferEnemy = new DebufferEnemySetting(randomUpgrade);
                DataManager.Instance.EnemyFormation[i] = debufferEnemy;
                Instantiate(EnemyPrefabs[2], GameObject.Find("BattleScene").transform.Find("Enemy" + (i + 1).ToString())).transform.localPosition = new Vector3(0, 0, 0);
            }
            else // Named 19
            {
                int randomUpgrade = UnityEngine.Random.Range(0, upgradeCount + 1);
                upgradeCount -= randomUpgrade;
                NamedEnemySetting NamedEnemy = new NamedEnemySetting(randomUpgrade);
                DataManager.Instance.EnemyFormation[i] = NamedEnemy;
                Instantiate(EnemyPrefabs[3], GameObject.Find("BattleScene").transform.Find("Enemy" + (i + 1).ToString())).transform.localPosition = new Vector3(0, 0, 0);
                NamedEnemyNum++;
            }
        }

        BattleCanvas.transform.Find("TurnCount").GetComponent<TextMeshProUGUI>().text = "���� �� : " + TurnCount.ToString();
    }

    public void OnPlayerButtonClicked(Button selectedButton) // ��� �ӽ� ����
    {
        string name = selectedButton.name;
        switch(name)
        {
            case "Player1":
                Destroy(BattleScene.transform.Find("Player1").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player1").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player1�� ���ݿ� ���� ����߽��ϴ�.");
                DataManager.Instance.PartyFormation[0].isDead = true;
                HeroLeft--;
                break;
            case "Player2":
                Destroy(BattleScene.transform.Find("Player2").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player2").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player2�� ���ݿ� ���� ����߽��ϴ�.");
                DataManager.Instance.PartyFormation[1].isDead = true;
                HeroLeft--;
                break;
            case "Player3":
                Destroy(BattleScene.transform.Find("Player3").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player3").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player3�� ���ݿ� ���� ����߽��ϴ�.");
                DataManager.Instance.PartyFormation[2].isDead = true;
                HeroLeft--;
                break;
            case "Player4":
                Destroy(BattleScene.transform.Find("Player4").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player4").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player4�� ���ݿ� ���� ����߽��ϴ�.");
                DataManager.Instance.PartyFormation[3].isDead = true;
                HeroLeft--;
                break;

            case "Enemy1":
                Destroy(BattleScene.transform.Find("Enemy1").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy1").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy1�� ���ݿ� ���� ����߽��ϴ�.");
                EnemyLeft--;
                break;
            case "Enemy2":
                Destroy(BattleScene.transform.Find("Enemy2").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy2").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy2�� ���ݿ� ���� ����߽��ϴ�.");
                EnemyLeft--;
                break;
            case "Enemy3":
                Destroy(BattleScene.transform.Find("Enemy3").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy3").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy3�� ���ݿ� ���� ����߽��ϴ�.");
                EnemyLeft--;
                break;
            case "Enemy4":
                Destroy(BattleScene.transform.Find("Enemy4").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy4").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy4�� ���ݿ� ���� ����߽��ϴ�.");
                EnemyLeft--;
                break;
        }
    }

    void ShowBattleLog(string log)
    {
        BattleLog.Enqueue(log);
        if(BattleLog.Count > MaxLog)
        {
            BattleLog.Dequeue();
        }
        UpdateLogText();
    }

    void UpdateLogText()
    {
        BattleCanvas.transform.Find("BattleLog").GetComponent<TextMeshProUGUI>().text
            = string.Join("\n\n", BattleLog.ToArray());
    }

    #region ��ư Ŭ��
    public void OnFirstEnemyBtnClicked()
    {
        SelectedEnemy = 0;
        firstEnemyClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    public void OnSecondEnemyBtnClicked()
    {
        SelectedEnemy = 1;
        secondEnemyClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    public void OnThirdEnemyBtnClicked()
    {
        SelectedEnemy = 2;
        thirdEnemyClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    public void OnFourEnemyBtnClicked()
    {
        SelectedEnemy = 3;
        fourthEnemyClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnFirstHeroBtnClicked()
    {
        SelectedHero = 0;
        firstHeroClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnSecondHeroBtnClicked()
    {
        SelectedHero = 1;
        secondHeroClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnThirdHeroBtnClicked()
    {
        SelectedHero = 2;
        thirdHeroClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnFourHeroBtnClicked()
    {
        SelectedHero = 3;
        fourthHeroClicked = true;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
    public void OnFirstSkillBtnClicked(Button SelectedButton)
    {
        firstEnemyClicked = firstHeroClicked = secondEnemyClicked = secondHeroClicked =
            thirdEnemyClicked = thirdHeroClicked = fourthHeroClicked = fourthEnemyClicked = false; // ��ų���� �����ϵ��� �� �� �ʱ�ȭ
        if (firstBtnClicked)
        {
            firstBtnClicked = false;
        }
        else
        {
            secondBtnClicked = false;
            firstBtnClicked = true;
            //EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    public void OnSecondSkillBtnClicked(Button SelectedButton)
    {
        firstEnemyClicked = firstHeroClicked = secondEnemyClicked = secondHeroClicked =
            thirdEnemyClicked = thirdHeroClicked = fourthHeroClicked = fourthEnemyClicked = false; // ��ų���� �����ϵ��� �� �� �ʱ�ȭ
        if (secondBtnClicked)
        {
            secondBtnClicked = false;
        }
        else
        {
            firstBtnClicked = false;
            secondBtnClicked = true;
            if(SpeedOrder.Peek().Item1 < 10)
            {
                if(DataManager.Instance.PartyFormation[SpeedOrder.Peek().Item1].heroClass == ClassName.Melee ||
                    DataManager.Instance.PartyFormation[SpeedOrder.Peek().Item1].heroClass == ClassName.Marksman ||
                    DataManager.Instance.PartyFormation[SpeedOrder.Peek().Item1].heroClass == ClassName.Tanker)
                    EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            }
        }
    }
    #endregion

    IEnumerator CheckCorruptionOrCourage(int HeroNum)
    {
        if (!DataManager.Instance.PartyFormation[HeroNum].isDead) // ������ �ֵ鸸 üũ
        {
            if (DataManager.Instance.PartyFormation[HeroNum].heroStress >= 200)
            {
                if (DataManager.Instance.PartyFormation[HeroNum].heroHp == 0)
                {
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + "(��)�� ���帶��� <color=\"red\">���</color>�߽��ϴ�.");
                    HeroLeft--;
                    DataManager.Instance.PartyFormation[HeroNum].isDead = true;
                    BattleCanvas.transform.Find("Player" + (HeroNum + 1).ToString()).gameObject.SetActive(false);
                    Destroy(BattleScene.transform.Find("Player" + (HeroNum + 1).ToString()).transform.GetChild(2).gameObject);
                    Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroNum + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                    ShowBattleLog("��� Player�� ��Ʈ���� ����!");
                    yield return new WaitForSecondsRealtime(1.5f);
                    for (int i = 0; i < 4; i++)
                    {
                        if (!DataManager.Instance.PartyFormation[i].isDead)
                        {
                            DataManager.Instance.PartyFormation[i].heroStress += 20;
                            yield return StartCoroutine(CheckCorruptionOrCourage(i));
                        }
                    }
                }
                else
                {
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + "�� ������ �ر��˴ϴ�! ü���� 0�� �˴ϴ�.");
                    DataManager.Instance.PartyFormation[HeroNum].heroStress = 150;
                    DataManager.Instance.PartyFormation[HeroNum].heroHp = 0;
                    yield return new WaitForSecondsRealtime(1.5f);
                }
            }
            // �ر�/���� ���°� �ƴϸ� ó������ 100�� �ʰ�
            else if (DataManager.Instance.PartyFormation[HeroNum].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroNum].Stress == Stress.Default)
            {
                ShowBattleLog("Player" + (HeroNum + 1).ToString() + "�� ������ ����ް� �ֽ��ϴ�.");

                //yield return new WaitForSecondsRealtime(3f);
                yield return waitForCorruption;

                if (UnityEngine.Random.Range(1, 101) > 25)
                {
                    DataManager.Instance.PartyFormation[HeroNum].Stress = Stress.Negative;
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + " : <color=\"red\">������!</color>");
                }
                else
                {
                    DataManager.Instance.PartyFormation[HeroNum].Stress = Stress.Positive;
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + " : <color=\"yellow\">������!</color>");
                }
            }
        }
    }

    IEnumerator ShowDamageText(string text, bool isEnemy, int CharacterNum, bool isAdditionalDmg, bool isJustText)
    {
        yield return null;
        if(isEnemy) // ������ ���̴�
        {
            GameObject DmgText = Instantiate(DamageTextPrefab, BattleScene.transform.Find("Enemy" + (CharacterNum + 1).ToString()).transform);
            DmgText.transform.localPosition = Vector3.zero;
            DmgText.transform.Find("Canvas_Dmg").transform.Find("DmgText").GetComponent<TextMeshProUGUI>().text = text;
            DmgText.transform.localPosition = Vector3.zero;
            if(isAdditionalDmg)
            {
                DmgText.transform.localPosition += new Vector3(-0.8f, 1.5f, 0);
            }
            else
            {
                if (isJustText)
                {
                    DmgText.transform.localPosition += new Vector3(-0.8f, 1f, 0);
                }
                else
                {
                    DmgText.transform.localPosition += new Vector3(-0.8f, 0.5f, 0);
                }
            }
            StartCoroutine(MoveDamageText(DmgText));
        }
        else
        {
            GameObject DmgText = Instantiate(DamageTextPrefab, BattleScene.transform.Find("Player" + (CharacterNum + 1).ToString()).transform);
            DmgText.transform.localPosition = Vector3.zero;
            DmgText.transform.Find("Canvas_Dmg").transform.Find("DmgText").GetComponent<TextMeshProUGUI>().text = text;
            DmgText.transform.localPosition = Vector3.zero;
            if (isAdditionalDmg)
            {
                DmgText.transform.localPosition += new Vector3(0.8f, 1.5f, 0);
            }
            else
            {
                if (isJustText)
                {
                    DmgText.transform.localPosition += new Vector3(0.8f, 1f, 0);
                }
                else
                {
                    DmgText.transform.localPosition += new Vector3(0.8f, 0.5f, 0);
                }
            }
            StartCoroutine(MoveDamageText(DmgText));
        }
    }

    IEnumerator MoveDamageText(GameObject DmgText) 
    {
        float deltaT = 0f;
        Color c;
        while (deltaT < 1f)
        {
            yield return null;
            c = DmgText.transform.Find("Canvas_Dmg").transform.Find("DmgText").GetComponent<TextMeshProUGUI>().color;
            c.a -= Time.unscaledDeltaTime;
            if(c.a <= 0f)
            {
                Destroy(DmgText);
                break;
            }
            DmgText.transform.Find("Canvas_Dmg").transform.Find("DmgText").GetComponent<TextMeshProUGUI>().color = c; // ���������� ���İ��� �ٿ�����
        }
    }
}
