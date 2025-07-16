using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dan_DauLauMap3 : MonoBehaviour
{
    public float damage = 10f; // Lượng sát thương của viên đạn
    public float lifetime = 0.6f; // Thời gian tồn tại của viên đạn trước khi tự hủy
    public Animator animator; // Đúng kiểu Animator

    private void Start()
    {
        // Tự hủy viên đạn sau một thời gian nhất định
        Destroy(gameObject, lifetime);

        // Lấy Animator từ chính GameObject này
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Không tìm thấy component Animator trên GameObject!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("dungkhien"))
        {

            Destroy(gameObject);
        }
         else  if (collision.CompareTag("Player"))
        {
            // Gọi hàm nổ
            batNo();

            // Hủy viên đạn sau khi va chạm
            Destroy(gameObject,0.2f); // Đợi animation nổ trước khi hủy
        }
        else if (collision.CompareTag("Ground"))
        {
            // Gọi hàm nổ
            batNo();

            // Hủy viên đạn sau khi va chạm
            Destroy(gameObject, 0.5f); // Đợi animation nổ trước khi hủy
        }
    }

    public void batNo()
    {
        if (animator != null)
        {
            animator.SetBool("vacham_bullet", true);
        }
    }
}
