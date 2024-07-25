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
    private Button[] cardButtons = new Button[3];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        if (DataManager.Instance.justCleared)
        {
            DataManager.Instance.justCleared = false;
            StartCoroutine("CardSelect");
        }
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
        // 선택된 카드의 효과를 적용
        ApplyCardEffect(selectedButton.gameObject.name);

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
    private void ApplyCardEffect(string cardName)
    {
        switch (cardName)
        {
            case "card1":
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
        int rand = DataManager.Instance.specialWeaponGet ? Random.Range(1, 21) : Random.Range(5, 21);
        if (rand <= 4 /*&& !DataManager.Instance.epicSkill*/)
        {
            string effect = "";
            if (DataManager.Instance.SpecialWeapon == SpecialWeaponType.ShortSword.ToString())
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
            }

            card = Instantiate(epicCard, CardSelectUI.transform);
            card.gameObject.name = "card1";
            card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Now, you can kill them all....";
            card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effect;
        }
        else if (rand <= 6)
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
        }
        else
        {
            card = Instantiate(normalCard, CardSelectUI.transform);
            switch (rand % 4)
            {
                case 0:
                    card.gameObject.name = "card4";
                    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Attack speed ++";
                    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Attack more!";
                    break;
                case 1:
                    card.gameObject.name = "card5";
                    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Move speed ++";
                    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Move faster!";
                    break;
                case 2:
                    card.gameObject.name = "card6";
                    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Attack damage ++";
                    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Attack harder!";
                    break;
                case 3:
                    card.gameObject.name = "card7";
                    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Recover Health ++";
                    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Don't die!";
                    break;
                //case 4:
                //    card.gameObject.name = "card8";
                //    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "OOPS!";
                //    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "Maybe next chance...";
                //    break;
                //case 4:
                //    card.gameObject.name = "card9";
                //    card.transform.Find("Explain").GetComponent<TextMeshProUGUI>().text = "Random";
                //    card.transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "It's literally Random.";
                //    break;
                default:
                    break;
            }
        }
        return card;
    }
}
