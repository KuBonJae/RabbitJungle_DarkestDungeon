using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;

public class ShieldCounter : MonoBehaviour
{
    //[SerializeField] CinemachineVirtualCamera parryCamera;
    GameObject parryCamera;
    float originalSize;
    public ParticleSystem parryParticle;
    // Start is called before the first frame update
    void Start()
    {
        //parryCamera = GameObject.Find("Camera").transform.Find("ParryCamera").GetComponent<CinemachineVirtualCamera>();
        parryCamera = GameObject.Find("Camera").transform.Find("ParryCamera").gameObject;
        parryCamera.GetComponent<CinemachineFreeLook>().m_Lens.OrthographicSize = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("카운터!");

            parryCamera.gameObject.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 5f);
            Instantiate(parryParticle, collision.transform.position, collision.transform.rotation);
            //parryParticle.Play();
            //Destroy(parryParticle, parryParticle.main.duration);
            //parryCamera.m_Lens.OrthographicSize = 10f;

            collision.transform.GetComponent<EnemyHeat>().enemyHP -= DataManager.Instance.Damage;
            collision.transform.GetComponent<EnemyHeat>().hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
            collision.transform.GetComponent<EnemyHeat>().beDamaged = true;
            collision.transform.GetComponent<EnemyHeat>().checkEnemysHP();
            Vector3 revDir = collision.transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
            collision.transform.GetComponent<Rigidbody2D>().AddForce(revDir.normalized * 1000f);
            //GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Control_Sword>().shield.GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("SlowMotionInCounter");
            StartCoroutine("ParryCameraOn");
            //StartCoroutine("Invincible");
        }
        else if (collision.gameObject.CompareTag("BOSS"))
        {
            Debug.Log("카운터!");

            parryCamera.gameObject.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 5f);
            Instantiate(parryParticle, collision.transform.position, collision.transform.rotation);
            //parryParticle.Play();
            //Destroy(parryParticle, parryParticle.main.duration);
            //parryCamera.m_Lens.OrthographicSize = 10f;

            collision.transform.GetComponent<BossHeat>().bossHP -= DataManager.Instance.Damage;
            collision.transform.GetComponent<BossHeat>().hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
            collision.transform.GetComponent<BossHeat>().beDamaged = true;
            collision.transform.GetComponent<BossHeat>().checkBosssHP();
            Vector3 revDir = collision.transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
            collision.transform.GetComponent<Rigidbody2D>().AddForce(revDir.normalized * 1000f);
            //GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Control_Sword>().shield.GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("SlowMotionInCounter");
            StartCoroutine("ParryCameraOn");
            //StartCoroutine("Invincible");
        }
        else if (collision.gameObject.CompareTag("EnemyWeapon"))
        {

            if (collision.transform.GetComponent<BossShootBullet>() != null || collision.transform.GetComponent<EnemyBullet>() != null)
            {
                parryCamera.gameObject.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 5f);
                Instantiate(parryParticle, collision.transform.position, collision.transform.rotation);

                // if bullet, reverse their direction and make their tag to player's weapon
                if (collision.transform.GetComponent<BossShootBullet>() != null)
                    collision.transform.GetComponent<BossShootBullet>().bulletDirection *= -1;
                else if (collision.transform.GetComponent<EnemyBullet>() != null)
                    collision.transform.GetComponent<EnemyBullet>().bulletDirection *= -1;

                collision.gameObject.tag = "Weapon";
            }
            else
            {
                parryCamera.gameObject.transform.position = new Vector3(collision.transform.parent.transform.parent.position.x, 
                    collision.transform.parent.transform.parent.position.y, collision.transform.parent.transform.parent.position.z - 5f);
                Instantiate(parryParticle, collision.transform.parent.transform.parent.position, collision.transform.parent.transform.parent.rotation);

                collision.transform.parent.transform.parent.transform.GetComponent<EnemyHeat>().enemyHP -= DataManager.Instance.Damage;
                collision.transform.parent.transform.parent.transform.GetComponent<EnemyHeat>().hpText.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.Damage.ToString();
                collision.transform.parent.transform.parent.transform.GetComponent<EnemyHeat>().beDamaged = true;
                collision.transform.parent.transform.parent.transform.GetComponent<EnemyHeat>().checkEnemysHP();
                Vector3 revDir = collision.transform.parent.transform.parent.position - GameObject.FindGameObjectWithTag("Player").transform.position;
                collision.transform.parent.transform.parent.GetComponent<Rigidbody2D>().AddForce(revDir.normalized * 1000f);
            }
            
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("SlowMotionInCounter");
            StartCoroutine("ParryCameraOn");
        }
    }

    IEnumerator SlowMotionInCounter()
    {
        Time.timeScale = 0.1f;
        float slowTime = 0f;
        while (true)
        {
            yield return null;

            slowTime += Time.unscaledDeltaTime;
            if (slowTime > 0.5f)
            {
                Time.timeScale = 1f;
                break;
            }
        }
    }

    IEnumerator ParryCameraOn()
    {
        parryCamera.SetActive(true);
        yield return new WaitForSecondsRealtime(0.7f);
        parryCamera.SetActive(false);
    }

    IEnumerator Invincible()
    {
        DataManager.Instance.DashState = true;
        //this.gameObject.layer = 10;
        yield return new WaitForSeconds(0.25f);
        DataManager.Instance.DashState = false;
        //this.gameObject.layer = 6;
    }
}
