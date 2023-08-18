using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2 offset;

    public PlayerController _playerController;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        MoveCameraToSpot();
    }

    public void ApplyCurrentOffset()
    {
        if (_playerController == null)
        {
            Debug.LogWarning("PlayerController�� �� ��ũ��Ʈ�� �Ҵ������ �۵��մϴ�.");
            return;
        }
        offset = transform.position - _playerController.transform.position;
    }

    public void MoveCameraToSpot()
    {
        Vector3 _pos = _playerController.transform.position + (Vector3)offset;
        _pos.z = -10;
        transform.position = _pos;
    }
}
