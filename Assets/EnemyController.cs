using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PolygonCollider2D triggerCollider;  // Collider trigger
    public  BoxCollider2D     physicalCollider; // Collider không phải trigger

    private void Start()
    {
        // Đảm bảo chỉ bật trigger ban đầu
        triggerCollider.enabled = true;
        physicalCollider.enabled = false;
    }

    // Hàm bật collider vật lý khi nhân vật trượt
    public void EnablePhysicalCollider()
    {
        Debug.Log("Bật collider vật lý của quái.");
        physicalCollider.enabled = true;
    }

    // Hàm tắt collider vật lý sau khi trượt xong
    public void DisablePhysicalCollider()
    {
        Debug.Log("Tắt collider vật lý của quái.");
        physicalCollider.enabled = false;
    }
}
