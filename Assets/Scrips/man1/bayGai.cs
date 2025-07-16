using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayGai : MonoBehaviour
{
    // Panel
    // public GameObject panel;
    // Animator
    public Animator animator;

    void Start()
    {
        // Lấy Animator từ GameObject hiện tại
        animator = GetComponent<Animator>();
        // Đảm bảo panel ẩn khi bắt đầu
        // panel.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gai hit the Player!");
            // Chuyển trạng thái Animation "isXut"
            animator.SetBool("isXut", true);
            Destroy(gameObject,1f);
            // Chờ Animation hoàn tất trước khi dừng game và hiển thị panel
            // StartCoroutine(WaitForAnimation());
        }
    }

    // private IEnumerator WaitForAnimation()
    // {
    //     // Đợi đến khi Animator chuyển sang trạng thái "isXut"
    //     while (!animator.GetCurrentAnimatorStateInfo(0).IsName("isXut"))
    //     {
    //         yield return null;
    //     }

    //     // Lấy thời lượng Animation "isXut"
    //     AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
    //     float animationDuration = animationState.length;

    //     // Đợi Animation chạy xong
    //     yield return new WaitForSeconds(animationDuration);

    //     // Dừng thời gian và hiển thị panel
    //     Time.timeScale = 0f;
    //     panel.SetActive(true);
    // }
}
