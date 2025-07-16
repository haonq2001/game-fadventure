using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NPC_Chau : MonoBehaviour
{
   public GameObject panelNhiemVuDialog; // Panel hiển thị nhiệm vụ  
   
    public Text dialogNhiemVuText; // Text để cập nhật nội dung nhiệm vụ  
    private bool playerIsClose = false; // Kiểm tra xem người chơi có ở gần không  
    private bool isHoanThanhNV = false; // Kiểm tra xem nhiệm vụ đã hoàn thành chưa  

    void Update()  
    {  
        // Kiểm tra xem người chơi có nhấn phím E khi ở gần NPC không  
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))  
        {  
            if (!isHoanThanhNV) // Nếu nhiệm vụ chưa hoàn thành  
            {  
                CompleteMission(); // Hoàn thành nhiệm vụ và cập nhật UI  
            }  
            else  
            {  
                // Logic khác nếu nhiệm vụ đã hoàn thành  
                Debug.Log("Nhiệm vụ đã hoàn thành!");   
            }  
        }  
    }  

    private void OnTriggerEnter2D(Collider2D other)  
    {  
        if (other.CompareTag("Player")) // Kiểm tra va chạm với người chơi  
        {  
            playerIsClose = true; // Đánh dấu người chơi đang ở gần
            if (!isHoanThanhNV) // Nếu nhiệm vụ chưa hoàn thành  
            {  
                CompleteMission(); // Hoàn thành nhiệm vụ và cập nhật UI  
            }  
             
        }  
    }  

    private void OnTriggerExit2D(Collider2D other)  
    {  
        if (other.CompareTag("Player")) // Khi người chơi rời đi  
        {  
            playerIsClose = false; // Đánh dấu người chơi không còn ở gần  
            // panelNhiemVuDialog.SetActive(false); // Ẩn bảng nhiệm vụ  
        }  
    }  

    private void CompleteMission() // Phương thức hoàn thành nhiệm vụ  
    {  
        isHoanThanhNV = true; // Đánh dấu nhiệm vụ đã hoàn thành  

        // Cập nhật nội dung text nhiệm vụ  
        dialogNhiemVuText.text = "Nhiệm vụ: Cứu cháu trai : (1/1)";  

        // Hiển thị panel nhiệm vụ  
        panelNhiemVuDialog.SetActive(true);   
        guiThongBaoOng();
        
        // Tạo hiệu ứng hoặc thông báo cho người chơi  
        Debug.Log("Nhiệm vụ đã được cập nhật!");  
    }  
    void guiThongBaoOng()
    {
        NPC_Yorn yorn = FindObjectOfType<NPC_Yorn>(); // Tìm NPC Ông
        yorn.hoanThanhNV();
    }
}
