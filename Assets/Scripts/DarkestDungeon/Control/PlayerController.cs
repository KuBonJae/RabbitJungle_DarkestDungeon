using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.ConstrainedExecution;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    bool up = false;
    bool down = false;
    bool right = false;
    bool left = false;

    public GameObject map;
    public GameObject keyGuide;

    float speed = 15f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Flip");
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = ManagingInput.GetAxis("Horizontal");
        verticalInput = ManagingInput.GetAxis("Vertical");

        Block();
        toggleMap();

        transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * speed);
        transform.Translate(Vector2.up * verticalInput * Time.deltaTime * speed);
    }

    private void toggleMap()
    {
        if (ManagingInput.GetKeyDown(KeyCode.Tab))
        {
            map.SetActive(!map.activeSelf);
            keyGuide.SetActive(!keyGuide.activeSelf);

            UpdatePlayerStat();

            if(!DataManager.Instance.battle_ing && !GameObject.Find("Canvas").transform.Find("CardSelect").gameObject.activeSelf) // battle - ing -> battle state -> timescale is 0
            {
                if (map.activeSelf)
                    Time.timeScale = 0f;
                else
                    Time.timeScale = 1f;
            }
        }
    }

    private void UpdatePlayerStat()
    {
        for(int i=0;i<4;i++)
        {
            string playerStat = "";
            playerStat += "Player" + (i + 1).ToString() + " : " + DataManager.Instance.PartyFormation[i].heroClass.ToString() + "\n";
            playerStat += "Hp : " + DataManager.Instance.PartyFormation[i].heroHp.ToString() + " / " + DataManager.Instance.PartyFormation[i].heroMaxHP.ToString() + "\n";
            playerStat += "Stress : " + DataManager.Instance.PartyFormation[i].heroStress.ToString() + "\n";
            playerStat += "State : " + DataManager.Instance.PartyFormation[i].Stress.ToString() + "\n";
            playerStat += "Dmg : " + DataManager.Instance.PartyFormation[i].heroMinDamage.ToString() + " / " + DataManager.Instance.PartyFormation[i].heroMaxDamage.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempDmg.ToString() : "0") + ")\n";
            playerStat += "Dodge : " + DataManager.Instance.PartyFormation[i].heroBasicDodgeRate.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempDodge.ToString() : "0") + ")\n";
            playerStat += "Crit : " + DataManager.Instance.PartyFormation[i].heroBasicCriticalHit.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempCrit.ToString() : "0") + ")\n";
            playerStat += "Protect : " + DataManager.Instance.PartyFormation[i].heroBasicProtection.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempProtect.ToString() : "0") + ")\n";
            playerStat += "Speed : " + DataManager.Instance.PartyFormation[i].heroBasicSpeed.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempSpeed.ToString() : "0") + ")\n";
            playerStat += "Accuracy : " + DataManager.Instance.PartyFormation[i].heroBasicAccuracy.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempAccuracy.ToString() : "0") + ")\n";
            playerStat += "Death Resist : " + DataManager.Instance.PartyFormation[i].heroBasicDeathDoor.ToString() + " + (" +
                (DataManager.Instance.tempStats[i] != null ? DataManager.Instance.tempStats[i].tempDD.ToString() : "0") + ")\n";

            map.transform.Find("KeyGuide").GetChild(i + 1).GetComponent<TextMeshProUGUI>().text = playerStat;
        }
        map.transform.Find("KeyGuide").transform.Find("CoinAmount").GetComponent<TextMeshProUGUI>().text = "현재 코인 : " + DataManager.Instance.coin.ToString();
    }

    IEnumerator Flip()
    {
        while (true)
        {
            horizontalInput = ManagingInput.GetAxis("Horizontal");

            yield return null;
            if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontalInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }

    public void Block()
    {
        RaycastHit2D[] hitDownRight = Physics2D.RaycastAll(transform.position, (Vector2.down + Vector2.right).normalized);
        for (int i = 0; i < hitDownRight.Length; i++)
        {
            if (hitDownRight[i].transform != null)
            {
                if (hitDownRight[i].distance < 0.5 && hitDownRight[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        //horizontalInput = 0;
                        right = true;
                    }
                    if (verticalInput < 0)
                    {
                        //verticalInput = 0;
                        down = true;
                    }

                    if (down && right)
                    {
                        down = right = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        down = right = false;
                }
            }
        }

        RaycastHit2D[] hitDownLeft = Physics2D.RaycastAll(transform.position, (Vector2.down + Vector2.left).normalized);
        for (int i = 0; i < hitDownLeft.Length; i++)
        {
            if (hitDownLeft[i].transform != null)
            {
                if (hitDownLeft[i].distance < 0.5 && hitDownLeft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        //horizontalInput = 0;
                        left = true;
                    }
                    if (verticalInput < 0)
                    {
                        //verticalInput = 0;
                        down = true;
                    }

                    if (down && left)
                    {
                        down = left = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        down = left = false;
                }
            }
        }

        RaycastHit2D[] hitUpRight = Physics2D.RaycastAll(transform.position, (Vector2.up + Vector2.right).normalized);
        for (int i = 0; i < hitUpRight.Length; i++)
        {
            if (hitUpRight[i].transform != null)
            {
                if (hitUpRight[i].distance < 0.5 && hitUpRight[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        //horizontalInput = 0;
                        right = true;
                    }
                    if (verticalInput > 0)
                    {
                        //verticalInput = 0;
                        up = true;
                    }

                    if (up && right)
                    {
                        up = right = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        up = right = false;
                }
            }
        }

        RaycastHit2D[] hitUpLeft = Physics2D.RaycastAll(transform.position, (Vector2.up + Vector2.left).normalized);
        for (int i = 0; i < hitUpLeft.Length; i++)
        {
            if (hitUpLeft[i].transform != null)
            {
                if (hitUpLeft[i].distance < 0.5 && hitUpLeft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        //horizontalInput = 0;
                        left = true;
                    }
                    if (verticalInput > 0)
                    {
                        //verticalInput = 0;
                        up = true;
                    }

                    if (up && left)
                    {
                        up = left = false;
                        horizontalInput = verticalInput = 0;
                        return;
                    }
                    else
                        up = left = false;
                }
            }
        }

        RaycastHit2D[] hitdown = Physics2D.RaycastAll(transform.position, Vector2.down);

        for (int i = 0; i < hitdown.Length; i++)
        {
            if (hitdown[i].transform != null)
            {
                if (hitdown[i].distance < 1 && hitdown[i].collider.CompareTag("Wall"))
                {
                    if (verticalInput < 0)
                    {
                        verticalInput = 0;
                        return;
                    }
                }

            }
        }

        RaycastHit2D[] hitup = Physics2D.RaycastAll(transform.position, Vector2.up);
        for (int i = 0; i < hitup.Length; i++)
        {
            if (hitup[i].transform != null)
            {
                if (hitup[i].distance < 1 && hitup[i].collider.CompareTag("Wall"))
                {
                    if (verticalInput > 0)
                    {
                        verticalInput = 0;
                        return;
                    }
                }
            }
        }

        RaycastHit2D[] hitleft = Physics2D.RaycastAll(transform.position, Vector2.left);
        for (int i = 0; i < hitleft.Length; i++)
        {
            if (hitleft[i].transform != null)
            {
                if (hitleft[i].distance < 0.5 && hitleft[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput < 0)
                    {
                        horizontalInput = 0;
                        return;
                    }
                }

            }
        }

        RaycastHit2D[] hitright = Physics2D.RaycastAll(transform.position, Vector2.right);
        for (int i = 0; i < hitright.Length; i++)
        {
            if (hitright[i].transform != null)
            {
                if (hitright[i].distance < 0.5 && hitright[i].collider.CompareTag("Wall"))
                {
                    if (horizontalInput > 0)
                    {
                        horizontalInput = 0;
                        return;
                    }
                }
            }
        }
    }
}
