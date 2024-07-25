using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
    public GameObject Player_Sword;
    public GameObject Player_Gun;

    public CinemachineVirtualCamera characterCamera;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go;
        GameObject map = GameObject.Find("Canvas").transform.Find("Map").gameObject;
        GameObject keyGuide = GameObject.Find("Canvas").transform.Find("KeyGuide").gameObject;

        if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
        {
            go = Instantiate(Player_Gun, new Vector3(0, 0, 0), Player_Gun.transform.rotation);
            go.GetComponent<Player_Control>().map = map;
            go.GetComponent<Player_Control>().keyGuide = keyGuide;
            go.GetComponent<Player_Control>().healthUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<HealthUIManager>();

            go.transform.Find("GameObject").transform.Find("Basic Gun").gameObject.GetComponent<Gun_Basic_Shooting>().bulletUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<BulletUIManager>();
            go.transform.Find("GameObject").transform.Find("Rifle").gameObject.GetComponent<Gun_Rifle>().bulletUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<BulletUIManager>();
            go.transform.Find("GameObject").transform.Find("ShotGun").gameObject.GetComponent<Gun_ShotGun>().bulletUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<BulletUIManager>();
            go.transform.Find("GameObject").transform.Find("Sniper").gameObject.GetComponent<Gun_Basic_Shooting>().bulletUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<BulletUIManager>();

        }
        else
        {
            go = Instantiate(Player_Sword, new Vector3(0, 0, 0), Player_Sword.transform.rotation);
            go.GetComponent<Player_Control_Sword>().map = map;
            go.GetComponent<Player_Control_Sword>().keyGuide = keyGuide;
            go.GetComponent<Player_Control_Sword>().healthUIManager = GameObject.Find("Battle_Ui").transform.GetComponentInChildren<HealthUIManager>();
        }
        characterCamera.GetComponent<CinemachineVirtualCamera>().Follow = go.transform;

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
