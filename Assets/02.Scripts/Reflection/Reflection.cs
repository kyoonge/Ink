using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<ClonePlayer>()!=null)
            {
                Debug.Log("is Clone");
                player.GetComponent<PlayerController>().StartCoroutine("ReflectionOff");
            }
            else if (!collision.gameObject.GetComponent<PlayerController>().isReflection)
            {
                Debug.Log("ontriggerenter: !isReflection");
                collision.gameObject.GetComponent<PlayerController>().StartCoroutine("Reflection");
            }
            else
            {
                Debug.Log("ontriggerenter: isReflection");
                collision.gameObject.GetComponent<PlayerController>().StartCoroutine("ReflectionOff");
            }
            this.gameObject.SetActive(false);
        }
    }
}
