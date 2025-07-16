  
 
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UserName : MonoBehaviour
    {
        public Text username;
        public GameObject userName;
        public Button buttonOK;
        public InputField ipUsername;

        private FirebaseDatabaseManager databaseManager;
        private static UserName instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // Đảm bảo đối tượng không bị hủy khi chuyển Scene
            }
            else
            {
                Destroy(gameObject); // Nếu đối tượng đã tồn tại, xóa đối tượng mới tạo
            }
        }
        void Start()
        {
            databaseManager = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManager>();
            if (LoadDataManager.userInGame.Name == "")
            {
                userName.SetActive(true);
            }
            else
            {
                username.text = LoadDataManager.userInGame.Name;
            }

            buttonOK.onClick.AddListener(SetNewUsername);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetNewUsername()
        {
            if (ipUsername.text != "")
            {
                LoadDataManager.userInGame.Name = ipUsername.text;

                databaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId, LoadDataManager.userInGame.ToString());

                username.text = ipUsername.text;
                userName.SetActive(false);
            }
        }
    }