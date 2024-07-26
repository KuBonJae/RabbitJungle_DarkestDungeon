using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnounceManager : MonoBehaviour
{
    public GameObject Announce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DataManager.Instance.makeAnnouncement)
        {
            Color c = Announce.GetComponent<TextMeshProUGUI>().color; // �Ƴ���� �ִ� �߿� �ٸ��濡 �� �������� ������ �ٷ� 0���� �������� �ϰ� ������ ����
            c.a = 0f;
            Announce.GetComponent<TextMeshProUGUI>().color = c;
            DataManager.Instance.makeAnnouncement = false;
            Announce.GetComponent<TextMeshProUGUI>().text = DataManager.Instance.announcement;
            StartCoroutine("MakeAnnouncement");
        }
    }

    IEnumerator MakeAnnouncement()
    {
        yield return new WaitForSecondsRealtime(1f);

        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0f;

        // �̹����� �ؽ�Ʈ�� �ʱ� ���� ��������
        Color textColor = Announce.GetComponent<TextMeshProUGUI>().color;

        while (elapsedTime < 2f)
        {
            // ��� �ð� ������Ʈ
            elapsedTime = Time.realtimeSinceStartup - startTime;

            // ���� ���İ� ���
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 2f);

            // �ؽ�Ʈ�� ���İ� ������Ʈ
            textColor.a = alpha;
            Announce.GetComponent<TextMeshProUGUI>().color = textColor;

            // ���� �����ӱ��� ���
            yield return null;
        }

        startTime = Time.realtimeSinceStartup;
        elapsedTime = 0f;
        while(elapsedTime < 2f)
        {
            // ��� �ð� ������Ʈ
            elapsedTime = Time.realtimeSinceStartup - startTime;

            // ���� ���İ� ���
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / 2f);

            // �ؽ�Ʈ�� ���İ� ������Ʈ
            textColor.a = alpha;
            Announce.GetComponent<TextMeshProUGUI>().color = textColor;

            // ���� �����ӱ��� ���
            yield return null;
        }
        // ������ �����ϰ� ����
        textColor.a = 0f;
        Announce.GetComponent<TextMeshProUGUI>().color = textColor;
        Announce.GetComponent<TextMeshProUGUI>().text = "";
    }
}
