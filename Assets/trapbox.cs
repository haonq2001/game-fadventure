using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class trapbox : MonoBehaviour
{
    public GameObject panelgameover;
    // Animator
     public Animator animator;


    void Start()
    {
        // Lấy Animator từ GameObject hiện tại
         animator = GetComponent<Animator>();
        // Đảm bảo panel ẩn khi bắt đầu
      //  panelgameover.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            panelgameover.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("trapattack");
        }
    }

}