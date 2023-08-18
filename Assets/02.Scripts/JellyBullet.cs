using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyBullet : MonoBehaviour
{
    public JellyData data;
    public bool isReturning;
    private Transform _target;
    private JellyShooter _shooter;

    public void SetTarget(Transform target, bool isReturning)
    {
        _target = target;
        this.isReturning = isReturning;
    }
    private void Start()
    {
        _shooter = FindObjectOfType<JellyShooter>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, _target.position) > data.jellyBulletSpeed * Time.deltaTime * 2f)
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            transform.position += (Vector3)direction * data.jellyBulletSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = _target.position;
            if (isReturning)
            {
                //�Ӹ� �����
                _shooter.slimeHeadGraphic.gameObject.SetActive(true);
                _shooter.canShoot = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _target && !isReturning)
        {
            //���� ��� ����Ʈ ����
            FindObjectOfType<JellyEffect>().JellyEffectOn(_target, transform.position);
            _target.GetComponent<ColoredObject>().EyeballEaten();
            gameObject.SetActive(false);

        }
    }
}
