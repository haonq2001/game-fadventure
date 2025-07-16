using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_bossMan3 : MonoBehaviour
{
     private Boss_man3 enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<Boss_man3>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyParent.target = other.transform;
            enemyParent.inRange = true;
            enemyParent.hotZone.SetActive(true);
        }
    }
}

