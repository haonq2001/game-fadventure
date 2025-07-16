using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxchecknuamap2trai : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cameramininuamap2trai;
    public GameObject cameramininuamap2phai;

    void Start()
    {
        //cameramininuamap2trai.SetActive(true);
        //cameramininuamap2phai.SetActive(false);
    }

    // Update is called once per frame
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
        
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cameramininuamap2trai.SetActive(true);
            cameramininuamap2phai.SetActive(false);
        }
    }
}
