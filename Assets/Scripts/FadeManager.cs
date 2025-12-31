using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public Image fadeImage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOut(float duration)
    {
        // ★ 씬 바뀌어서 놓쳤으면 다시 찾아라
        if (fadeImage == null)
        {
            GameObject obj = GameObject.Find("FadeImage"); // 이름 중요!!
            if (obj != null) fadeImage = obj.GetComponent<Image>();
        }

        if (fadeImage != null)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (fadeImage != null)
                    fadeImage.color = new Color(0, 0, 0, t / duration);
                yield return null;
            }
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        if (fadeImage == null)
        {
            GameObject obj = GameObject.Find("FadeImage");
            if (obj != null) fadeImage = obj.GetComponent<Image>();
        }

        if (fadeImage != null)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (fadeImage != null)
                    fadeImage.color = new Color(0, 0, 0, 1f - (t / duration));
                yield return null;
            }
        }
    }
}