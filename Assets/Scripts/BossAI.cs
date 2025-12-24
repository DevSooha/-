using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Pattern Settings")]
    public GameObject fireBallPrefab; // 파이어볼 프리팹 연결
    public float patternInterval = 4.0f; // 딜레이 타임 4초


    public void StartBattle()
    {
        StartCoroutine(PatternRoutine());
    }

    IEnumerator PatternRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f); // 전투 시작 전 잠시 대기

            Debug.Log("[BOSS] 파이어월 시전!");
            yield return StartCoroutine(FireWallPattern());

            Debug.Log("[BOSS] 지침... 4초 휴식");
            yield return new WaitForSeconds(patternInterval);
        }
    }

    IEnumerator FireWallPattern()
    {
        for (int i = 0; i < 2; i++)
        {
            SpawnFireWall();
            yield return new WaitForSeconds(0.7f); // 발사 간격 0.7초 [cite: 147]
        }
    }

    void SpawnFireWall()
    {
        // 전방 180도, 45도 간격으로 5발 발사 (-90 ~ +90) [cite: 145, 146]
        // 보스는 기본적으로 아래(Vector2.down)를 본다고 가정
        Vector2 baseDir = Vector2.down;

        float[] angles = { -90, -45, 0, 45, 90 };

        foreach (float angle in angles)
        {
            // 각도 계산 (Quaternion * Vector)
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

            // 투사체 생성
            GameObject ball = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            ball.GetComponent<FireBall>().Setup(dir);
        }
    }
}