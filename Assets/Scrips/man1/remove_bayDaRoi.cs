using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remove_bayDaRoi : MonoBehaviour
{
    // [SerializeField] private GameObject destroyEffect; // Hiệu ứng khi viên đá bị phá hủy

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gây sát thương cho nhân vật (nếu cần)
            Debug.Log("Rock hit the Player!");
            //      if (destroyEffect != null)
            // {
            //     Instantiate(destroyEffect, transform.position, Quaternion.identity);
            // }
            // Destroy viên đá
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            // Khi đá chạm đất
            Debug.Log("Rock hit the Ground!");

            // Destroy viên đá
            Destroy(gameObject);
        }
    }


}
