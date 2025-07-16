using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerChienBinh : MonoBehaviour
{
    private chienbinh enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<chienbinh>();
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
