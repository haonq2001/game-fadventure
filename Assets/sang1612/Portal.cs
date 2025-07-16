using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform targetPortal; // Cổng đích
    //public GameObject teleportEffect; // Hiệu ứng khi dịch chuyển (nếu cần)
    public float teleportDelay = 0.5f; // Thời gian chờ trước khi dịch chuyển
    public float colliderDisableDuration = 1f; // Thời gian tắt collider sau khi dịch chuyển
    private bool isTeleporting = false; // Trạng thái dịch chuyển
    public GameObject cong2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportPlayer(collision));
        }
    }

    private IEnumerator TeleportPlayer(Collider2D player)
    {
        isTeleporting = true;

        if (targetPortal == null)
        {
            Debug.LogWarning("Target portal is not assigned!");
            yield break;
        }

        // Hiệu ứng tại vị trí hiện tại
        //if (teleportEffect != null)
        //{
        //    Instantiate(teleportEffect, transform.position, Quaternion.identity);
        //}

        yield return new WaitForSeconds(teleportDelay);


        // Dịch chuyển Player đến vị trí cổng đích
        player.transform.position = targetPortal.position;
      
        // Hiệu ứng tại vị trí đích
        //if (teleportEffect != null)
        //{
        //    Instantiate(teleportEffect, targetPortal.position, Quaternion.identity);
        //}

        Debug.Log("Player teleported to: " + targetPortal.position);

        // Tắt collider của cả hai cổng trong một khoảng thời gian
        Collider2D thisPortalCollider = GetComponent<Collider2D>();
        Collider2D targetPortalCollider = targetPortal.GetComponent<Collider2D>();

        thisPortalCollider.enabled = false;
        targetPortalCollider.enabled = false;

        // Đợi trong khoảng thời gian tắt collider
        yield return new WaitForSeconds(colliderDisableDuration);
        cong2.SetActive(false);
        // Bật lại collider của cả hai cổng
      //  thisPortalCollider.enabled = true;
      //  targetPortalCollider.enabled = true;

        isTeleporting = false;
       
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị phạm vi phát hiện trong Scene View
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
