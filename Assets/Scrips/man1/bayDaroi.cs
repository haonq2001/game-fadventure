using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bayDaroi : MonoBehaviour
{
    [SerializeField] private GameObject[] fallingRocks; // Các viên đá
    [SerializeField] private float fallDelay = 0.5f;    // Thời gian giữa mỗi viên đá rơi
    [SerializeField] private GameObject boxColliderObject; // Vùng kích hoạt

    private bool hasTriggered = false; // Đảm bảo chỉ kích hoạt một lần

    private void Start()
    {
        foreach (GameObject rock in fallingRocks)
        {
            Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
            BoxCollider2D collider = rock.GetComponent<BoxCollider2D>();

            if (rb != null)
            {
                rb.isKinematic = true; // Tắt trọng lực ban đầu
            }

            if (collider != null)
            {
                collider.enabled = false; // Tắt BoxCollider của đá
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Đảm bảo chỉ kích hoạt một lần

            // Vô hiệu hóa BoxCollider của vùng kích hoạt thay vì xóa nó
            BoxCollider2D triggerCollider = boxColliderObject.GetComponent<BoxCollider2D>();
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false; // Disable collider
            }

            StartCoroutine(FallRocksSequentially()); // Bắt đầu làm rơi đá
        }
    }

    private IEnumerator FallRocksSequentially()
    {
        foreach (GameObject rock in fallingRocks) // Lặp qua từng viên đá
        {
            if (rock != null)
            {
                Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
                BoxCollider2D collider = rock.GetComponent<BoxCollider2D>();

                if (collider != null)
                {
                    collider.enabled = true; // Bật BoxCollider trước khi đá rơi
                }

                if (rb != null)
                {
                    rb.isKinematic = false; // Kích hoạt trọng lực
                }
            }

            yield return new WaitForSeconds(fallDelay); // Chờ giữa các viên đá
        }
    }
}
