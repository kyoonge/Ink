using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortalTiming : MonoBehaviour
{
    public GameObject portal;
    public GameObject camera;
    public float targetposX;
    public float delayTime = 1.0f; // ���� �ð�
    public float cameraAnimationDuration = 1.5f; // ī�޶� �ִϸ��̼� ���� �ð�
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
        yield return new WaitForSeconds(delayTime); // ���� �ð� ���

        portal.SetActive(true);
        hidden.SetActive(true);
        hiddenTxt.SetActive(true);

        // DOTween�� ����Ͽ� ī�޶� �������� x���� ������ �����ϴ� �ִϸ��̼�
        float initialOffsetX = camera.GetComponent<CameraController>().offset.x;
        camera.GetComponent<CameraController>().offset.x = initialOffsetX; // ���� �� ����

        // �ִϸ��̼� ���� �� ����
        Tweener tweener = DOTween.To(() => initialOffsetX, x => camera.GetComponent<CameraController>().offset.x = x, targetposX, cameraAnimationDuration);

        // �ִϸ��̼��� ���� ������ ���
        yield return tweener.WaitForCompletion();
    }
}