using UnityEngine;
<<<<<<< HEAD
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : Singleton<UIManager>
{
    [Header("Fade Settings")]
    public Image fadeImage;

    [Header("Message UI Settings")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private Coroutine messageRoutine;

    protected override void Awake()
    {
        base.Awake();
        // 초기 상태: UI 모두 비활성화
        if (messagePanel != null) messagePanel.SetActive(false);
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
    }

    #region Fade 기능
    public IEnumerator FadeIn(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, 1f - (t / duration));
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / duration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }
    #endregion

    #region 메시지 기능 (Warning vs Ending)

    // [Warning] 2초 뒤에 자동으로 사라짐
    public void ShowWarning(string message)
    {
        if (messageRoutine != null) StopCoroutine(messageRoutine);
=======
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("연결할 UI")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // [기존] 2초 뒤에 꺼지는 경고창
    public void ShowWarning(string message)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6

        messagePanel.SetActive(true);
        messageText.text = message;

<<<<<<< HEAD
        messageRoutine = StartCoroutine(HideMessageRoutine());
    }

    // [Ending] 자동으로 사라지지 않음 (EndingEvent에서 사용)
    public void ShowEnding(string message)
    {
        if (messageRoutine != null) StopCoroutine(messageRoutine);

        messagePanel.SetActive(true);
        messageText.text = message;
    }

    // UI를 수동으로 닫고 싶을 때 호출
    public void HideMessage()
    {
        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messagePanel.SetActive(false);
    }

    IEnumerator HideMessageRoutine()
=======
        currentRoutine = StartCoroutine(HideRoutine());
    }

    // ★ [추가] 엔딩용: 절대 자동으로 안 꺼짐!
    public void ShowEnding(string message)
    {
        // 혹시 켜져있던 끄기 타이머가 있다면 취소
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        messagePanel.SetActive(true);
        messageText.text = message;

        // HideRoutine()을 시작하지 않음 -> 계속 떠 있음
    }

    IEnumerator HideRoutine()
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    {
        yield return new WaitForSeconds(2.0f);
        messagePanel.SetActive(false);
    }
<<<<<<< HEAD
    #endregion
=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
}