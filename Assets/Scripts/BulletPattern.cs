using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPattern : MonoBehaviour
{
    private GameObject bulletPrefab;
    private BulletPatternData patternData;
    private Vector3 explosionCenter;
    
    private float elapsedTime = 0f;
    private bool isActive = false;
    private float startDelay = 2f;
    private float cycleDuration = 8f;
    private bool isFiring = false; 
    private bool hasFired = false;

    public float shotInterval = 0.2f;   // 발 사이 간격(초)

    public void Initialize(GameObject bulletPrefab, BulletPatternData patternData, Vector3 center, float startDelay)
    {
        this.bulletPrefab = bulletPrefab;
        this.patternData = patternData;
        this.explosionCenter = center;
        this.startDelay = startDelay; // ⭐ 0 또는 2
        
        isActive = true;
        elapsedTime = 0f;
    }

    private void Update()
{
    if (!isActive) return;

    elapsedTime += Time.deltaTime;
    float timeInCycle = elapsedTime % cycleDuration;

    float fireTime1 = startDelay;
    float fireTime2 = startDelay + 4f;
    float tolerance = 0.1f;

    bool isInWindow =
        Mathf.Abs(timeInCycle - fireTime1) < tolerance ||
        Mathf.Abs(timeInCycle - fireTime2) < tolerance;

    if (isInWindow)
{
    if (!isFiring && !hasFired)
    {
        StartCoroutine(FireRoutine());
        timeInCycle = elapsedTime - shotInterval * 2;
        hasFired = true;
    }
}
else
{
    hasFired = false;
}
}
    IEnumerator FireRoutine()
{
    isFiring = true;
    for (int i = 0; i < 3; i++)
    {
        FireBullet();
        yield return new WaitForSeconds(shotInterval);
    }
    isFiring= false;
}
    private void FireBullet()
    {
        Vector2[] directions = GetDirections();
        
        foreach (Vector2 direction in directions)
        {
                Vector3 spawnPos = explosionCenter;

                GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                
                if (bullet != null)
                {
                    bullet.Initialize(
                        direction,
                        patternData.bulletSpeed,
                        patternData.damage,
                        patternData.bulletEffect
                    );
                }
            
        }

        Debug.Log($"[{patternData.element}] {patternData.bulletType} 탄막 발사! (Time: {elapsedTime:F2}s)");
    }

    private Vector2[] GetDirections()
    {
        if (patternData.bulletType == BulletType.Spiral1)
        {
            return new Vector2[]
            {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right
            };
        }
        else
        {
            return new Vector2[]
            {
                new Vector2(1, 1).normalized,
                new Vector2(-1, -1).normalized,
                new Vector2(-1, 1).normalized,
                new Vector2(1, -1).normalized
            };
        }
    }
}
