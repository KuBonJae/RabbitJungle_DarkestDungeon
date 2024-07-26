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
            Color c = Announce.GetComponent<TextMeshProUGUI>().color; // 아나운서가 있는 중에 다른방에 들어가 버렸을땐 기존꺼 바로 0으로 없어지게 하고 다음거 진행
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

        // 이미지와 텍스트의 초기 색상 가져오기
        Color textColor = Announce.GetComponent<TextMeshProUGUI>().color;

        while (elapsedTime < 2f)
        {
            // 경과 시간 업데이트
            elapsedTime = Time.realtimeSinceStartup - startTime;

            // 현재 알파값 계산
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 2f);

            // 텍스트의 알파값 업데이트
            textColor.a = alpha;
            Announce.GetComponent<TextMeshProUGUI>().color = textColor;

            // 다음 프레임까지 대기
            yield return null;
        }

        startTime = Time.realtimeSinceStartup;
        elapsedTime = 0f;
        while(elapsedTime < 2f)
        {
            // 경과 시간 업데이트
            elapsedTime = Time.realtimeSinceStartup - startTime;

            // 현재 알파값 계산
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / 2f);

            // 텍스트의 알파값 업데이트
            textColor.a = alpha;
            Announce.GetComponent<TextMeshProUGUI>().color = textColor;

            // 다음 프레임까지 대기
            yield return null;
        }
        // 완전히 투명하게 설정
        textColor.a = 0f;
        Announce.GetComponent<TextMeshProUGUI>().color = textColor;
        Announce.GetComponent<TextMeshProUGUI>().text = "";
    }
}
