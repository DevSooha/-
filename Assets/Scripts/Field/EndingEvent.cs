using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingEvent : MonoBehaviour
{
    [Header("설정")]
    public GameObject endingTextObj; // 꼭 인스펙터에서 연결하고, 미리 꺼두세요!
    public string titleSceneName = "TitleScene";

    // Start나 OnTriggerEnter 다 지우고 이거 하나만 둡니다.
    public void PlayEnding()
    {
        StartCoroutine(EndingRoutine());
    }

    IEnumerator EndingRoutine()
    {

        // 2. 글씨 켜기
        if (endingTextObj != null) endingTextObj.SetActive(true);

        // 3. [핵심] 플레이어 오브젝트를 끄지 말고, 그림과 물리만 끕니다!
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // (1) 모습 숨기기 (스프라이트 렌더러만 끔)
            SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            // (2) 물리 끄기 (충돌 방지)
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false; // 물리 연산 중단

            // (3) 이동 멈추기 (혹시 모르니 속도 0)
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("엔딩 연출 시작...");

        // 4. 5초 대기 후 종료
        yield return new WaitForSeconds(5.0f);

        if (!string.IsNullOrEmpty(titleSceneName))
            SceneManager.LoadScene(titleSceneName);
        else
            Application.Quit();
    }
}