using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Yorn : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue; // Hội thoại NPC Ông khi chưa hoàn thành nhiệm vụ 
    public string thankYouMessage; // Hội thoại cảm ơn khi nhiệm vụ hoàn thành  
    public string dialogueMayMan; // Hội thoại chúc may mắn
    private int dialogueIndex;
    public float typingSpeed;
    public bool playerIsClose;
    public bool isNhanNV; // Trạng thái nhận nhiệm vụ
    public bool isNhanqua; // Trạng thái nhận quà
    public GameObject continueButton;
    public GameObject continueButton1;

    public GameObject BTNnhiemvu;
    public GameObject btnTangQua; // Nút để nhận quà  

    // Thêm biến cho nhiệm vụ
    public GameObject Panel_nhiemVu_Dialog; // Text bên trái hiển thị nhiệm vụ
    public Text Dialog_nhiemVu_Text; // Nội dung nhiệm vụ
    public GameObject NhanNV_TextPrefab; // Prefab chữ "Thành công" hiển thị trên đầu
    // khai báo image quà tặng
    public GameObject ImgquaTang;
    // khai báo số lượng đá
    public Text SoLuongDa_Txt;
    private int stoneCount = 0; // Số lượng viên đá
    private bool isHoanThanhNV = false; // Trạng thái hoàn thành nhiệm vụ

    private bool isQuayLai = false; // Cờ kiểm tra người chơi có thể quay lại tương tác hay không
    public gamemanagermap3 Gamemanagermap3;

    public GameObject cong1;
    public GameObject cong2;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        Panel_nhiemVu_Dialog.SetActive(false);
        ImgquaTang.SetActive(false);
        continueButton1.SetActive(false);
        ZeroText();
        isNhanNV = false;
        cong1.SetActive(false);
        cong2.SetActive(false);
    }

    void Update()
    {
        // Nếu người chơi gần NPC và nhấn E, kiểm tra xem có thể tương tác hay không
        if (playerIsClose && Input.GetKeyDown(KeyCode.E) && !isQuayLai)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                ZeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if (dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
        {
            if (continueButton.activeSelf)
            {
                NextLine();
            }
        }

        if (dialogueText.text == dialogue[dialogueIndex])
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }

        if (stoneCount >= 1 && !isHoanThanhNV)
        {
            CompleteMission(); // Gọi hàm hoàn thành nhiệm vụ
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            if (stoneCount >= 1 && !isHoanThanhNV)
            {
                CompleteMission(); // Gọi hàm hoàn thành nhiệm vụ
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            ZeroText();
        }
    }

    public void ZeroText()
    {
        dialogueText.text = "";
        dialogueIndex = 0;
        dialoguePanel.SetActive(false);
        continueButton.SetActive(false);
        BTNnhiemvu.SetActive(false);
        btnTangQua.SetActive(false);
        if (isHoanThanhNV)
        {
            btnTangQua.SetActive(true); // Hiển thị nút nhận quà khi nhiệm vụ hoàn thành
        }
    }

    IEnumerator Typing()
    {
        string currentDialogue = "";

        // Kiểm tra trạng thái nhiệm vụ
        if (isHoanThanhNV)
        {
            currentDialogue = thankYouMessage;
        }
        else if (isNhanNV || isNhanqua)
        {
            currentDialogue = dialogueMayMan;
        }
        else
        {
            currentDialogue = dialogue[dialogueIndex];
        }

        dialogueText.text = "";

        // Hiển thị từng ký tự một với tốc độ typingSpeed
        foreach (char letter in currentDialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Khi hội thoại kết thúc, bật tiếp các nút nếu cần
        if (isHoanThanhNV)
        {
            ImgquaTang.SetActive(true); // Hiển thị hình ảnh quà tặng
            btnTangQua.SetActive(true); // Hiển thị nút nhận quà
        }
        else if (!isNhanNV)
        {
            // Nút nhận nhiệm vụ chỉ xuất hiện sau khi hoàn thành toàn bộ hội thoại
            if (dialogueIndex == dialogue.Length - 1)
            {
                BTNnhiemvu.SetActive(true); // Hiển thị nút nhận nhiệm vụ sau khi hết hội thoại
            }
        }

        // Hiển thị nút Tiếp tục khi dialogMayMan hiển thị
        if (currentDialogue == dialogueMayMan)
        {
            continueButton1.SetActive(true); // Bật nút tiếp tục khi hiển thị dialogueMayMan
        }
        else
        {
            continueButton1.SetActive(false); // Ẩn nút tiếp tục cho các hội thoại khác
        }
    }

    public void NextLine()
    {
        continueButton.SetActive(false);

        if (isHoanThanhNV)
        {
            return;
        }

        if (dialogueIndex < dialogue.Length - 1)
        {
            dialogueIndex++;
            StartCoroutine(Typing());
        }
        else
        {
            // Sau khi hội thoại kết thúc, hiện nút nhận nhiệm vụ
            ZeroText();
            BTNnhiemvu.SetActive(true); // Hiển thị nút nhận nhiệm vụ khi hội thoại hoàn tất
        }
    }

    public void OnNhiemVuButtonClick()
    {
        Debug.Log("Người chơi đã nhận nhiệm vụ!");
        BTNnhiemvu.SetActive(false); // Ẩn nút sau khi nhận nhiệm vụ

        // Hiển thị nhiệm vụ bên trái màn hình
        Panel_nhiemVu_Dialog.SetActive(true);
        isNhanNV = true; // Đánh dấu đã nhận nhiệm vụ
        Dialog_nhiemVu_Text.text = "Nhiệm vụ: Cứu cháu trai : (0/1)";
        ZeroText();
    }

    // Sự kiện click nhận quà
    public void OnNhanQuaButtonClick()
    {
        stoneCount++; // Tăng số lượng viên đá lên 1
        SoLuongDa_Txt.text = " " + stoneCount.ToString(); // Cập nhật số lượng viên đá lên Text  
        Gamemanagermap3.AddScorehp();
        Gamemanagermap3.AddScoremp();

        // Sau khi nhận quà, tiếp tục hiển thị hội thoại "Chúc may mắn"
        ZeroText(); // Ẩn hội thoại hiện tại
        Panel_nhiemVu_Dialog.SetActive(false); // Ẩn nhiệm vụ bên trái

        // Cập nhật trạng thái là đã nhận quà và tiếp tục hội thoại chúc may mắn
        isNhanqua = true; // Đánh dấu đã nhận quà
        StartCoroutine(Typing()); // Hiển thị lời thoại "Chúc may mắn"
        cong1.SetActive(true);
        cong2.SetActive(true);

        // Đánh dấu NPC không thể tương tác nữa
        isQuayLai = true; // Ngừng cho phép tương tác với NPC sau khi nhận quà
    }

    public void OnTiepTuc1ButtonClick()
    {
        ZeroText();
        // StartCoroutine(Typing());
    }

    private void CompleteMission()
    {
        isHoanThanhNV = true;

        // Cập nhật text nhiệm vụ bên trái
        Dialog_nhiemVu_Text.text = "Nhiệm vụ: Cứu cháu trai : (1/1)";

        // Tạo hiệu ứng hoặc thông báo
        Debug.Log("Nhiệm vụ đã hoàn thành!");
    }

    public void hoanThanhNV()
    {
        isHoanThanhNV = true; // Đánh dấu nhiệm vụ đã hoàn thành  
        dialogueIndex = 0; // Reset chỉ số hội thoại  
        StartCoroutine(Typing()); // Hiển thị hội thoại cảm ơn  
    }
}