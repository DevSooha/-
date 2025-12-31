using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // ★ 싱글톤 인스턴스
    public static Inventory Instance { get; private set; }

    // ★ 다른 스크립트(UI)에서 접근할 수 있게 public으로 변경했습니다.
    public InventoryUI inventoryUI;

    [SerializeField] private int slotPerMaterialPage = 6;
    [SerializeField] private int slotPerPotionPage = 5;

    private List<Item> materialItems = new List<Item>();
    private List<Item> potionItems = new List<Item>();

    private int currentMaterialPage = 0;
    private int currentPotionPage = 0;

    public int CurrentMaterialPage => currentMaterialPage;
    public int CurrentPotionPage => currentPotionPage;

    public int MaxMaterialPage => Mathf.CeilToInt((float)materialItems.Count / slotPerMaterialPage);
    public int MaxPotionPage => Mathf.CeilToInt((float)potionItems.Count / slotPerPotionPage);

    public int SelectedIndex { get; private set; } = -1;

    public List<Item> MaterialItems => materialItems;
    public List<Item> PotionItems => potionItems;

    // ---------- 생명주기 ----------

    void Awake()
    {
        // 1. 싱글톤 패턴: 이미 있는 '형님'이 있으면 나는 죽는다
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 2. 내가 대장이다
        Instance = this;

        // 3. 씬이 바뀌어도 파괴되지 않음 (아이템 데이터 유지의 핵심)
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 씬이 시작될 때 연결된 UI가 있다면 화면을 갱신
        if (inventoryUI != null)
        {
            inventoryUI.RefreshUI();
        }
    }

    // ---------- 공통 헬퍼들 ----------

    private List<Item> GetList(ItemCategory category)
    {
        return category switch
        {
            ItemCategory.Material => materialItems,
            ItemCategory.Potion => potionItems,
            _ => materialItems
        };
    }

    private int GetSlotPerPage(ItemCategory category)
    {
        return category switch
        {
            ItemCategory.Material => slotPerMaterialPage,
            ItemCategory.Potion => slotPerPotionPage,
            _ => slotPerMaterialPage
        };
    }

    private ref int GetCurrentPageRef(ItemCategory category)
    {
        if (category == ItemCategory.Material)
            return ref currentMaterialPage;
        else
            return ref currentPotionPage;
    }

    // ---------- 아이템 관리 ----------

    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        ItemCategory category = itemData.category;

        List<Item> list = GetList(category);
        int slotPerPage = GetSlotPerPage(category);
        ref int currentPage = ref GetCurrentPageRef(category);

        int remaining = quantity;

        if (itemData.isStackable)
        {
            foreach (Item item in list)
            {
                if (item.data == itemData && item.quantity < itemData.maxStack)
                {
                    int addAmount = Mathf.Min(quantity, itemData.maxStack - item.quantity);
                    item.quantity += addAmount;
                    remaining -= addAmount;

                    if (remaining <= 0)
                    {
                        // ★ UI가 연결되어 있을 때만 갱신 (Null 에러 방지)
                        if (inventoryUI != null) inventoryUI.RefreshUI();
                        return true;
                    }
                }
            }
        }

        while (remaining > 0)
        {
            int addNow = itemData.isStackable ? Mathf.Min(remaining, itemData.maxStack) : 1;
            list.Add(new Item(itemData, addNow));
            remaining -= addNow;
        }

        int newMaxPage = Mathf.CeilToInt((float)list.Count / slotPerPage);
        if (newMaxPage <= 0) newMaxPage = 1;

        if (currentPage >= newMaxPage)
        {
            currentPage = newMaxPage - 1;
        }

        // ★ 추가 완료 후 UI 갱신
        if (inventoryUI != null) inventoryUI.RefreshUI();

        return true;
    }

    public List<Item> GetCurrentItems(ItemCategory category)
    {
        List<Item> source = GetList(category);
        int slotPerPage = GetSlotPerPage(category);
        int currentPage = GetCurrentPageRef(category);

        List<Item> pageItems = new List<Item>();
        int startIndex = currentPage * slotPerPage;
        int endIndex = Mathf.Min(startIndex + slotPerPage, source.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            pageItems.Add(source[i]);
        }

        return pageItems;
    }

    public void NextPage(ItemCategory category)
    {
        List<Item> list = GetList(category);
        int slotPerPage = GetSlotPerPage(category);
        ref int currentPage = ref GetCurrentPageRef(category);

        int maxPage = Mathf.Max(1, Mathf.CeilToInt((float)list.Count / slotPerPage));
        currentPage++;

        if (currentPage >= maxPage)
            currentPage = 0;

        if (inventoryUI != null) inventoryUI.RefreshUI();
    }

    public void SetPage(ItemCategory category, int page)
    {
        List<Item> list = GetList(category);
        int slotPerPage = GetSlotPerPage(category);
        ref int currentPage = ref GetCurrentPageRef(category);

        int maxPage = Mathf.Max(1, Mathf.CeilToInt((float)list.Count / slotPerPage));
        currentPage = Mathf.Clamp(page, 0, maxPage - 1);

        if (inventoryUI != null) inventoryUI.RefreshUI();
    }

    public bool RemoveItem(ItemCategory category, int index, int quantity = 1)
    {
        List<Item> list = GetList(category);
        if (index < 0 || index >= list.Count) return false;

        list[index].quantity -= quantity;
        if (list[index].quantity <= 0)
        {
            list.RemoveAt(index);
        }

        if (inventoryUI != null) inventoryUI.RefreshUI();
        return true;
    }

    public void SelectItem(ItemCategory category, int index)
    {
        List<Item> list = GetList(category);

        if (index < 0 || index >= list.Count)
        {
            SelectedIndex = -1;
        }
        else
        {
            SelectedIndex = index;
        }
    }
}