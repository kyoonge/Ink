using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortalTiming : MonoBehaviour
{
    public GameObject portal;
    public GameObject camera;
    public float targetposX;
    public float delayTime = 1.0f; // 지연 시간
    public float cameraAnimationDuration = 1.5f; // 카메라 애니메이션 지속 시간
    public GameObject hidden;
    public GameObject hiddenTxt;

    private void Start()
    {
        targetposX = -8.7f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivatePortalAndAdjustCamera());
        }
    }

    private IEnumerator ActivatePortalAndAdjustCamera()
    {
        yield return new WaitForSeconds(delayTime); // 일정 시간 대기

        portal.SetActive(true);
        hidden.SetActive(true);
        hiddenTxt.SetActive(true);

        // DOTween을 사용하여 카메라 오프셋의 x값을 서서히 변경하는 애니메이션
        float initialOffsetX = camera.GetComponent<CameraController>().offset.x;
        camera.GetComponent<CameraController>().offset.x = initialOffsetX; // 시작 값 설정

        // 애니메이션 생성 및 실행
        Tweener tweener = DOTween.To(() => initialOffsetX, x => camera.GetComponent<CameraController>().offset.x = x, targetposX, cameraAnimationDuration);

        // 애니메이션이 끝날 때까지 대기
        yield return tweener.WaitForCompletion();
    }
}