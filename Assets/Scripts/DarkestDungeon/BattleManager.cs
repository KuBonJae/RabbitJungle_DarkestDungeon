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

    private string[] CorruptionSerifu = new string[4] { "꼴 좋다!", "우린 망했어", "멍청아!", "장난해?" };
    private string[] CourageSerifu = new string[4] { "힘 내!", "할 수 있어", "도와줄게!", "아직이야!" };

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
        nextBattleOrder = false; // 맨 처음 시작할때 한번 적용
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
            if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative) // 스테이지 넘어갈 때 부정적이면 적용될 일?
            {
                if (DataManager.Instance.PartyFormation[i].heroStress <= 100)
                    DataManager.Instance.PartyFormation[i].heroStress += 20; // 스트레스 100 이하면 20 추가
            }
            else if(DataManager.Instance.PartyFormation[i].Stress == Stress.Positive) // 스테이지 넘어갈 때 긍정적이면 적용될 일?
            {
                if (DataManager.Instance.PartyFormation[i].heroStress >= 150) // 스트레스가 150 이상이면
                {
                    DataManager.Instance.PartyFormation[i].heroStress = 0; // 스트레스 초기화
                    DataManager.Instance.PartyFormation[i].Stress = Stress.Default; // 영웅의 각성 초기화 시켜버리기
                }
                else // 스트레스 관리를 잘 했다면
                {
                    for(int j=0;j<4;j++)
                    {
                        if (i == j)
                            continue; // 본인은 제외

                        DataManager.Instance.PartyFormation[j].heroStress -= 10; // 아군들 스트레스 10씩 줄여줌
                        if (DataManager.Instance.PartyFormation[j].heroStress <= 0)
                            DataManager.Instance.PartyFormation[j].heroStress = 0;
                        DataManager.Instance.PartyFormation[j].heroHp += 5; // 아군들 체력 회복 5씩
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

        if (BattleCanvas.activeSelf && EnemyLeft == 0) // 배틀 진행 중인데 남은 적이 0이면 배틀을 종료
        {
            // 배틀 페이즈를 종료하며, 보상을 획득하고 계속 진행함
            EnemyLeft = 4;
            TurnCount = 0;
            DataManager.Instance.battle_ing = false; // end the battle phase

            BattleCanvas.transform.Find("Player1").gameObject.SetActive(true); // 혹시 숨겨둔 버튼들을 미리 다 제자리로 돌려두기
            BattleCanvas.transform.Find("Player2").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Player3").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Player4").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy1").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy2").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy3").gameObject.SetActive(true);
            BattleCanvas.transform.Find("Enemy4").gameObject.SetActive(true);


            firstBtnClicked = false;
            secondBtnClicked = false;
            nextBattleOrder = false; // 어차피 BattleSetting에서 적용시켜주니 그냥 안전하게 false로 전환
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
                DataManager.Instance.tempStats[i] = new Stat(0, 0, 0, 0, 0, 0, 0); // 임시 버프 초기화
            }

            int coinAmount = 200 + UnityEngine.Random.Range(150, 300 + 1) + NamedEnemyNum * 100; // 최소 300, 최대 500 + 네임드몹 수 * 100;
            NamedEnemyNum = 0; // 리셋
            DataManager.Instance.coin += coinAmount;
            string coinText = "코인 " + coinAmount.ToString() + " 획득!";
            if(UnityEngine.Random.Range(0, 100) < 5) // 5% 확률로
            {
                DataManager.Instance.coin += coinAmount; // 코인 한번 더 제공
                coinText = "코인 보너스! X 2배!\n코인 " + (coinAmount * 2).ToString() + " 획득!";
            }
            DataManager.Instance.announcement = coinText;
            DataManager.Instance.makeAnnouncement = true;
            BattleCanvas.SetActive(false);
            Time.timeScale = 1f;
        }

        if (DataManager.Instance.battleEvent) // 배틀 이벤트 발생 -> 배틀 세팅
        {
            DataManager.Instance.battleEvent = false;
            while (BattleLog.Count > 0)
                BattleLog.Dequeue();
            SkillIcons.SetActive(false);
            StartCoroutine("BattleSetting");
        }
        
        if(HeroLeft == 0) // 아군이 0이면 게임 종료
        {
            SceneManager.LoadScene("GameOver");
        }

        if(TurnEnd || battleStart) // 배틀의 첫 시작 or 턴이 끝났다면 순서 재배치
        {
            battleStart = false;
            TurnEnd = false;
            BattleCanvas.transform.Find("TurnCount").GetComponent<TextMeshProUGUI>().text = "현재 턴 : " + (++TurnCount).ToString();
            for(int i=0;i<4;i++)
            {
                if (DataManager.Instance.PartyFormation[i].heroBasicProtection != 0) // 탱커가 준 보호 버프가 존재한다면
                {
                    DataManager.Instance.PartyFormation[i].heroBuffRemain--; // 버프 유지 횟수 감소
                    if (DataManager.Instance.PartyFormation[i].heroBuffRemain < 0)
                        DataManager.Instance.PartyFormation[i].heroBasicProtection = 0;
                }
            }
            shouldCheckingOrder = true;
            while (SpeedOrder.Count > 0)
                SpeedOrder.Dequeue(); // 전 전투에서 쓰던 값이 있다면 미리 다 비울 것
            CheckAttackOrder();
        }

        if (DataManager.Instance.battle_ing) // 현재 배틀 진행중
        {
            Time.timeScale = 0f; // 전투중일땐 항시 0으로 유지

            if (SpeedOrder.Count == 0 && !shouldCheckingOrder)
                TurnEnd = true;
            else
            {
                if (nextBattleOrder)
                {
                    nextBattleOrder = false;

                    firstBtnClicked = false; // 모든 버튼들 선택 해제
                    secondBtnClicked = false;
                    firstEnemyClicked = false;
                    secondEnemyClicked = false;
                    thirdEnemyClicked = false;
                    fourthEnemyClicked = false;
                    firstHeroClicked = false;
                    secondHeroClicked = false;
                    thirdHeroClicked = false;
                    fourthHeroClicked = false;

                    if (SpeedOrder.Peek().Item1 >= 10) // 적군이면
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
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "공격";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "E1, E2\n동시 공격";
                                    break;
                                case ClassName.Marksman:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "공격";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "E3, E4\n동시 공격";
                                    break;
                                case ClassName.Tanker:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "공격";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "데미지 경감\n버프";
                                    break;
                                case ClassName.Healer:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "공격";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "치유";
                                    break;
                                case ClassName.Supporter:
                                    Skills[0].GetComponentInChildren<TextMeshProUGUI>().text = "공격";
                                    Skills[1].GetComponentInChildren<TextMeshProUGUI>().text = "스트레스\n감소";
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
            //TextMeshProUGUI TEXT = BattleCanvas.transform.Find("Player" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>(); // 기존, UI Canvas
            TextMeshProUGUI TEXT = BattleScene.transform.Find("Player" + (i + 1).ToString()).transform.Find("Canvas_P" + (i + 1).ToString()).transform.Find("HpStress").GetComponent<TextMeshProUGUI>(); // 신규, 게임 오브젝트에 붙인 Canvas
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
        // 적군의 스킬을 하나 랜덤하게 선택
        int randomSkill = UnityEngine.Random.Range(0, 2); // 1번 or 2번 스킬
        if (randomSkill == 0) // 오로지 데미지만 주는 스킬
        {
            // 줄 데미지 선택
            int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
            // 데미지를 받을 아군 랜덤 선택
            int HeroDmged;
            while (true)
            {
                HeroDmged = UnityEngine.Random.Range(0, 4);
                if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                    continue;
                else
                    break;
            }

            ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "(이)가 " + "Player" + (HeroDmged + 1).ToString() + "에게 공격!");
            destination = new Vector3(-1, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.localPosition.y, 0);
            moveObject = BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).gameObject;
            origin = moveObject.transform.localPosition;
            //yield return new WaitForSecondsRealtime(2f);
            yield return waitForAttack;//new WaitForSecondsRealtime(2f); // 시간 줄이기
            // 아군의 회피율을 확인해서 회피 확인
            int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
            int randomDodge = UnityEngine.Random.Range(0, 101);
            if (HeroDodgeRate >= randomDodge)
            {
                // 회피 성공
                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 공격을 회피했습니다. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                StartCoroutine(ShowDamageText("회피", false, HeroDmged, false, true));
            }
            else // 맞음
            {
                int randomCritRate = UnityEngine.Random.Range(0, 101);
                if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                {
                    // 치명타 터짐
                    ShowBattleLog("<color=\"red\">치명타!</color>");
                    Vector3 pos = BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.position 
                        + BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).transform.position; // 공격자와 피격자의 값의 평균에 위치시킬 예정
                    pos.x /= 2;
                    pos.y /= 2; // 평균 값
                    pos.z = BattleEventCamera.transform.position.z;
                    BattleEventCamera.transform.position = pos;
                    BattleEventCamera.SetActive(true);
                    randomDmg *= 2;
                    moveTime -= 1f; // movetoward로 이동중인 친구를 잠시 늦춤
                    StartCoroutine(ShowDamageText("<color=\"yellow\">치명타!</color>", false, HeroDmged, false, true));
                    //yield return new WaitForSecondsRealtime(1.5f);
                    yield return waitForCritical;
                    BattleEventCamera.SetActive(false);
                }
                if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // 현재 죽음의 문턱
                {
                    int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                        + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // 현재 죽음의 일격 버티는 정도
                    int randomDeath = UnityEngine.Random.Range(0, 101);
                    if (randomDeathDoor >= randomDeath) // 숫자가 더 크다 => 버텨냈다
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음을 버텨냈습니다. " + randomDeathDoor.ToString() + " / " + randomDeath.ToString());
                        DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // 영구적으로 죽문 확률 감소
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음에 대한 공포를 느낍니다. 스트레스 20 증가");
                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += 20;
                        yield return StartCoroutine(CheckCorruptionOrCourage(HeroDmged));
                    }
                    else // 버텨내지 못했다
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 <color=\"red\">사망</color>했습니다.");
                        DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                        HeroLeft--;
                        BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                        Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                        Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                        ShowBattleLog("모든 Player의 스트레스 증가!");
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
                else // 죽음의 문턱은 아니다
                {
                    // 경감률에 따라 데미지 경감
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
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + realDmg.ToString() + " 만큼의 데미지");
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음의 문턱 상태입니다.");
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                    }
                }
            }
        }
        else // 데미지의 절반만 주는 대신 그 절반이 스트레스로 들어가는 스킬, 예외로 디버퍼는 모든 값이 전부 스트레스로 들어감
        {
            // 줄 데미지 선택
            int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
            // 데미지를 받을 아군 랜덤 선택
            int HeroDmged;
            while (true)
            {
                HeroDmged = UnityEngine.Random.Range(0, 4);
                if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                    continue;
                else
                    break;
            }

            ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "(이)가 " + "Player" + (HeroDmged + 1).ToString() + "에게 공격!");
            destination = new Vector3(-1, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.localPosition.y, 0);
            moveObject = BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).gameObject;
            origin = moveObject.transform.localPosition;
            //yield return new WaitForSecondsRealtime(2f);
            yield return waitForAttack;

            // 아군의 회피율을 확인해서 회피 확인
            int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
            int randomDodge = UnityEngine.Random.Range(0, 101);
            if (HeroDodgeRate >= randomDodge)
            {
                // 회피 성공
                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 공격을 회피했습니다. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                StartCoroutine(ShowDamageText("회피", false, HeroDmged, false, true));
            }
            else // 맞음
            {
                int randomCritRate = UnityEngine.Random.Range(0, 101);
                if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                {
                    // 치명타 터짐
                    ShowBattleLog("<color=\"red\">치명타!</color>");
                    Vector3 pos = BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.position
                        + BattleScene.transform.Find("Enemy" + (CurEnemy + 1).ToString()).transform.position; // 공격자와 피격자의 값의 평균에 위치시킬 예정
                    pos.x /= 2;
                    pos.y /= 2; // 평균 값
                    pos.z = BattleEventCamera.transform.position.z;
                    BattleEventCamera.transform.position = pos;
                    BattleEventCamera.SetActive(true);
                    randomDmg *= 2;
                    moveTime -= 1f; // movetoward로 이동중인 친구를 잠시 늦춤

                    StartCoroutine(ShowDamageText("<color=\"yellow\">치명타!</color>", false, HeroDmged, false, true)); // 스트레스 데미지에 대한 텍스트

                    //yield return new WaitForSecondsRealtime(1.5f);
                    yield return waitForCritical;
                    BattleEventCamera.SetActive(false);
                }

                if (DataManager.Instance.EnemyFormation[CurEnemy].enemyClass == EnemyClassName.Debuffer) // 모든 데미지가 스트레스
                {
                    DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg * 2;
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg * 2).ToString() + " 만큼의 스트레스");
                    StartCoroutine(ShowDamageText((randomDmg * 2).ToString(), false, HeroDmged, false, false)); // 스트레스 데미지에 대한 텍스트
                    // 붕괴 시 팀원에게 확률적으로 추가 스트레스
                    for (int i = 0; i < 4; i++) 
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if(randomStress < 25) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg).ToString() + " 만큼의 추가 스트레스");
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
                    // 각성 시 팀원의 스트레스 피격 절반으로 감소
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 30) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "의 \"긍정적\" 효과 발동!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress -= randomDmg;
                                if (DataManager.Instance.PartyFormation[HeroDmged].heroStress < 0)
                                    DataManager.Instance.PartyFormation[HeroDmged].heroStress = 0;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg).ToString() + " 만큼의 스트레스 회복");
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

                    // 스트레스가 200을 초과 -> 체력을 0으로 만들고 죽음의 문턱
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 <color=\"red\">심장마비</color>로 <color=\"red\">사망</color>했습니다.");
                            HeroLeft--;
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("모든 Player의 스트레스 증가!");
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
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "의 마음이 붕괴됩니다! 체력이 0이 됩니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                    // 붕괴/각성 상태가 아니며 처음으로 100을 초과
                    else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 의지를 시험받고 있습니다.");

                        //yield return new WaitForSecondsRealtime(3f);
                        yield return waitForCorruption;

                        if (UnityEngine.Random.Range(1, 101) > 25)
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"red\">부정적!</color>");
                        }
                        else
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"yellow\">긍정적!</color>");
                        }
                    }
                }
                else
                {
                    // 경감률에 따라 데미지 감소
                    //float realFloatDmg = ((float)randomDmg / 2) * 100 *
                    //        ((100 - DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100) / 100;
                    //int realDmg = (int)Math.Ceiling(realFloatDmg);
                    float realFloatDmg = ((float)randomDmg / 2f);
                    realFloatDmg *= ((100f - (float)DataManager.Instance.PartyFormation[HeroDmged].heroBasicProtection - (float)DataManager.Instance.tempStats[HeroDmged].tempProtect) / 100f);
                    int realDmg = Convert.ToInt32(Math.Truncate(realFloatDmg));

                    if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // 현재 죽음의 문턱
                    {
                        int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                            + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // 현재 죽음의 일격 버티는 정도
                        int randomDeath = UnityEngine.Random.Range(0, 101);
                        if (randomDeathDoor >= randomDeath) // 숫자가 더 크다 => 버텨냈다
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음을 버텨냈습니다. " + randomDeathDoor.ToString() + " / " + randomDeath.ToString());
                            DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // 영구적으로 죽문 확률 감소
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음에 대한 공포를 느낍니다. 스트레스 20 증가");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress += 20;
                            yield return StartCoroutine(CheckCorruptionOrCourage(HeroDmged));
                        }
                        else // 버텨내지 못했다
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 <color=\"red\">사망</color>했습니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            HeroLeft--;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("모든 Player의 스트레스 증가!");
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
                    else // 죽음의 문턱은 아니다
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp -= realDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + realDmg.ToString() + " 만큼의 데미지");
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음의 문턱 상태입니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }

                    DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + randomDmg.ToString() + " 만큼의 스트레스");
                    StartCoroutine(ShowDamageText("<color=\"red\">" + realDmg.ToString() + "</color>\n" + randomDmg.ToString(), false, HeroDmged, false, false));

                    // 붕괴 시 팀원에게 확률적으로 추가 스트레스
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 25) // 25%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg / 2;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg / 2).ToString() + " 만큼의 추가 스트레스");
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
                    // 각성 시 팀원의 스트레스 피격 절반으로 감소
                    for (int i = 0; i < 4; i++)
                    {
                        if (DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead)
                        {
                            int randomStress = UnityEngine.Random.Range(0, 100);
                            if (randomStress < 40) // 30%
                            {
                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"yellow\">\"긍정적\"</color> 효과 발동!");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress -= randomDmg / 2;
                                if (DataManager.Instance.PartyFormation[HeroDmged].heroStress < 0)
                                    DataManager.Instance.PartyFormation[HeroDmged].heroStress = 0;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg / 2).ToString() + " 만큼의 스트레스 회복");
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

                    // 스트레스가 200을 초과 -> 체력을 0으로 만들고 죽음의 문턱
                    if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 <color=\"red\">심장마비</color>로 <color=\"red\">사망</color>했습니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            HeroLeft--;
                            BattleCanvas.transform.Find("Player" + (HeroDmged + 1).ToString()).gameObject.SetActive(false);
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(2).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            ShowBattleLog("모든 Player의 스트레스 증가!");
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
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "의 마음이 붕괴됩니다! 체력이 0이 됩니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                    // 붕괴/각성 상태가 아니며 처음으로 100을 초과
                    else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                    {
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 의지를 시험받고 있습니다.");

                        //yield return new WaitForSecondsRealtime(3f);
                        yield return waitForCorruption;

                        if (UnityEngine.Random.Range(1, 101) > 25)
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"red\">부정적!</color>");
                        }
                        else
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : <color=\"yellow\">긍정적!</color>");
                        }

                    }
                    // 두 경우 포함 안되는 상태면 그냥 넘어감
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
        /*if(WhoseTurn.Item1 >= 10) // 적군
        {
            CurEnemy = WhoseTurn.Item1 - 10;
            // 적군의 스킬을 하나 랜덤하게 선택
            int randomSkill = UnityEngine.Random.Range(0, 2); // 1번 or 2번 스킬
            if(randomSkill == 0) // 오로지 데미지만 주는 스킬
            {
                // 줄 데미지 선택
                int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
                // 데미지를 받을 아군 랜덤 선택
                int HeroDmged;
                while (true)
                {
                    HeroDmged = UnityEngine.Random.Range(0, 4);
                    if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                        continue;
                    else
                        break;
                }

                ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "의 공격!");
                yield return new WaitForSeconds(1f);

                // 아군의 회피율을 확인해서 회피 확인
                int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate 
                    + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
                int randomDodge = UnityEngine.Random.Range(0, 101);
                if(HeroDodgeRate >= randomDodge)
                {
                    // 회피 성공
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 공격을 회피했습니다. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                }
                else // 맞음
                {
                    int randomCritRate = UnityEngine.Random.Range(0, 101);
                    if(DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                    {
                        // 치명타 터짐
                        ShowBattleLog("치명타!");
                        randomDmg *= 2;
                    }
                    if(DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // 현재 죽음의 문턱
                    {
                        int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                            + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // 현재 죽음의 일격 버티는 정도
                        int randomDeath = UnityEngine.Random.Range(0, 101);
                        if(randomDeathDoor >= randomDeath) // 숫자가 더 크다 => 버텨냈다
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음을 버텨냈습니다. " + randomDeath.ToString() + " / " + randomDeath.ToString());
                            DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // 영구적으로 죽문 확률 감소
                        }
                        else // 버텨내지 못했다
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 사망했습니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                            Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                            Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }
                    else // 죽음의 문턱은 아니다
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroHp -= randomDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + randomDmg.ToString() + " 만큼의 데미지");
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음의 문턱 상태입니다.");
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                        }
                    }
                }
            }
            else // 데미지의 절반만 주는 대신 그 절반이 스트레스로 들어가는 스킬, 예외로 디버퍼는 모든 값이 전부 스트레스로 들어감
            {
                // 줄 데미지 선택
                int randomDmg = UnityEngine.Random.Range(DataManager.Instance.EnemyFormation[CurEnemy].MinDamage, DataManager.Instance.EnemyFormation[CurEnemy].MaxDamage + 1);
                // 데미지를 받을 아군 랜덤 선택
                int HeroDmged;
                while (true)
                {
                    HeroDmged = UnityEngine.Random.Range(0, 4);
                    if (DataManager.Instance.PartyFormation[HeroDmged].isDead)
                        continue;
                    else
                        break;
                }

                ShowBattleLog("Enemy" + (CurEnemy + 1).ToString() + "의 공격!");
                yield return new WaitForSeconds(1f);

                // 아군의 회피율을 확인해서 회피 확인
                int HeroDodgeRate = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDodgeRate
                    + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDodge : 0);
                int randomDodge = UnityEngine.Random.Range(0, 101);
                if (HeroDodgeRate >= randomDodge)
                {
                    // 회피 성공
                    ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 공격을 회피했습니다. " + HeroDodgeRate.ToString() + " / " + randomDodge.ToString());
                }
                else // 맞음
                {
                    int randomCritRate = UnityEngine.Random.Range(0, 101);
                    if (DataManager.Instance.EnemyFormation[CurEnemy].BasicCriticalHit >= randomCritRate)
                    {
                        // 치명타 터짐
                        ShowBattleLog("치명타!");
                        randomDmg *= 2;
                    }

                    if (DataManager.Instance.EnemyFormation[CurEnemy].enemyClass == EnemyClassName.Debuffer) // 모든 데미지가 스트레스
                    {
                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + randomDmg.ToString() + " 만큼의 스트레스");

                        // 스트레스가 200을 초과 -> 체력을 0으로 만들고 죽음의 문턱
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                        {
                            if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 심장마비로 사망했습니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "의 마음이 붕괴됩니다! 체력이 0이 됩니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }
                        // 붕괴/각성 상태가 아니며 처음으로 100을 초과
                        else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 의지를 시험받고 있습니다.");

                            yield return new WaitForSeconds(1.5f);

                            if (UnityEngine.Random.Range(1, 101) > 25)
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : 부정적!");
                            }
                            else
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : 긍정적!");
                            }
                        }
                    }
                    else
                    {
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0) // 현재 죽음의 문턱
                        {
                            int randomDeathDoor = DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor
                                + (DataManager.Instance.tempStats[HeroDmged] != null ? DataManager.Instance.tempStats[HeroDmged].tempDD : 0); // 현재 죽음의 일격 버티는 정도
                            int randomDeath = UnityEngine.Random.Range(0, 101);
                            if (randomDeathDoor >= randomDeath) // 숫자가 더 크다 => 버텨냈다
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음을 버텨냈습니다. " + randomDeath.ToString() + " / " + randomDeath.ToString());
                                DataManager.Instance.PartyFormation[HeroDmged].heroBasicDeathDoor -= 5; // 영구적으로 죽문 확률 감소
                            }
                            else // 버텨내지 못했다
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 사망했습니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                        }
                        else // 죽음의 문턱은 아니다
                        {
                            DataManager.Instance.PartyFormation[HeroDmged].heroHp -= randomDmg / 2;
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + (randomDmg / 2).ToString() + " 만큼의 데미지");
                            if (DataManager.Instance.PartyFormation[HeroDmged].heroHp <= 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 죽음의 문턱 상태입니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }

                        DataManager.Instance.PartyFormation[HeroDmged].heroStress += randomDmg / 2;
                        ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "에게 " + randomDmg.ToString() + " 만큼의 스트레스");
                        // 스트레스가 200을 초과 -> 체력을 0으로 만들고 죽음의 문턱
                        if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 200)
                        {
                            if(DataManager.Instance.PartyFormation[HeroDmged].heroHp == 0)
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "(이)가 심장마비로 사망했습니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].isDead = true;
                                Destroy(BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform.GetChild(0).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroDmged + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "의 마음이 붕괴됩니다! 체력이 0이 됩니다.");
                                DataManager.Instance.PartyFormation[HeroDmged].heroStress = 150;
                                DataManager.Instance.PartyFormation[HeroDmged].heroHp = 0;
                            }
                        }
                        // 붕괴/각성 상태가 아니며 처음으로 100을 초과
                        else if (DataManager.Instance.PartyFormation[HeroDmged].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroDmged].Stress == Stress.Default)
                        {
                            ShowBattleLog("Player" + (HeroDmged + 1).ToString() + "가 의지를 시험받고 있습니다.");

                            yield return new WaitForSeconds(1.5f);

                            if (UnityEngine.Random.Range(1, 101) > 25)
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Negative;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : 부정적!");
                            }
                            else
                            {
                                DataManager.Instance.PartyFormation[HeroDmged].Stress = Stress.Positive;
                                ShowBattleLog("Player" + (HeroDmged + 1).ToString() + " : 긍정적!");
                            }

                        }
                        // 두 경우 포함 안되는 상태면 그냥 넘어감
                    }
                }
            }

            yield return new WaitForSeconds(1f);
            nextBattleOrder = true;
        }*/
        //else
        {
            CurHero = WhoseTurn.Item1;
            ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " 의 차례!");
            BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).Find("Indicator").gameObject.SetActive(true);
            while (true)
            {
                yield return null;
                if (firstBtnClicked) // 첫번째 스킬 버튼이 눌렸는지 확인 -> 공격 스킬
                {
                    if (firstEnemyClicked || secondEnemyClicked || thirdEnemyClicked || fourthEnemyClicked) // 대상을 선택했는지 확인
                    {
                        if (DataManager.Instance.PartyFormation[CurHero].Stress == Stress.Negative && !DataManager.Instance.PartyFormation[CurHero].isDead)
                        {
                            if (UnityEngine.Random.Range(0, 100) < 10) // 10%
                            {
                                ShowBattleLog("Player" + (CurHero + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                ShowBattleLog("Player" + (CurHero + 1).ToString() + "의 턴이 넘어갑니다!");
                                Serifu[CurHero].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                Serifu[CurHero].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "안해 안해~";
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

                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " 의 공격!");
                        SkillIcons.SetActive(false);
                        destination = new Vector3(1, BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.localPosition.y, 0);
                        moveObject = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).gameObject;
                        origin = moveObject.transform.localPosition;
                        //yield return new WaitForSecondsRealtime(2f);
                        yield return waitForAttack;

                        int dodgeRate = DataManager.Instance.EnemyFormation[SelectedEnemy].BasicDodgeRate; // 해당 적의 회피율 가져오기
                        int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // 아군의 명중률 가져오기
                        if (DataManager.Instance.tempStats[CurHero] != null)
                            hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // 명중 (디)버프 더해주기
                        int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                        int randomHit = UnityEngine.Random.Range(0, 101);  // 타격에 성공했는지 확인
                        if (totalRate >= randomHit) // 성공시
                        {
                            int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // 최소 ~ 최대 데미지 사이의 임의의 값 가져오기
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // 데미지 버프가 있다면 더해주기

                            if (dealingDmg <= 0)
                                dealingDmg = 1;

                            int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // 크리티컬 여부 확인하기
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // 크리 버프가 있다면 더해주기
                            randomHit = UnityEngine.Random.Range(0, 101);
                            if (critRate >= randomHit) // 크리 확률보다 아래 값, 즉 포함되는 값이 나온다면 크리티컬
                            {
                                ShowBattleLog("<color=\"red\">치명타!</color>");
                                //Vector3 pos = BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.position;
                                Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.position; // 공격자와 피격자의 값의 평균에 위치시킬 예정
                                pos.x /= 2;
                                pos.y /= 2; // 평균 값
                                pos.z = BattleEventCamera.transform.position.z;
                                BattleEventCamera.transform.position = pos;
                                BattleEventCamera.SetActive(true);
                                dealingDmg *= 2;
                                moveTime -= 1f; // movetoward로 이동중인 친구를 잠시 늦춤
                                StartCoroutine(ShowDamageText("<color=\"yellow\">치명타!</color>", true, SelectedEnemy, false, true));
                                //yield return new WaitForSecondsRealtime(1.5f);
                                yield return waitForCritical;
                                BattleEventCamera.SetActive(false);
                            }

                            // 해당되는 적의 체력 감소
                            DataManager.Instance.EnemyFormation[SelectedEnemy].Hp -= dealingDmg;
                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                            StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, SelectedEnemy, false, false));

                            #region 영웅의 각성 시 효과 발동
                            for (int i=0;i<4;i++)
                            {
                                if(i != CurHero && DataManager.Instance.PartyFormation[i].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // 공격자 제외 다른 이가 긍정적 효과일 때
                                {
                                    if(UnityEngine.Random.Range(0, 100) < 40) // 20%
                                    {
                                        ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"yellow\">\"긍정적\"</color> 효과 발동!");
                                        ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "에게 " + (dealingDmg / 2).ToString() + "만큼의 추가 데미지");
                                        StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, SelectedEnemy, true, false));
                                        DataManager.Instance.EnemyFormation[SelectedEnemy].Hp -= dealingDmg / 2;
                                        break; // 한번만 적용
                                    }
                                }
                            }
                            #endregion
                            if (DataManager.Instance.EnemyFormation[SelectedEnemy].Hp <= 0) // 적이 죽었는지 체크
                            {
                                DataManager.Instance.EnemyFormation[SelectedEnemy].isDead = true;
                                Destroy(BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform.GetChild(1).gameObject);
                                Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // 적 프리팹 묘비로 교체
                                ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "이 공격에 의해 <color=\"red\">사망</color>했습니다.");
                                BattleCanvas.transform.Find("Enemy" + (SelectedEnemy + 1).ToString()).gameObject.SetActive(false);
                                EnemyLeft--; // 적 수 감소
                            }
                            else // 죽지 않았다면 로그만 출력
                            {
                                //ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                            }
                            // 턴 넘김
                            break;
                        }
                        else // 실패 시
                        {
                            // 로그 출력
                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "가 공격을 회피했습니다. " + totalRate.ToString() + " / " + randomHit.ToString());
                            StartCoroutine(ShowDamageText("회피", true, SelectedEnemy, false, true));
                            // 후 턴 넘김
                            break;
                        }
                    }
                    else
                        continue;
                }
                else if (secondBtnClicked) // 스킬 버튼이 눌렸는지 확인 -> 광역공격 or 팀원 보조
                {
                    if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Melee) // 앞 두놈을 떄리는 광역기
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " 의 공격!");
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
                            int dodgeRate = DataManager.Instance.EnemyFormation[i].BasicDodgeRate; // 해당 적의 회피율 가져오기
                            int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // 아군의 명중률 가져오기
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // 명중 (디)버프 더해주기
                            int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                            int randomHit = UnityEngine.Random.Range(0, 101);  // 타격에 성공했는지 확인
                            if (totalRate >= randomHit) // 성공시
                            {
                                int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                    DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // 최소 ~ 최대 데미지 사이의 임의의 값 가져오기
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // 데미지 버프가 있다면 더해주기

                                if (dealingDmg <= 0)
                                    dealingDmg = 1;

                                // 광역기이므로 데미지는 절반만 적용 (올림)
                                dealingDmg *= 5;
                                if (dealingDmg % 10 != 0) // 나머지가 있다 -> 10의 자리를 올린다
                                {
                                    dealingDmg /= 10; dealingDmg++;
                                }
                                else
                                    dealingDmg /= 10;

                                int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // 크리티컬 여부 확인하기
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // 크리 버프가 있다면 더해주기
                                randomHit = UnityEngine.Random.Range(0, 101);
                                if (critRate >= randomHit) // 크리 확률보다 아래 값, 즉 포함되는 값이 나온다면 크리티컬
                                {
                                    ShowBattleLog("<color=\"red\">치명타!</color>");
                                    //Vector3 pos = BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position;
                                    Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position; // 공격자와 피격자의 값의 평균에 위치시킬 예정
                                    pos.x /= 2;
                                    pos.y /= 2; // 평균 값
                                    pos.z = BattleEventCamera.transform.position.z;
                                    BattleEventCamera.transform.position = pos;
                                    BattleEventCamera.SetActive(true);
                                    dealingDmg *= 2;
                                    moveTime -= 1f; // movetoward로 이동중인 친구를 잠시 늦춤
                                    StartCoroutine(ShowDamageText("<color=\"yellow\">치명타!</color>", true, i, false, true));
                                    //yield return new WaitForSecondsRealtime(1.5f);
                                    yield return waitForCritical;
                                    BattleEventCamera.SetActive(false);
                                }

                                // 해당되는 적의 체력 감소
                                DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg;
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                                StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, i, false, false));
                                #region 영웅의 각성 시 효과 발동
                                for (int j = 0; j < 4; j++)
                                {
                                    if (j != CurHero && DataManager.Instance.PartyFormation[j].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // 공격자 제외 다른 이가 긍정적 효과일 때
                                    {
                                        if (UnityEngine.Random.Range(0, 100) < 40) // 20%
                                        {
                                            ShowBattleLog("Player" + (j + 1).ToString() + "의 <color=\"yellow\">\"긍정적\"</color> 효과 발동!");
                                            ShowBattleLog("Enemy" + (i + 1).ToString() + "에게 " + (dealingDmg / 2).ToString() + "만큼의 추가 데미지");
                                            StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, i, true, false));
                                            DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg / 2;
                                            break; // 한번만 적용
                                        }
                                    }
                                }
                                #endregion

                                if (DataManager.Instance.EnemyFormation[i].Hp <= 0) // 적이 죽었는지 체크
                                {
                                    DataManager.Instance.EnemyFormation[i].isDead = true;
                                    Destroy(BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.GetChild(1).gameObject);
                                    Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // 적 프리팹 묘비로 교체
                                    ShowBattleLog("Enemy" + (i + 1).ToString() + "이 공격에 의해 <color=\"red\">사망</color>했습니다.");
                                    BattleCanvas.transform.Find("Enemy" + (i + 1).ToString()).gameObject.SetActive(false);
                                    EnemyLeft--; // 적 수 감소
                                }
                                else // 죽지 않았다면 로그만 출력
                                {
                                    //ShowBattleLog("Enemy" + (i + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                                }
                                // 턴 넘김
                            }
                            else // 실패 시
                            {
                                // 로그 출력
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "가 공격을 회피했습니다. " + totalRate.ToString() + " / " + randomHit.ToString());
                                StartCoroutine(ShowDamageText("회피", true, i, false, true));
                                // 후 턴 넘김
                            }
                        }
                        break;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Marksman)
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " 의 공격!");
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
                            int dodgeRate = DataManager.Instance.EnemyFormation[i].BasicDodgeRate; // 해당 적의 회피율 가져오기
                            int hitRate = DataManager.Instance.PartyFormation[CurHero].heroBasicAccuracy; // 아군의 명중률 가져오기
                            if (DataManager.Instance.tempStats[CurHero] != null)
                                hitRate += DataManager.Instance.tempStats[CurHero].tempAccuracy; // 명중 (디)버프 더해주기
                            int totalRate = hitRate - dodgeRate < 0 ? 0 : (hitRate - dodgeRate > 100 ? 100 : hitRate - dodgeRate);
                            int randomHit = UnityEngine.Random.Range(0, 101);  // 타격에 성공했는지 확인
                            if (totalRate >= randomHit) // 성공시
                            {
                                int dealingDmg = UnityEngine.Random.Range(DataManager.Instance.PartyFormation[CurHero].heroMinDamage,
                                    DataManager.Instance.PartyFormation[CurHero].heroMaxDamage + 1); // 최소 ~ 최대 데미지 사이의 임의의 값 가져오기
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    dealingDmg += DataManager.Instance.tempStats[CurHero].tempDmg; // 데미지 버프가 있다면 더해주기

                                if (dealingDmg <= 0)
                                    dealingDmg = 1;

                                // 광역기이므로 데미지는 절반만 적용 (올림)
                                dealingDmg *= 5;
                                if (dealingDmg % 10 != 0) // 나머지가 있다 -> 10의 자리를 올린다
                                {
                                    dealingDmg /= 10; dealingDmg++;
                                }
                                else
                                    dealingDmg /= 10;

                                int critRate = DataManager.Instance.PartyFormation[CurHero].heroBasicCriticalHit; // 크리티컬 여부 확인하기
                                if (DataManager.Instance.tempStats[CurHero] != null)
                                    critRate += DataManager.Instance.tempStats[CurHero].tempCrit; // 크리 버프가 있다면 더해주기
                                randomHit = UnityEngine.Random.Range(0, 101);
                                if (critRate >= randomHit) // 크리 확률보다 아래 값, 즉 포함되는 값이 나온다면 크리티컬
                                {
                                    ShowBattleLog("<color=\"red\">치명타!</color>");
                                    //Vector3 pos = BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position;
                                    Vector3 pos = BattleScene.transform.Find("Player" + (CurHero + 1).ToString()).transform.position
                                    + BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.position; // 공격자와 피격자의 값의 평균에 위치시킬 예정
                                    pos.x /= 2;
                                    pos.y /= 2; // 평균 값
                                    pos.z = BattleEventCamera.transform.position.z;
                                    BattleEventCamera.transform.position = pos;
                                    BattleEventCamera.SetActive(true);
                                    dealingDmg *= 2;
                                    moveTime -= 1f; // movetoward로 이동중인 친구를 잠시 늦춤
                                    StartCoroutine(ShowDamageText("<color=\"yellow\">치명타!</color>", true, i, false, true));
                                    //yield return new WaitForSecondsRealtime(1.5f);
                                    yield return waitForCritical;
                                    BattleEventCamera.SetActive(false);
                                }

                                // 해당되는 적의 체력 감소
                                DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg;
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                                StartCoroutine(ShowDamageText("<color=\"red\">" + dealingDmg.ToString() + "</color>", true, i, false, false));
                                #region 영웅의 각성 시 효과 발동
                                for (int j = 0; j < 4; j++)
                                {
                                    if (j != CurHero && DataManager.Instance.PartyFormation[j].Stress == Stress.Positive && !DataManager.Instance.PartyFormation[i].isDead) // 공격자 제외 다른 이가 긍정적 효과일 때
                                    {
                                        if (UnityEngine.Random.Range(0, 100) < 40) // 20%
                                        {
                                            ShowBattleLog("Player" + (j + 1).ToString() + "의 <color=\"yellow\">\"긍정적\"</color> 효과 발동!");
                                            ShowBattleLog("Enemy" + (SelectedEnemy + 1).ToString() + "에게 " + (dealingDmg / 2).ToString() + "만큼의 추가 데미지");
                                            StartCoroutine(ShowDamageText("<color=\"red\">" + (dealingDmg / 2).ToString() + "</color>", true, i, true, false));
                                            DataManager.Instance.EnemyFormation[i].Hp -= dealingDmg / 2;
                                            break; // 한번만 적용
                                        }
                                    }
                                }
                                #endregion

                                if (DataManager.Instance.EnemyFormation[i].Hp <= 0) // 적이 죽었는지 체크
                                {
                                    DataManager.Instance.EnemyFormation[i].isDead = true;
                                    Destroy(BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform.GetChild(1).gameObject);
                                    Instantiate(Tomb, BattleScene.transform.Find("Enemy" + (i + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0); // 적 프리팹 묘비로 교체
                                    ShowBattleLog("Enemy" + (i + 1).ToString() + "이 공격에 의해 <color=\"red\">사망</color>했습니다.");
                                    BattleCanvas.transform.Find("Enemy" + (i + 1).ToString()).gameObject.SetActive(false);
                                    EnemyLeft--; // 적 수 감소
                                }
                                else // 죽지 않았다면 로그만 출력
                                {
                                    //ShowBattleLog("Enemy" + (i + 1).ToString() + "에게 " + dealingDmg.ToString() + "만큼의 데미지");
                                }
                                // 턴 넘김
                            }
                            else // 실패 시
                            {
                                // 로그 출력
                                ShowBattleLog("Enemy" + (i + 1).ToString() + "가 공격을 회피했습니다. " + totalRate.ToString() + " / " + randomHit.ToString());
                                StartCoroutine(ShowDamageText("회피", true, i, false, true));
                                // 후 턴 넘김
                            }
                        }
                        break;
                    }
                    else if (DataManager.Instance.PartyFormation[CurHero].heroClass == ClassName.Tanker)
                    {
                        ShowBattleLog("Player" + (CurHero + 1).ToString() + " : " + DataManager.Instance.PartyFormation[CurHero].heroClass.ToString() + " 의 경감 버프!");
                        SkillIcons.SetActive(false);
                        for (int i = 0; i < 4; i++)
                        {
                            if (DataManager.Instance.PartyFormation[i].heroBasicProtection != 20) // 버프가 없는 상태였다면
                                DataManager.Instance.PartyFormation[i].heroBasicProtection = 20; // 버프 추가
                            DataManager.Instance.PartyFormation[i].heroBuffRemain = 2; // 버프 횟수 2턴
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
                                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(이)가 회복을 거부합니다!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "필요 없어!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                            else
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(이)가 회복을 가로챕니다!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "그건 내꺼야!";
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
                                ShowBattleLog("Player1이 " + randomHeal.ToString() + "만큼의 체력 회복");
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
                                ShowBattleLog("Player2이 " + randomHeal.ToString() + "만큼의 체력 회복");
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
                                ShowBattleLog("Player3이 " + randomHeal.ToString() + "만큼의 체력 회복");
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
                                ShowBattleLog("Player4이 " + randomHeal.ToString() + "만큼의 체력 회복");
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
                                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(이)가 회복을 거부합니다!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "필요 없어!";
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(true);

                                                //yield return new WaitForSecondsRealtime(2f);
                                                yield return waitForSerifu;
                                                Serifu[i].transform.Find("Image").gameObject.SetActive(false);
                                                break;
                                            }
                                            else
                                            {
                                                ShowBattleLog("Player" + (i + 1).ToString() + "의 <color=\"red\">\"부정적\"</color> 효과 발동!");
                                                ShowBattleLog("Player" + (i + 1).ToString() + "(이)가 회복을 가로챕니다!");
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().color = Color.red;
                                                Serifu[i].transform.Find("Image").transform.Find("Serif").GetComponent<TextMeshProUGUI>().text = "그건 내꺼야!";
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
                                    // 만약 붕괴상태면 기본 상태로 되돌림
                                    if (DataManager.Instance.PartyFormation[0].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[0].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player1이 " + randomHeal.ToString() + "만큼의 스트레스 회복");
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
                                    // 만약 붕괴상태면 기본 상태로 되돌림
                                    if (DataManager.Instance.PartyFormation[1].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[1].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player2이 " + randomHeal.ToString() + "만큼의 스트레스 회복");
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
                                    // 만약 붕괴상태면 기본 상태로 되돌림
                                    if (DataManager.Instance.PartyFormation[2].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[2].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player3이 " + randomHeal.ToString() + "만큼의 스트레스 회복");
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
                                    // 만약 붕괴상태면 기본 상태로 되돌림
                                    if (DataManager.Instance.PartyFormation[3].Stress == Stress.Negative)
                                        DataManager.Instance.PartyFormation[3].Stress = Stress.Default;
                                }
                                ShowBattleLog("Player4이 " + randomHeal.ToString() + "만큼의 스트레스 회복");
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

        // DD -> 적 생성 로직 만들기
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

        BattleCanvas.transform.Find("TurnCount").GetComponent<TextMeshProUGUI>().text = "현재 턴 : " + TurnCount.ToString();
    }

    public void OnPlayerButtonClicked(Button selectedButton) // 사용 임시 보류
    {
        string name = selectedButton.name;
        switch(name)
        {
            case "Player1":
                Destroy(BattleScene.transform.Find("Player1").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player1").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player1이 공격에 의해 사망했습니다.");
                DataManager.Instance.PartyFormation[0].isDead = true;
                HeroLeft--;
                break;
            case "Player2":
                Destroy(BattleScene.transform.Find("Player2").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player2").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player2이 공격에 의해 사망했습니다.");
                DataManager.Instance.PartyFormation[1].isDead = true;
                HeroLeft--;
                break;
            case "Player3":
                Destroy(BattleScene.transform.Find("Player3").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player3").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player3이 공격에 의해 사망했습니다.");
                DataManager.Instance.PartyFormation[2].isDead = true;
                HeroLeft--;
                break;
            case "Player4":
                Destroy(BattleScene.transform.Find("Player4").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Player4").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Player4이 공격에 의해 사망했습니다.");
                DataManager.Instance.PartyFormation[3].isDead = true;
                HeroLeft--;
                break;

            case "Enemy1":
                Destroy(BattleScene.transform.Find("Enemy1").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy1").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy1이 공격에 의해 사망했습니다.");
                EnemyLeft--;
                break;
            case "Enemy2":
                Destroy(BattleScene.transform.Find("Enemy2").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy2").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy2이 공격에 의해 사망했습니다.");
                EnemyLeft--;
                break;
            case "Enemy3":
                Destroy(BattleScene.transform.Find("Enemy3").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy3").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy3이 공격에 의해 사망했습니다.");
                EnemyLeft--;
                break;
            case "Enemy4":
                Destroy(BattleScene.transform.Find("Enemy4").transform.GetChild(0).gameObject);
                Instantiate(Tomb, BattleScene.transform.Find("Enemy4").transform).transform.localPosition = new Vector3(0, 0, 0);
                ShowBattleLog("Enemy4이 공격에 의해 사망했습니다.");
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

    #region 버튼 클릭
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
            thirdEnemyClicked = thirdHeroClicked = fourthHeroClicked = fourthEnemyClicked = false; // 스킬부터 선택하도록 값 다 초기화
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
            thirdEnemyClicked = thirdHeroClicked = fourthHeroClicked = fourthEnemyClicked = false; // 스킬부터 선택하도록 값 다 초기화
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
        if (!DataManager.Instance.PartyFormation[HeroNum].isDead) // 안죽은 애들만 체크
        {
            if (DataManager.Instance.PartyFormation[HeroNum].heroStress >= 200)
            {
                if (DataManager.Instance.PartyFormation[HeroNum].heroHp == 0)
                {
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + "(이)가 심장마비로 <color=\"red\">사망</color>했습니다.");
                    HeroLeft--;
                    DataManager.Instance.PartyFormation[HeroNum].isDead = true;
                    BattleCanvas.transform.Find("Player" + (HeroNum + 1).ToString()).gameObject.SetActive(false);
                    Destroy(BattleScene.transform.Find("Player" + (HeroNum + 1).ToString()).transform.GetChild(2).gameObject);
                    Instantiate(Tomb, BattleScene.transform.Find("Player" + (HeroNum + 1).ToString()).transform).transform.localPosition = new Vector3(0, 0, 0);
                    ShowBattleLog("모든 Player의 스트레스 증가!");
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
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + "의 마음이 붕괴됩니다! 체력이 0이 됩니다.");
                    DataManager.Instance.PartyFormation[HeroNum].heroStress = 150;
                    DataManager.Instance.PartyFormation[HeroNum].heroHp = 0;
                    yield return new WaitForSecondsRealtime(1.5f);
                }
            }
            // 붕괴/각성 상태가 아니며 처음으로 100을 초과
            else if (DataManager.Instance.PartyFormation[HeroNum].heroStress >= 100 && DataManager.Instance.PartyFormation[HeroNum].Stress == Stress.Default)
            {
                ShowBattleLog("Player" + (HeroNum + 1).ToString() + "가 의지를 시험받고 있습니다.");

                //yield return new WaitForSecondsRealtime(3f);
                yield return waitForCorruption;

                if (UnityEngine.Random.Range(1, 101) > 25)
                {
                    DataManager.Instance.PartyFormation[HeroNum].Stress = Stress.Negative;
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + " : <color=\"red\">부정적!</color>");
                }
                else
                {
                    DataManager.Instance.PartyFormation[HeroNum].Stress = Stress.Positive;
                    ShowBattleLog("Player" + (HeroNum + 1).ToString() + " : <color=\"yellow\">긍정적!</color>");
                }
            }
        }
    }

    IEnumerator ShowDamageText(string text, bool isEnemy, int CharacterNum, bool isAdditionalDmg, bool isJustText)
    {
        yield return null;
        if(isEnemy) // 맞은게 적이다
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
            DmgText.transform.Find("Canvas_Dmg").transform.Find("DmgText").GetComponent<TextMeshProUGUI>().color = c; // 지속적으로 알파값을 줄여나감
        }
    }
}
