using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSceneManager : MonoBehaviour
{
    public GameObject[] Players;

    public GameObject MeleePrefab;
    public GameObject MarksmanPrefab;
    public GameObject HealerPrefab;
    public GameObject SupporterPrefab;
    public GameObject TankerPrefab;
    public GameObject Tomb;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<4; i++)
        {
            Destroy(Players[i].transform.GetChild(0).gameObject); // 처음 시작 시 체크용으로 두었던 캡슐들 제거

            if (DataManager.Instance.PartyFormation[i].isDead)
            {
                Instantiate(Tomb, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                DataManager.Instance.PartyFormation[i].DeathChecked = true;
            }
            else
            {
                switch(DataManager.Instance.PartyFormation[i].heroClass)
                {
                    case ClassName.Supporter:
                        Instantiate(SupporterPrefab, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case ClassName.Healer:
                        Instantiate(HealerPrefab, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case ClassName.Melee:
                        Instantiate(MeleePrefab, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case ClassName.Marksman:
                        Instantiate(MarksmanPrefab, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case ClassName.Tanker:
                        Instantiate(TankerPrefab, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<4;i++)
        {
            if (DataManager.Instance.PartyFormation[i].isDead && !DataManager.Instance.PartyFormation[i].DeathChecked)
            {
                Destroy(Players[i].transform.GetChild(0).gameObject); // 기존 프리팹 삭제
                Instantiate(Tomb, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0); // 묘비로 변경
                DataManager.Instance.PartyFormation[i].DeathChecked = true; // 더이상 프리팹을 재생성하지 않도록 체크
            }
        }
    }
}
