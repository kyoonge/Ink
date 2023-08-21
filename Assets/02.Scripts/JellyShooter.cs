using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class JellyShooter : MonoBehaviour
{
    public JellyData data;
    public JellyBullet jellyBullet;
    public GameObject slimeHeadGraphic;
    public GameObject slimeHeadGraphicClone;
    public GameObject cloneJellyBullet;
    public GameObject clonePlayer;

    public Coloring jellyColoring = Coloring.Red;

    public ColoredObject jelliedObject = null;
    public bool canShoot = true;
    public bool canRetrieve = false;

    private void Start()
    {
        UpdateHeadColor();
        canShoot = true;
        canRetrieve = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (jelliedObject == null)
            {
                var _hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
                if (_hit.collider != null)
                {
                    ColoredObject _obj = _hit.collider.GetComponent<ColoredObject>();
                    if (_obj != null && _obj.isEyeball)
                    {
                        if (canShoot) ShootJelly(_obj.transform);
                        Debug.Log("Mouse & Shoot");
                    }
                }
            }
            else if(jelliedObject != null)
            {
                Debug.Log("Mouse & Retrieve");
                if (canRetrieve) RetriveJelly();
            }
        }
    }

    private void ShootJelly(Transform target)
    {
        canShoot = false;
        slimeHeadGraphic.SetActive(false);
        jellyBullet.transform.position = slimeHeadGraphic.transform.position;
        jellyBullet.SetTarget(target, false);
        jellyBullet.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
        jellyBullet.gameObject.SetActive(true);

        if (GetComponent<PlayerController>().isReflection)
        {
            slimeHeadGraphicClone.SetActive(false);
            cloneJellyBullet.transform.position = slimeHeadGraphicClone.transform.position;
            cloneJellyBullet.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
            cloneJellyBullet.gameObject.SetActive(true);
        }
    }

    private void RetriveJelly()
    {
        if (jelliedObject.isEyeball)
        {
            SetJellyColoring(jelliedObject.objectColoring);
            jelliedObject.JellyLeavesEyeball();
        }

        canRetrieve = false;
        jellyBullet.transform.position = jelliedObject.transform.position;
        jellyBullet.SetTarget(slimeHeadGraphic.transform, true);
        jellyBullet.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
        jellyBullet.gameObject.SetActive(true);

        if (GetComponent<PlayerController>().isReflection)
        {
            cloneJellyBullet.transform.position = jelliedObject.transform.position;
            //cloneJellyBullet.SetTarget(slimeHeadGraphic.transform, true);
            cloneJellyBullet.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
            cloneJellyBullet.gameObject.SetActive(true);
        }


        Collider2D _col = jelliedObject.GetComponent<Collider2D>();
        bool _tempActive = _col.enabled;
        bool _tempTrigger = _col.isTrigger;
        _col.enabled = true;
        _col.isTrigger = true;
        Vector2 _closestPoint = _col.ClosestPoint(slimeHeadGraphic.transform.position);
        _col.enabled = _tempActive;
        _col.isTrigger = _tempTrigger;
        Vector2 _direction = ((Vector2)slimeHeadGraphic.transform.position - _closestPoint).normalized;
        GetComponent<JellyEffect>().JellyEffectOff(_closestPoint + _direction * 0.3f);

    }

    public void UpdateHeadColor()
    {
        slimeHeadGraphic.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
        slimeHeadGraphicClone.GetComponent<SpriteRenderer>().color = ColorManager.instance.GetColorByColoring(jellyColoring);
    }

    public void JellifyComplete(ColoredObject _jelliedObject)
    {
        Debug.Log("JellifyComplete");
        _jelliedObject.GetJellied(jellyColoring);
        jelliedObject = _jelliedObject;
        Debug.Log("jelliedObject "+jelliedObject);
        Debug.Log("_jelliedObject " + _jelliedObject);
        canRetrieve = true;
    }

    public void UnjellifyComplete()
    {
        Debug.Log("unjellify");
        jelliedObject.GetUnjellied();
        jelliedObject = null;
    }

    public void SetJellyColoring(Coloring coloring)
    {
        jellyColoring = coloring;
        UpdateHeadColor();
    }
}
