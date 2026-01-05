using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public TextAsset csvFile;
    public Tilemap floorTilemap;

<<<<<<< HEAD
    [Header("공용 아이템 프리팹")]
    public GameObject worldItemPrefab;

=======
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    [System.Serializable]
    public struct SpawnMapping
    {
        public string itemID;
<<<<<<< HEAD
        public string spritePath;
        //public ItemData itemData;
=======
        public GameObject originalTemplate;
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }

    public List<SpawnMapping> spawnList;

<<<<<<< HEAD
=======
    void Awake()
    {
        // 1. 원본들은 숨깁니다.
        foreach (var mapping in spawnList)
        {
            if (mapping.originalTemplate != null)
                mapping.originalTemplate.SetActive(false);
        }
    }

>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    void Start()
    {
        SpawnFromCSV();
    }

    public void SpawnFromCSV()
    {
<<<<<<< HEAD
        if (csvFile == null) return;

        ClearPreviousSpawns();

        string[] lines = csvFile.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
=======
        if (csvFile == null) { Debug.LogError("CSV 파일이 연결되지 않았습니다!"); return; }

        ClearPreviousSpawns();

        // 유니티 텍스트 에셋 읽기 시 줄바꿈 처리
        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("CSV 줄 수: " + lines.Length);
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6

        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            if (data.Length < 6) continue;

            string itemID = data[0].Trim();
            string mapID = data[4].Trim();
            string rateStr = data[5].Trim().Replace("%", "");

<<<<<<< HEAD
            if (mapID != "spr_3") continue;

            float spawnRate = float.Parse(rateStr) / 100f;

            // 리스트에서 ID에 맞는 맵핑 데이터 찾기
            SpawnMapping mapping = spawnList.Find(x => x.itemID == itemID);

            // ★ [수정됨] spritePath 체크 대신 itemID가 맞는게 있는지 체크
            if (!string.IsNullOrEmpty(mapping.itemID))
            {
                TrySpawn(mapping, spawnRate, itemID);
=======
            if (mapID == "spr_3")
            {
                float spawnRate = float.Parse(rateStr) / 100f;
                GameObject template = spawnList.Find(x => x.itemID == itemID).originalTemplate;

                if (template != null)
                {
                    TrySpawnClone(template, spawnRate, itemID);
                }
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
            }
        }
    }

<<<<<<< HEAD
    void TrySpawn(SpawnMapping mapping, float rate, string id)
    {
        if (Random.value > rate) return;

        BoundsInt bounds = floorTilemap.cellBounds;

=======
    void TrySpawnClone(GameObject original, float rate, string id)
    {
        float dice = Random.value;
        if (dice > rate) return;

        BoundsInt bounds = floorTilemap.cellBounds;
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        for (int attempts = 0; attempts < 100; attempts++)
        {
            Vector3Int randomCell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

<<<<<<< HEAD
            if (!floorTilemap.HasTile(randomCell)) continue;

            Vector3 spawnPos = floorTilemap.GetCellCenterWorld(randomCell);
            spawnPos.z = 0;

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.3f);
            if (hit != null && hit.CompareTag("Obstacle")) continue;

            // 1. 아이템 생성
            GameObject item = Instantiate(worldItemPrefab, spawnPos, Quaternion.identity, transform);

            // ★★★ [핵심 수정] WorldItem 컴포넌트 초기화 (Init 호출) ★★★
            WorldItem worldItemScript = item.GetComponent<WorldItem>();

            //if (mapping.itemData != null)
            //{
            //    // 여기서 Init을 해줘야 WorldItem의 initialized가 true가 되어 주워집니다.
            //    worldItemScript.Init(mapping.itemData, 1);
            //}
            //else
            //{
            //    Debug.LogError($"Spawner: {id}에 해당하는 ItemData가 Inspector에 연결되지 않았습니다!");
            //}

            // 2. 스프라이트 설정 (기존 로직 유지)
            Sprite sprite = Resources.Load<Sprite>(mapping.spritePath);
            if (sprite != null)
            {
                item.GetComponent<SpriteRenderer>().sprite = sprite;
            }

            // 3. 태그 설정 (Item 태그여야 PlayerInteraction이 인식함)
            // PlayerInteraction에서 "Item" 태그를 줍게 되어 있다면 "Item"으로, 
            // "Respawn"으로직을 따로 짰다면 "Respawn" 유지.
            // 보통 줍기 로직은 "Item" 태그를 씁니다. 확인 필요!
            item.tag = "Item";

            item.SetActive(true);
            return;
        }
=======
            if (floorTilemap.HasTile(randomCell))
            {
                Vector3 spawnPos = floorTilemap.GetCellCenterWorld(randomCell);
                spawnPos.z = 0;

                Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.3f);

                if (hit == null || !hit.CompareTag("Obstacle"))
                {
                    GameObject clone = Instantiate(original, spawnPos, Quaternion.identity, transform);
                    
                    clone.SetActive(true);
                    clone.tag = "Respawn";
                    return;
                }
            }
        }
        Debug.LogWarning($"{id} 스폰 실패: 위치 못 찾음");
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
    }

    void ClearPreviousSpawns()
    {
<<<<<<< HEAD
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // 태그가 Item이든 Respawn이든 Spawner 자식이면 다 지움
            Destroy(transform.GetChild(i).gameObject);
=======
        // 내 하위(자식) 오브젝트들 중에서만 찾아서 파괴
        // 리스트를 역순으로 도는 게 삭제할 때 안전함
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("Respawn"))
            {
                Destroy(child.gameObject);
            }
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
        }
    }
}