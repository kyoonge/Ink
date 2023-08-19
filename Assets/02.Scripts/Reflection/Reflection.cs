using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<PlayerController>().isReflection)
            {
                collision.gameObject.GetComponent<PlayerController>().Reflection();
                this.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("ontriggerenter");
                collision.gameObject.GetComponent<PlayerController>().ReflectionOff();
                this.gameObject.SetActive(false);
            }
        }
    }
}
