using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerResetPos : MonoBehaviour
{
    [SerializeField] private Vector3 newPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.transform.position = newPos;
        }
    }
}
