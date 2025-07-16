using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dungGame_bayGai : MonoBehaviour
{
    // Panel
    public GameObject panel;
    // Animator
    // public Animator animator;

    void Start()
    {
        // Lấy Animator từ GameObject hiện tại
        // animator = GetComponent<Animator>();
        // Đảm bảo panel ẩn khi bắt đầu
        panel.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            panel.SetActive(true);
        }
    }


}