using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private CraftUI craftUI;
    [SerializeField] private PotionManager potionManager;
    
    [SerializeField] private Transform materialContainer;
    [SerializeField] private Button materialPageButton;
 
    [SerializeField] private Transform potionContainer;
    [SerializeField] private Button potionPageButton;
    
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextMeshProUGUI materialPageText;
    [SerializeField] private TextMeshProUGUI potionPageText;
    
    [SerializeField] private int slotsPerMaterialPage = 6;
    [SerializeField] private int slotsPerPotionPage = 5;
    
    private InventorySlot[] materialSlots;
    private InventorySlot[] potionSlots;

    private int materialCurrentPage = 0;
    private int potionCurrentPage = 0;


    private void Awake()
    {
        InitializeSlots(ItemCategory.Material);
        InitializeSlots(ItemCategory.Potion);
        
        materialPageButton.onClick.AddListener(() => NextMaterialPage());
        potionPageButton.onClick.AddListener(() => NextPotionPage());
        
        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = gameObject.activeSelf;
            gameObject.SetActive(!isActive);
        }
    }
    private void FixedUpdate()
    {
        RefreshUI();
    }


    private void InitializeSlots(ItemCategory category)
    {
        int slotCount = (category == ItemCategory.Material) ? slotsPerMaterialPage : slotsPerPotionPage;
        Transform container = (category == ItemCategory.Material) ? materialContainer : potionContainer;

        InventorySlot[] slots = new InventorySlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, container);

            RectTransform rect = slotObj.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;        // 크기를 1로 강제! (제일 중요)
            rect.localPosition = Vector3.zero;    // 위치를 0으로 (Z축 가출 방지)
            rect.localRotation = Quaternion.identity; // 회전 초기화

            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slot.Init(this, i, category);
            slots[i] = slot;
        }

        if (category == ItemCategory.Material)
            materialSlots = slots;
        else if (category == ItemCategory.Potion)
            potionSlots = slots;
    }


    private void NextMaterialPage()
    {
        int maxPage = GetMaxPage(ItemCategory.Material, slotsPerMaterialPage);
        materialCurrentPage++;
        if (materialCurrentPage >= maxPage)
            materialCurrentPage = 0;
        RefreshUI();
    }

    private void NextPotionPage()
    {
        int maxPage = GetMaxPage(ItemCategory.Potion, slotsPerPotionPage);
        potionCurrentPage++;
        if (potionCurrentPage >= maxPage)
            potionCurrentPage = 0;
        RefreshUI();
    }

    public void OnSlotClicked(ItemCategory category, int localIndex)
    {
        List<Item> allItems = (category == ItemCategory.Material) ?
            inventory.MaterialItems : inventory.PotionItems;

        int currentPage = (category == ItemCategory.Material) ? materialCurrentPage : potionCurrentPage;
        int slotPerPage = (category == ItemCategory.Material) ? slotsPerMaterialPage : slotsPerPotionPage;

        int globalIndex = currentPage * slotPerPage + localIndex;

        if (globalIndex < 0 || globalIndex >= allItems.Count)
            return;

        Item selectedItem = allItems[globalIndex];
        if (selectedItem == null || selectedItem.data == null)
            return;

        // 1. 재료 클릭 시 (크래프팅 UI로 전달)
        if (category == ItemCategory.Material && craftUI != null)
        {
            craftUI.OnMaterialSelected(selectedItem);
        }

        // ★ [수정됨] 포션 장착 연결 부분
        if (category == ItemCategory.Potion)
        {
            // 1. PlayerAttackSystem 찾기
            PlayerAttackSystem attackSystem = FindFirstObjectByType<PlayerAttackSystem>();

            if (attackSystem != null)
            {
                // 2. 장착 함수 호출!
                attackSystem.EquipPotionFromInventory(selectedItem);
            }
        }
    }


    public void RefreshUI()
    {
        RefreshCategoryUI(ItemCategory.Material, materialSlots, materialCurrentPage, slotsPerMaterialPage, materialPageText);
        RefreshCategoryUI(ItemCategory.Potion, potionSlots, potionCurrentPage, slotsPerPotionPage, potionPageText);

    }
    public void RefreshCategoryUI(ItemCategory category, InventorySlot[] slots, int currentPage, int slotPerPage, TextMeshProUGUI pageTextUI)
    {
        List<Item> allItems = (category == ItemCategory.Material) ? 
            inventory.MaterialItems : inventory.PotionItems;
        
        int startIndex = currentPage * slotPerPage;
        int endIndex = Mathf.Min(startIndex + slotPerPage, allItems.Count);
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (startIndex + i < endIndex)
                slots[i].SetItem(allItems[startIndex + i]);
            else
                slots[i].Clear();
        }
        
        int maxPage = GetMaxPage(category, slotPerPage);
        pageTextUI.text = $"{currentPage + 1} / {maxPage}";
    }
    private int GetMaxPage(ItemCategory category, int slotPerPage)
    {
        List<Item> allItems = category == ItemCategory.Material ? 
            inventory.MaterialItems : inventory.PotionItems;
        
        return Mathf.Max(1, Mathf.CeilToInt((float)allItems.Count / slotPerPage));
    }
}
