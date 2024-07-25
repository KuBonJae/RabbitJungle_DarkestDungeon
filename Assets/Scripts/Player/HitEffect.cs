using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Image hitUIImage; // UI 이미지
    public CinemachineImpulseSource cameraShake; // 카메라 흔들림 소스
    public float fadeDuration = 0.5f; // 투명도 조절 시간
    private bool isHeat = false;

    // Start is called before the first frame update
    void Start()
    {
        hitUIImage = GameObject.Find("Canvas").transform.Find("HitImg").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeat)
        {
            Debug.Log("Hit");
            // 카메라 흔들림 효과를 적용합니다.
            cameraShake.GenerateImpulse();

            // Hit UI 이미지의 투명도를 서서히 감소시킵니다.
            StartCoroutine(FadeOutHitImage());

            // isHeat 상태를 false로 설정합니다.
            isHeat = false;
        }
    }
    /// <summary>
    /// 피격당하면 실행
    /// </summary>
    public void TriggerHitEffect()
    {
        // isHeat 상태를 true로 설정합니다.
        isHeat = true;

        // Hit UI 이미지를 활성화합니다.
        hitUIImage.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hit UI 이미지의 투명도를 서서히 감소시킵니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutHitImage()
    {
        float elapsedTime = 0f;
        Color color = hitUIImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            hitUIImage.color = color;
            yield return null;
        }

        // Hit UI 이미지를 비활성화합니다.
        hitUIImage.gameObject.SetActive(false);
    }
}
