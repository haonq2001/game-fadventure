using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CotDuocController : MonoBehaviour
{
    public static CotDuocController cotDuocInstance;
    private int soNgonDuocDaThap = 0; // Số ngọn đuốc đã thắp
    public Animator doorAnimator;      // Animator của cánh cửa
    public GameObject border;          // Thêm biến border


   //private bool isDoorOpened = false; // Kiểm tra nếu cánh cửa đã mở

    private void Awake()
    {
        if (cotDuocInstance == null)
        {
            cotDuocInstance = this;
            Debug.Log("cotDuocInstance đã được khởi tạo");
        }
        else
        {
            Debug.LogWarning("Đã có một instance của CotDuocController, đối tượng này sẽ không được sử dụng.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (cotDuocInstance == null)

        { 
            border.SetActive(false); // Đảm bảo border được tắt ban đầu
            if (cotDuocInstance == null)
            {
                cotDuocInstance = this;
                Debug.Log("cotDuocInstance được khởi tạo trong Start.");
            }
        }
    } 

        public void TangSoNgonDuocDaThap()
        {
            // if (isDoorOpened) return;

            soNgonDuocDaThap++;
            Debug.Log("Số ngọn đuốc đã thắp: " + soNgonDuocDaThap);

            if (soNgonDuocDaThap >= 6)
            {
                // isDoorOpened = true;
                doorAnimator.SetBool("OpenDoor", true);
                Debug.Log("Đủ 5 ngọn đuốc - mở cửa");
            }
        }


        public void Quaman()
        {
            border.SetActive(true); // Kích hoạt border
        }

    
}
