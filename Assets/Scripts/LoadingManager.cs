using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

    public static string NEXT_SCENE = "hao";
    public GameObject progressBar;
    public Text textPerenct;
    private float fixedLoadingTime = 3f;

    private void Start()
    {
        StartCoroutine(LoadSceneFixedTime(NEXT_SCENE));
    }
    public IEnumerator LoadSceneAsync(string scencName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scencName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.GetComponent<Image>().fillAmount = progress;
            textPerenct.text = (progress * 100).ToString("0") + "%";

            yield return null;
        }

    }
    public IEnumerator LoadSceneFixedTime(string scencName)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fixedLoadingTime)
        {
            float progress = Mathf.Clamp01(elapsedTime / fixedLoadingTime);
            progressBar.GetComponent<Image>().fillAmount = progress;
            textPerenct.text = (progress * 100).ToString("0") + "%";

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        SceneManager.LoadScene(scencName);

    }

}
