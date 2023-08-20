using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Vector2 offset;

    public PlayerController _playerController;
    public ClonePlayer _clonePlayer;
    public bool isReflection;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        isReflection = false;
    }

    private void Update()
    {
        MoveCameraToSpot();
    }

    public void ApplyCurrentOffset()
    {
        if (_playerController == null)
        {
            Debug.LogWarning("PlayerController를 이 스크립트에 할당해줘야 작동합니다.");
            return;
        }
        offset = transform.position - _playerController.transform.position;
    }

    public void MoveCameraToSpot()
    {
        if (!isReflection)
        {
            Vector3 _pos = _playerController.transform.position + (Vector3)offset;
            _pos.z = -10;
            transform.position = _pos;
            //Debug.Log("1: "+offset);
        }
        else
        {
            offset = transform.position - (_playerController.transform.position + _clonePlayer.transform.position) * 0.5f;
            Vector3 _pos = (_playerController.transform.position + _clonePlayer.transform.position + (Vector3)offset)*0.5f;
            _pos.z = -10;
            transform.position = _pos;
            //Debug.Log("2: " + offset);
        }

    }

}
