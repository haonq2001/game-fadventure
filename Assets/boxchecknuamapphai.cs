using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxchecknuamaptrai : MonoBehaviour
{  // Start is called before the first frame update
    public GameObject cameramininuamap2phai;
    public GameObject cameramininuamap2trai;

    void Start()
    {
       
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cameramininuamap2phai.SetActive(true);
            cameramininuamap2trai.SetActive(false);
        }
    }
}
