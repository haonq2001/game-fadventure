using TMPro;
using UnityEngine;

public class UsernameFollow : MonoBehaviour
{
    public Transform target; // đầu player
    public Vector3 offset; // chỉnh lên đầu

    private RectTransform rectTransform;
    private Camera cam;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    void Update()
    {
        if (target != null && cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            rectTransform.position = screenPos;
        }
    }
}
