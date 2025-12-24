using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤

    [Header("연결할 UI")]
    public GameObject messagePanel;       // 배경 패널
    public TextMeshProUGUI messageText;   // 글자 텍스트

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 외부(MapNode)에서 이 함수를 부를 겁니다.
    public void ShowWarning(string message)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        messagePanel.SetActive(true);
        messageText.text = message;

        currentRoutine = StartCoroutine(HideRoutine());
    }

    IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(2.0f); // 2초 뒤 꺼짐
        messagePanel.SetActive(false);
    }
}