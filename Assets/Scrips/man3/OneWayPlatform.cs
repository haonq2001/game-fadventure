using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  

public class OneWayPlatform : MonoBehaviour  
{  
    private PlatformEffector2D effector;  
    public float waitTime = 0.2f;  
    public float jumpCooldown = 3f; // Thời gian chờ giữa các lần nhảy  
    private bool canJump = true; // Kiểm tra trạng thái nhảy  

    void Start()  
    {  
        effector = GetComponent<PlatformEffector2D>();  
    }  

    void Update()  
    {  
        // Nhấn phím "S" để nhảy xuyên qua nền  
        if (Input.GetKeyDown(KeyCode.S) && canJump)  
        {  
            effector.rotationalOffset = 180f; // Cho phép xuyên từ dưới lên  
            StartCoroutine(ResetEffector());  
            
            // Gọi hàm nhảy ở đây  
            Jump();  
            StartCoroutine(JumpCooldown());  
        }  
    }  
//
    private void Jump()  
    {  
        // Cơ chế nhảy  
        Rigidbody2D rb = GetComponent<Rigidbody2D>();  
        if (rb != null)  
        {  
            rb.velocity = new Vector2(rb.velocity.x, 10f); // Thay đổi giá trị 10f theo cách mà bạn muốn  
        }  
    }  

    IEnumerator ResetEffector()  
    {  
        yield return new WaitForSeconds(waitTime);  
        effector.rotationalOffset = 0f; // Trả về trạng thái ban đầu  
    }  

    IEnumerator JumpCooldown()  
    {  
        canJump = false; // Ngăn chặn nhảy  
        yield return new WaitForSeconds(jumpCooldown); // Chờ thời gian giữa các lần nhảy  
        canJump = true; // Cho phép nhảy lại  
    }  
}