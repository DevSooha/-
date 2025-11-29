using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotionExplosion : MonoBehaviour
{
    private PotionData potionData;
    private GameObject bulletPatternPrefab;
    private GameObject bulletPrefabBlue;
    private GameObject bulletPrefabRed;
    private GameObject bulletPrefabGreen;
    
    private List<BulletPattern> activePatterns = new List<BulletPattern>();
    private float cycleDuration = 8f;

    public void Initialize(PotionData data, GameObject patternPrefab, GameObject bulletBlue, GameObject bulletRed, GameObject bulletGreen, Vector3 position)
    {
        if (data == null)
        {
            Debug.LogError("PotionData가 null입니다!");
            return;
        }

        potionData = data;
        bulletPatternPrefab = patternPrefab;
        bulletPrefabBlue = bulletBlue;
        bulletPrefabRed = bulletRed;
        bulletPrefabGreen = bulletGreen;
        transform.position = position;
        
        Debug.Log($"포션 폭발! {data.name}");
        
        StartCoroutine(FireAllPatterns(position));
    }

    private IEnumerator FireAllPatterns(Vector3 explosionPos)
    {
        List<BulletPatternData> patterns = potionData.GetPatterns();
        
        if (patterns == null || patterns.Count == 0)
        {
            Debug.LogError("포션에 탄막 패턴이 없습니다!");
            Destroy(gameObject);
            yield break;
        }

        float cooltime = 0.2f;
        
        for (int i = 0; i < patterns.Count; i++)
        {
            float startDelay = 2 + i * 2f;
            CreatePatternSpawner(patterns[i], explosionPos, startDelay);
        }
        
        yield return new WaitForSeconds(cycleDuration + cooltime * 2);
        CleanupAllPatterns();
    }

    private void CreatePatternSpawner(BulletPatternData patternData, Vector3 center, float startDelay)
    {
        GameObject patternObj = Instantiate(bulletPatternPrefab, center, Quaternion.identity);
        patternObj.name = $"Pattern_{patternData.element}_{patternData.bulletType}";
        
        BulletPattern pattern = patternObj.GetComponent<BulletPattern>();
        if (pattern == null)
            pattern = patternObj.AddComponent<BulletPattern>();
        
        GameObject bulletPrefab = null;
        switch (patternData.element)
        {
            case BulletElement.Fire: 
            bulletPrefab = bulletPrefabRed;
            break;
            case BulletElement.Water: 
            bulletPrefab = bulletPrefabBlue;
            break;
            case BulletElement.Lightning:
            bulletPrefab = bulletPrefabGreen;
            break;
        }
        pattern.Initialize(bulletPrefab, patternData, center, startDelay);
        activePatterns.Add(pattern);
    }

    private void CleanupAllPatterns()
    {
        foreach (BulletPattern pattern in activePatterns)
        {
            if (pattern != null)
                Destroy(pattern.gameObject);
        }
        activePatterns.Clear();
        Destroy(gameObject);
    }
}
