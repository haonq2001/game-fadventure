using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moquisArea : MonoBehaviour
{
    private moquisto enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<moquisto>();
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
