using Unity.VisualScripting;
using UnityEngine;

public class TestInventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData testItem1;
    [SerializeField] private ItemData testItem2;
    [SerializeField] private PotionData testPotion;
    
    private void Start()
    {
        inventory.AddItem(testItem1, 30);
        inventory.AddItem(testItem2, 50);
        inventory.AddItem(testPotion, 1);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(testItem1, 100);
            Debug.Log("아이템 추가!");
        }
    }
}

