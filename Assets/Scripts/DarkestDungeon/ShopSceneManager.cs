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
            Destroy(Players[i].transform.GetChild(0).gameObject); // ó�� ���� �� üũ������ �ξ��� ĸ���� ����

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
                Destroy(Players[i].transform.GetChild(0).gameObject); // ���� ������ ����
                Instantiate(Tomb, Players[i].transform).transform.localPosition = new Vector3(0, 0, 0); // ����� ����
                DataManager.Instance.PartyFormation[i].DeathChecked = true; // ���̻� �������� ��������� �ʵ��� üũ
            }
        }
    }
}
