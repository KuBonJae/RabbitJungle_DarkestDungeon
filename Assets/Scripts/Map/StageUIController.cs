using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUIController : MonoBehaviour
{
    public Image loading;
    public TextMeshProUGUI stageText;
    // Start is called before the first frame update
    void Start()
    {
        stageText.text = "STAGE - " + DataManager.Instance.StageLevel;
        StartCoroutine(FadeOutCoroutine());
    }
    IEnumerator FadeOutCoroutine()
    {
        // Time.timeScale을 0으로 설정
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        // 시작 시간 기록
        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0f;

        // 이미지와 텍스트의 초기 색상 가져오기
        Color imageColor = loading.color;
        Color textColor = stageText.color;

        while (elapsedTime < 2f)
        {
            // 경과 시간 업데이트
            elapsedTime = Time.realtimeSinceStartup - startTime;

            // 현재 알파값 계산
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / 2f);

            // 이미지와 텍스트의 알파값 업데이트
            imageColor.a = alpha;
            textColor.a = alpha;
            loading.color = imageColor;
            stageText.color = textColor;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 완전히 투명하게 설정
        imageColor.a = 0f;
        textColor.a = 0f;
        loading.color = imageColor;
        stageText.color = textColor;

        // Time.timeScale을 1로 복원
        Time.timeScale = 1f;
        loading.gameObject.SetActive(false);
    }
}
