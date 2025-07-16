using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using System;
using Firebase.Extensions;

public class FirebaseDatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;

    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;

    }
    // private void Start()
    // {
    //     WriteDatabase("123", "Xin chao the gioi");
    //     ReadDatabase("123");
    // }

    public void WriteDatabase(string path, string message)
    {
        reference.Child(path).SetValueAsync(message).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("ghi du lieu thanh cong");
            }
            else
            {
                Debug.Log("ghi du lieu that bai: " + task.Exception);
            }
        });
    }

    public void ReadDatabase(string id)
    {
        reference.Child("Users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("doc du lieu thanh cong: " + snapshot.Value.ToString());
            }
            else
            {
                Debug.Log("doc du lieu that bai: " + task.Exception);
            }
        });
    }
}
