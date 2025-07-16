using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tranformplayer : MonoBehaviour
{

    public Transform player; // Gắn Transform của nhân vật
    public Vector3 offset; // Offset từ vị trí nhân vật
    public RectTransform usernameUI; // Gắn RectTransform của Text hoặc UI

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Lấy camera chính
    }

    void Update()
    {
        if (player != null && usernameUI != null)
        {
            // Chuyển đổi vị trí thế giới sang màn hình
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + offset);
            usernameUI.position = screenPos;
        }
    }
}
