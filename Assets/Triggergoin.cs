using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggergoin : MonoBehaviour
{
    // Start is called before the first frame update
    private vukhi_goblin enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<vukhi_goblin>();
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
